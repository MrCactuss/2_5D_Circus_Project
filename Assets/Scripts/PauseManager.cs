using UnityEngine;
using UnityEngine.SceneManagement; // Required for loading scenes (like Main Menu)

public class PauseManager : MonoBehaviour
{
    [Header("Pause Settings")]
    public GameObject pauseMenuPanel; // Assign your PauseMenuPanel GameObject here
    public KeyCode pauseKey = KeyCode.Escape; // Key to toggle pause

    // Tracks whether the game is currently paused
    private bool isPaused = false;

    // Optional: Reference to GameManager to check game state
    // public GameManager gameManager;

    void Start()
    {
        // Ensure the pause menu is hidden at the start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("PauseManager: Pause Menu Panel is not assigned in the Inspector!", this.gameObject);
        }

        // Optional: Find GameManager if not assigned
        // if (gameManager == null) gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        // Check if the pause key is pressed
        // Optional: Add check to prevent pausing if game is already over
        // if (Input.GetKeyDown(pauseKey) && (gameManager == null || gameManager.currentState != GameManager.GameState.GameOver))
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    // Toggles the pause state
    public void TogglePause()
    {
        isPaused = !isPaused; // Flip the paused state

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    // Actions to perform when pausing the game
    void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // Pause game time (affects FixedUpdate, animations, Time.deltaTime)
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true); // Show the pause menu
        }
        // Optional: Unlock cursor if needed
        // Cursor.lockState = CursorLockMode.None;
        // Cursor.visible = true;
        Debug.Log("Game Paused");
    }

    // Actions to perform when resuming the game
    void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // Resume normal game time
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false); // Hide the pause menu
        }
        // Optional: Lock cursor again if needed
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
        Debug.Log("Game Resumed");
    }

    // --- Public Methods for UI Buttons ---

    /// <summary>
    /// Call this method from the Resume Button's OnClick() event.
    /// </summary>
    public void ResumeButton()
    {
        ResumeGame();
    }

    /// <summary>
    /// Call this method from the Quit Button's OnClick() event.
    /// Loads the Main Menu scene (change "MainMenu" to your actual scene name).
    /// </summary>
    public void QuitToMainMenuButton()
    {
        // !!! IMPORTANT: Always reset timeScale before leaving the scene !!!
        Time.timeScale = 1f;
        // Replace "MainMenu" with the actual name of your main menu scene
        SceneManager.LoadScene("MainMenue");
        Debug.Log("Quitting to Main Menu...");
    }

    // Optional: Add methods for Options button, Quit Game button etc.
    // public void OptionsButton() { /* Add logic here */ }
    // public void QuitGameButton() { Application.Quit(); } // Note: Application.Quit() only works in builds, not the editor.
}
