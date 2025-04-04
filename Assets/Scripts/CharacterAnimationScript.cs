using UnityEngine;

[RequireComponent(typeof(Animator))] // Makes sure an Animator component exists
public class CharacterAnimationScript : MonoBehaviour
{
    private Animator animator;

    // --- Variables for controlling animation ---

    [Tooltip("Set this value based on your character's movement speed.")]
    public float currentSpeed = 0f;

    [Tooltip("For testing: Press this key to trigger the Hurt animation.")]
    public KeyCode hurtKey = KeyCode.H; // Key to simulate taking damage

    // --- Animator Parameter Hashes (Optimization, Optional) ---
    private readonly int speedHash = Animator.StringToHash("Walk");
    private readonly int hurtHash = Animator.StringToHash("Hurt");


    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();

        // Check if the Animator component was found
        if (animator == null)
        {
            Debug.LogError("CharacterAnimationScript: Animator component not found on this GameObject!", this);
            this.enabled = false; // Disable script if Animator is missing
        }
    }


    void Update()
    {
        // Exit early if the Animator component is missing
        if (animator == null) return;

        // --- Update Animator Parameters ---

        // 1. Set the Speed parameter
        animator.SetFloat(speedHash, currentSpeed); // Using hash is slightly faster

        // 2. Check for Hurt trigger 
        if (Input.GetKeyDown(hurtKey))
        {
            TriggerHurtAnimation();
        }
    }

    /// <summary>
    /// </summary>
    public void TriggerHurtAnimation()
    {
        if (animator != null)
        {
            Debug.Log("Hurt animation triggered!");
            // Set the Hurt trigger in the Animator Controller
            animator.SetTrigger(hurtHash); // Using hash is slightly faster
        }
    }

    // Update currentSpeed from another script ---
    /*
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
    */
}