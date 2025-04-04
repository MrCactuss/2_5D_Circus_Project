// --- Complete GameManager.cs with Smooth Movement ---
using UnityEngine;
using System.Collections; // Required for Coroutines
using System.Collections.Generic;
using UnityEngine.UI; // Required if you add UI Text for turns/winner (Or TMPro)
// using TMPro; // Uncomment if using TextMeshPro for UI

public class GameManager : MonoBehaviour
{
    // Enum to track the current state of the game turn
    public enum GameState { WaitingForRoll, ProcessingMove, MovingPiece, GameOver }
    [Header("Game State")]
    public GameState currentState = GameState.WaitingForRoll;

    [Header("Game Setup")]
    public Transform[] boardSquares; // Assign 120 square center Transforms in Inspector (index 0 = square 1)
    public int playerCount = 2; // Default value, will be read from PlayerPrefs
    public int winningSquare = 120;

    [Header("Movement Settings")]
    public float moveSpeed = 3.0f; // How many world units the piece moves per second

    [Header("References")]
    public DiceRollScript diceScript; // Assign your Dice GameObject here in the Inspector
    // public Text turnIndicatorText; // Assign a UI Text element (Standard UI)
    // public TextMeshProUGUI turnIndicatorText; // OR Assign a TextMeshPro element

    // --- Private State ---
    private List<GameObject> playerPieces = new List<GameObject>(); // Holds references to the spawned player pieces
    private int[] playerPositions; // Tracks current square number for each player (0 = off board, 1-120 for squares)
    private int currentPlayerIndex = 0; // Index of the player whose turn it is
    private bool waitingForDiceResult = false; // Flag to prevent processing dice result multiple times
    private Coroutine activeMoveCoroutine = null; // Track the currently running movement coroutine


    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
    {
        Debug.Log("--- GameManager Start() Initializing ---");

        // --- 1. Get Player Count ---
        playerCount = PlayerPrefs.GetInt("PlayerCount", 2); // Read from prefs, default to 2 if not set

        // --- 2. Initialize Position Tracking ---
        playerPositions = new int[playerCount];
        for (int i = 0; i < playerCount; i++)
        {
            playerPositions[i] = 0; // All players start off-board (square 0)
        }

        // --- 3. Find Player Pieces Spawned by PlayerScript ---
        // Assumes PlayerScript runs first and tags prefabs with "PlayerPiece"
        playerPieces.Clear(); // Ensure list is empty before finding
        GameObject[] foundPieces = GameObject.FindGameObjectsWithTag("PlayerPiece");
        playerPieces.AddRange(foundPieces);

        // --- 4. Validate Piece Count ---
        if (playerPieces.Count != playerCount)
        {
            Debug.LogError($"GameManager ERROR: Expected {playerCount} objects tagged 'PlayerPiece', but found {playerPieces.Count}! Ensure PlayerScript runs first and prefabs are tagged correctly!");
            this.enabled = false; // Stop if setup is wrong
            return;
        }
        else
        {
            Debug.Log($"GameManager found {playerPieces.Count} player pieces.");
            // Optional: Sort playerPieces list here if needed
        }

        // --- 5. Set Initial Game State ---
        currentPlayerIndex = 0;
        currentState = GameState.WaitingForRoll;
        waitingForDiceResult = false;
        if (diceScript != null)
            diceScript.Initialize(0); // Initialize dice position and make it kinematic
        else
            Debug.LogError("Dice Script not assigned to GameManager in Inspector!");

        UpdateTurnIndicator(); // Update any UI showing whose turn it is
        Debug.Log($"Game Initialized for {playerCount} players. Player {currentPlayerIndex + 1}'s turn. Waiting for roll.");
    }


    void Update()
    {
        // Only allow rolling if waiting and no move is happening
        if (currentState == GameState.WaitingForRoll && diceScript != null && !waitingForDiceResult)
        {
            // Check the flag set by SideDetectorScript via DiceRollScript
            if (diceScript.isLanded)
            {
                waitingForDiceResult = true; // Process result only once per roll cycle
                ProcessDiceResult();
            }
        }
    }

    // Reads the dice result and starts handling the move
    void ProcessDiceResult()
    {
        int rolledNumber = 0;
        // Try to convert the dice face name string ("1", "2", etc.) into an integer number
        if (int.TryParse(diceScript.diceFaceNum, out rolledNumber) && rolledNumber >= 1 && rolledNumber <= 6)
        {
            Debug.Log($"Player {currentPlayerIndex + 1} rolled a {rolledNumber}");
            currentState = GameState.ProcessingMove; // Start processing
            HandleDiceRoll(rolledNumber); // Go to handle movement logic
        }
        else
        {
            Debug.LogError($"Invalid dice face number detected: '{diceScript.diceFaceNum}'. Please check SideDetector or Dice naming. Resetting dice.");
            ResetDiceAndWait(); // Reset dice, stay in WaitingForRoll for the same player
        }
    }

