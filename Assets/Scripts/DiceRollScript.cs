using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceRollScript : MonoBehaviour
{
    Rigidbody rBody;
    Vector3 position; // Stores initial position for reset
    [SerializeField] private float maxRadForceVal = 500f; // Example default
    [SerializeField] private float startRollingForce = 1200f; // Example default
    float forceX, forceY, forceZ;

    // Public state variables read by other scripts
    public string diceFaceNum; // Set by SideDetectorScript
    public bool isLanded = false; // Set by SideDetectorScript, reset here

    // Internal state
    private bool firstThrow = false;


    void Awake()
    {
        Initialize(0); // Initialize fully on awake
    }

    void Update()
    {
        // Only allow clicking if the dice isn't kinematic (i.e. mid-roll) AND has landed OR it's the first ever throw
        if (rBody != null) // Add null check
                           // Check if mouse button is clicked AND (dice has landed OR it's the first throw)
            if (Input.GetMouseButtonDown(0) && (isLanded || !firstThrow)) // Use GetMouseButtonDown for single click detection
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                    {
                        // Only roll if kinematic (prevents re-rolling mid-air)
                        if (rBody.isKinematic)
                        {
                            if (!firstThrow)
                                firstThrow = true;

                            RollDice();
                        }
                    }
                }
            }
    }

    // Resets the dice state. node=0 for full init, node=1 for reset position/state
    public void Initialize(int node)
    {
        if (node == 0 && rBody == null) // Get Rigidbody only once if needed
        {
            rBody = GetComponent<Rigidbody>();
            if (rBody == null) { Debug.LogError("DiceRollScript requires a Rigidbody component!"); return; }
            position = transform.position; // Store initial position only once
        }
        else if (node == 1) // Reset position
        {
            if (rBody != null) transform.position = position;
        }

        // Common reset logic for both nodes
        firstThrow = (node == 0) ? false : firstThrow; // Reset firstThrow only on full init? Or always? Let's reset always for safety on Initialize(1).
        firstThrow = false;
        isLanded = false; // *** CRUCIAL: Ensure landed flag is reset ***

        if (rBody != null)
        {
            rBody.isKinematic = true; // Stop physics
            // rBody.velocity = Vector3.zero; // Reset velocity/angular velocity
            // rBody.angularVelocity = Vector3.zero;
            // Use Random.rotation for a proper uniform random rotation
            transform.rotation = Random.rotation;
        }
    }

    // Applies physics forces to roll the dice
    private void RollDice()
    {
        if (rBody == null) return;

        // --- ADD THESE TWO LINES ---
        isLanded = false; // Ensure flag is false when rolling
        diceFaceNum = ""; // Clear the previous result string

        rBody.isKinematic = false; // Enable physics

        // Random torque values
        forceX = Random.Range(-maxRadForceVal, maxRadForceVal); // Allow negative torque too
        forceY = Random.Range(-maxRadForceVal, maxRadForceVal);
        forceZ = Random.Range(-maxRadForceVal, maxRadForceVal);

        // Apply forces
        rBody.AddForce(Vector3.up * Random.Range(10, startRollingForce), ForceMode.Impulse); // Impulse for sudden force
        rBody.AddTorque(forceX, forceY, forceZ, ForceMode.Impulse);

        Debug.Log("Rolling Dice!");
    }
}
