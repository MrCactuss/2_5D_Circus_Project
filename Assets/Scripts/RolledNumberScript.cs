using UnityEngine;
using UnityEngine.UI; // Or change to using TMPro; if you use TextMeshPro
using System.Collections; 

public class RolledNumberScript : MonoBehaviour
{
    DiceRollScript diceRollScript;

    public Text rolledNumberText; // If using standard UI Text
    // OR
    // public TMPro.TextMeshProUGUI rolledNumberText; // If using TextMeshPro Text (requires adding 'using TMPro;' above)

    void Awake()
    {
        // Try to find the script immediately
        FindDiceScript();

        // Optional: If it might not be ready in Awake, try again after a short delay
        // if (diceRollScript == null)
        // {
        //     StartCoroutine(DelayedFindDiceScript());
        // }
    }

    void FindDiceScript()
    {
        diceRollScript = FindObjectOfType<DiceRollScript>();
        if (diceRollScript == null)
        {
            Debug.LogError("RolledNumberScript [Awake]: FAILED to find DiceRollScript!", this.gameObject);
        }
        else
        {
            Debug.Log("RolledNumberScript [Awake]: Successfully found DiceRollScript.", this.gameObject);
        }
    }

    // IEnumerator DelayedFindDiceScript()
    // {
    //      yield return new WaitForSeconds(0.1f); // Wait a fraction of a second
    //      FindDiceScript();
    //      if(diceRollScript == null) Debug.LogError("RolledNumberScript [DelayedFind]: Still couldn't find DiceRollScript!");
    // }


    // Inside RolledNumberScript.cs
    void Update()
    {
        // --- Check 1: Is the Text field assigned? ---
        if (rolledNumberText == null)
        {
            // Only log error once to avoid spam
            if (!gameObject.GetComponent<ErrorLogger>()?.loggedTextNull ?? true)
            {
                Debug.LogError("RolledNumberScript [Update]: rolledNumberText field is NOT assigned in the Inspector!", this.gameObject);
                gameObject.AddComponent<ErrorLogger>().loggedTextNull = true; // Add temporary component to track logging
            }
            return;
        }

        // --- Check 2: Is the DiceRollScript reference valid? ---
        if (diceRollScript == null)
        {
            if (rolledNumberText.text != "ERR") // Prevent spamming text set
            {
                Debug.LogError("RolledNumberScript [Update]: DiceRollScript reference is NULL!", this.gameObject);
                rolledNumberText.text = "ERR";
            }
            return;
        }

        // --- Check 3: Get the current face number string ---
        string currentFace = diceRollScript.diceFaceNum;

        // --- Determine what to display ---
        // Check if the string contains a valid number (1-6)
        bool isValidResult = !string.IsNullOrEmpty(currentFace) &&
                             int.TryParse(currentFace, out int num) &&
                             num >= 1 && num <= 6;

        if (isValidResult)
        {
            // It's a valid number (1-6), display it
            if (rolledNumberText.text != currentFace) // Only update if needed
            {
                // You can uncomment this log for debugging if needed
                // Debug.Log($"RolledNumberScript: Displaying stored face '{currentFace}'");
                rolledNumberText.text = currentFace;
            }
        }
        else
        {
            // It's not a valid number (likely empty because rolling or not set yet), display "?"
            if (rolledNumberText.text != "?") // Only update if needed
            {
                // You can uncomment this log for debugging if needed
                // Debug.Log("RolledNumberScript: Displaying '?' (face num is empty or invalid)");
                rolledNumberText.text = "?";
            }
        }
    }

    // Helper component to prevent spamming the same error log every frame
    private class ErrorLogger : MonoBehaviour { public bool loggedTextNull = false; }
}