    // Determines target square and starts the movement sequence
    void HandleDiceRoll(int rolledNumber)
    {
        int currentSquare = playerPositions[currentPlayerIndex];
        int targetSquare = currentSquare; // Default target is current square

        // --- Start Condition ---
        if (currentSquare == 0) // Player is not yet on the board
        {
            if (rolledNumber == 1 || rolledNumber == 6) // Need 1 or 6 to start
            {
                targetSquare = 1; // Target the first square
                Debug.Log($"Player {currentPlayerIndex + 1} starts!");
            }
            else
            {
                Debug.Log($"Player {currentPlayerIndex + 1} rolled {rolledNumber}, needs 1 or 6 to start.");
                EndTurn(rolledNumber); // Didn't start, end turn immediately
                return; // Exit this method
            }
        }
        // --- Normal Move ---
        else // Player is already on the board
        {
            int targetSquareRaw = currentSquare + rolledNumber;

            // --- Overshoot Logic ---
            if (targetSquareRaw > winningSquare)
            {
                targetSquare = winningSquare - (targetSquareRaw - winningSquare); // Calculate bounce back square
                Debug.Log($"Player {currentPlayerIndex + 1} overshot! Bouncing back from {targetSquareRaw} to {targetSquare}");
            }
            else
            {
                targetSquare = targetSquareRaw; // Normal target square
            }
        }

        // --- Start the Movement Coroutine ---
        // Stop any previous movement just in case
        if (activeMoveCoroutine != null)
        {
            Debug.LogWarning("Stopping an existing move coroutine to start a new one.");
            StopCoroutine(activeMoveCoroutine);
        }
        // Start the new movement sequence
        activeMoveCoroutine = StartCoroutine(MovePieceSequence(currentPlayerIndex, targetSquare, rolledNumber));

        // Note: EndTurn is now called *at the end* of the MovePieceSequence coroutine
    }


    // Coroutine to handle visual movement, animation, logic checks, and turn ending
    private IEnumerator MovePieceSequence(int playerIndex, int finalTargetSquare, int rolledNumber)
    {
        currentState = GameState.MovingPiece; // Set state to moving

        GameObject pieceToMove = playerPieces[playerIndex];
        Animator animator = pieceToMove.GetComponent<Animator>(); // Get animator

        // --- Step 1: Move to the calculated target square ---
        yield return StartCoroutine(AnimateMove(pieceToMove, animator, finalTargetSquare));

        // Update position data AFTER basic move is complete
        playerPositions[playerIndex] = finalTargetSquare;
        Debug.Log($"Player {playerIndex + 1} visually moved to square {finalTargetSquare}");

        // --- Step 2: Check for Win Condition ---
        if (finalTargetSquare == winningSquare)
        {
            Debug.Log($"Player {playerIndex + 1} Wins!");
            currentState = GameState.GameOver;
            UpdateTurnIndicator();
            ResetDice(); // Reset dice visually (make kinematic)
            activeMoveCoroutine = null; // Clear coroutine tracker
            // TODO: Activate Game Over screen/logic here
            yield break; // End the coroutine here, game is over
        }

        // --- Step 3: Check for Special Squares (Snakes/Ladders) --- // TODO: Implement this logic
        // int squareAfterSpecial = CheckSpecialSquares(finalTargetSquare);
        // if (squareAfterSpecial != finalTargetSquare)
        // {
        //     Debug.Log($"Player {playerIndex + 1} landed on special square! Moving to {squareAfterSpecial}");
        //     // Start another move animation to the new square
        //     yield return StartCoroutine(AnimateMove(pieceToMove, animator, squareAfterSpecial));
        //     playerPositions[playerIndex] = squareAfterSpecial; // Update position again
        //     Debug.Log($"Player {playerIndex + 1} visually moved to special square {squareAfterSpecial}");
        //     // Re-check win condition if special square leads to win
        //     if (squareAfterSpecial == winningSquare) { /* ... Game Over Logic ... */ activeMoveCoroutine = null; yield break; }
        // }
        // --- End Special Square Check ---


        // --- Step 4: End the Turn (or Roll Again) ---
        EndTurn(rolledNumber); // Call EndTurn *after* all movement and checks are done

        activeMoveCoroutine = null; // Clear coroutine tracker
    }


    // Coroutine that handles the visual Lerp movement and animation for one step
    private IEnumerator AnimateMove(GameObject piece, Animator anim, int targetSquareNum)
    {
        // --- Get start and end positions ---
        Vector3 startPos = piece.transform.position;
        Vector3 endPos;

        // Ensure targetSquareNum is valid index for boardSquares array
        if (targetSquareNum <= 0 || targetSquareNum > boardSquares.Length || boardSquares[targetSquareNum - 1] == null)
        {
            Debug.LogError($"AnimateMove Error: Invalid target square number {targetSquareNum}");
            // Optionally snap to last known valid position or handle error differently
            if (anim != null) anim.SetFloat("Walk", 0.0f); // Ensure walk stops if erroring out
            yield break; // Stop coroutine if target is invalid
        }
        endPos = boardSquares[targetSquareNum - 1].position;


        // --- Trigger walk animation ---
        // Transition to Walk state when Walk > 0, transition back to Idle when Walk <= 0
        if (anim != null) anim.SetFloat("Walk", 1.0f); // Start walking animation


        // --- Perform Movement ---
        float journeyLength = Vector3.Distance(startPos, endPos);
        if (journeyLength > 0.01f) // Only move if distance is significant
        {
            float duration = journeyLength / moveSpeed; // Calculate time based on distance and speed
            Debug.Log($"AnimateMove START: TargetSquare={targetSquareNum}, Distance={journeyLength}, Speed={moveSpeed}, Calculated Duration={duration}");
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate interpolation factor (ensuring it doesn't exceed 1)
                float fraction = Mathf.Clamp01(elapsedTime / duration);
                // Move piece using Lerp (Linear Interpolation)
                piece.transform.position = Vector3.Lerp(startPos, endPos, fraction);

                elapsedTime += Time.deltaTime;
                yield return null; // Wait for the next frame
            }
        }


        // --- Finalize Position & Animation ---
        // Ensure piece is exactly at the end position to correct potential float inaccuracies
        piece.transform.position = endPos;

        // Trigger idle animation
        if (anim != null) anim.SetFloat("Walk", 0.0f); // Stop walking animation
    }


    // Handles logic after a move is completed - switching turns or allowing another roll
    void EndTurn(int rolledNumber)
    {
        ResetDice(); // Reset the dice state ready for the next roll

        if (currentState == GameState.GameOver) return; // Don't switch turns if someone won

        // Check if player gets another turn for rolling a 6
        if (rolledNumber == 6)
        {
            Debug.Log($"Player {currentPlayerIndex + 1} rolled a 6! Roll again.");
            currentState = GameState.WaitingForRoll; // Stay in waiting state
            waitingForDiceResult = false; // Ready for next roll result
            UpdateTurnIndicator(); // Optional: Update UI to say "Roll again!"
        }
        else
        {
            // Advance to the next player index, wrapping around
            currentPlayerIndex = (currentPlayerIndex + 1) % playerCount;
            Debug.Log($"Turn ended. Next turn: Player {currentPlayerIndex + 1}");
            currentState = GameState.WaitingForRoll; // Set state for next player
            waitingForDiceResult = false; // Ready for next roll result
            UpdateTurnIndicator(); // Update UI to show next player's turn
        }
    }

    // Resets the dice using its Initialize method
    void ResetDice()
    {
        if (diceScript != null)
        {
            diceScript.Initialize(1); // Reset position, make kinematic, reset flags
        }
    }

    // Use this if a roll was invalid, resets dice and keeps turn with current player
    void ResetDiceAndWait()
    {
        ResetDice();
        currentState = GameState.WaitingForRoll;
        waitingForDiceResult = false; // Ready for next input
    }

    // Updates any UI Text showing whose turn it is or the winner
    void UpdateTurnIndicator()
    {
        // --- TODO: Update assigned UI Text element ---
        // Example using standard UI Text:
        // if (turnIndicatorText != null)
        // {
        //     if (currentState == GameState.GameOver) {
        //         turnIndicatorText.text = $"Player {currentPlayerIndex + 1} Wins!";
        //     } else {
        //         turnIndicatorText.text = $"Player {currentPlayerIndex + 1}'s Turn";
        //     }
        // }
    }

    // --- TODO: Implement Special Square Logic ---
    // int CheckSpecialSquares(int landedSquare) {
    //     // Use a Dictionary<int, int> defined elsewhere to map start squares to end squares
    //     // if (specialSquareMap.ContainsKey(landedSquare)) {
    //     //    return specialSquareMap[landedSquare]; // Return the new square number
    //     // }
    //     return landedSquare; // Return original square if not special
    // }
}