using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerMovement : MonoBehaviour
{
    private GameManagerScript gameManager; // Helps communicate with Game Manager script for pause menu/gameover screen
    public float speed = 5.0f; //How fast the player character is at normal movement
    public float runMultiplier = 1.75f; // How much faster running is
    public float jumpForce = 10.0f;
    public int maxJumps = 2; // Public variable to set the total number of jumps allowed
    public float groundPoundForce = 25f;

    // Added for pickup functionality (assign in Inspector)
    public CoinManager coinManager;
    public AudioManager audioManager;
    public GameObject pickupCollectVFX; // assign prefab in Inspector

    private float moveDirection;
    private Rigidbody2D rb;
    private bool isGrounded;
    private int availableJumps; // Private variable to track jumps remaining
    private bool isGroundPounding;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        availableJumps = maxJumps; // Initialize available jumps on start

        gameManager = FindFirstObjectByType<GameManagerScript>();
    }


    void Update()
    {
        if (Time.timeScale == 0f) return;

        if (transform.position.y < -10f)
        {
            gameManager.GameOver();
            enabled = false; // disable player movement
        }
        Gamepad currentGamepad = InputSystem.devices.OfType<Gamepad>().FirstOrDefault();

        moveDirection = GetHorizontalInput(currentGamepad);

        // Deadzone clamp
        if (Mathf.Abs(moveDirection) < 0.1f) moveDirection = 0f;

        float currentSpeed = speed;

        if (IsRunHeld(currentGamepad))
        {
            currentSpeed *= runMultiplier;
        }

        if (!isGroundPounding)
        {
            rb.linearVelocity = new Vector2(moveDirection * currentSpeed, rb.linearVelocity.y);
        }

        //Before your normal jump check, include Ground Pound (only in air:)
        if (!isGrounded && !isGroundPounding && IsGroundPoundPressed(currentGamepad))
        {
            GroundPound();
            return; // Stop other movement this frame
        }

        // Check for jump input. The condition now checks if we have jumps available
        if (IsJumpPressed(currentGamepad) && availableJumps > 0)
        {
            Jump();
        }
    }

    private float GetHorizontalInput(Gamepad currentGamepad)
    {
        float gamepadInput = 0f;
        float keyboardInput = 0f;

        // Gamepad
        if (currentGamepad != null)
        {
            gamepadInput = currentGamepad.leftStick.x.ReadValue();
        }

        // Keyboard (A/D + Left/Right Arrows)

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) keyboardInput -= 1f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) keyboardInput += 1f;
        }

        // Prefer gamepad if active
        if (Mathf.Abs(gamepadInput) > 0.1f)
            return gamepadInput;

        return keyboardInput;
    }

    private void Jump()
    {
        // When jumping, we reset the vertical velocity before applying force
        // This ensures the second jump always has the same force, regardless of gravity
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
        rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);

        availableJumps--; // Decrease the number of available jumps
    }

    private void GroundPound()
    {
        isGroundPounding = true;

        // Cancel current motion
        rb.linearVelocity = Vector2.zero;

        // Slam downward
        rb.AddForce(Vector2.down * groundPoundForce, ForceMode2D.Impulse);
    }

    private bool IsJumpPressed(Gamepad currentGamepad)
    {
        bool gamepadJump = currentGamepad != null && currentGamepad.aButton.wasPressedThisFrame;
        bool keyboardJump = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        return gamepadJump || keyboardJump;
    }

    private bool IsGroundPoundPressed(Gamepad currentGamepad)
    {
        bool gamepadPound =
            currentGamepad != null && currentGamepad.leftStick.y.ReadValue() < -0.5f && currentGamepad.aButton.wasPressedThisFrame;

        bool keyboardPound =
            Keyboard.current != null && (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) && Keyboard.current.spaceKey.wasPressedThisFrame;

        return gamepadPound || keyboardPound;
    }

    private bool IsRunHeld(Gamepad currentGamepad)
    {
        bool gamepadRun =
            currentGamepad != null && (currentGamepad.leftStickButton.isPressed || currentGamepad.rightTrigger.ReadValue() > 0.1f);

        bool keyboardRun =
            Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

        return gamepadRun || keyboardRun;
    }
    // New Unity function to detect collisions with the ground
    void OnCollisionEnter2D(Collision2D collision)
    {
        // When the player lands on the ground, reset the available jumps
        // to the maximum allowed value.
        if (collision.gameObject.CompareTag("Ground")) // Ensure your ground object has the "Ground" tag
        {
            // The following "if" statement gives the Ground Pound impact some bounce to it
            if (isGroundPounding)
            {
                rb.AddForce(Vector2.up * 2f, ForceMode2D.Impulse);
            }

            availableJumps = maxJumps;
            isGrounded = true; // Retained for ground detection logic, though less critical now
            isGroundPounding = false; // Reset ground pound
        }
    }

    // Optional: Add OnCollisionExit2D to update isGrounded status when leaving the ground
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    // Coin / pickup handling copied/adapted from old script
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("CoinCollection"))
        {
            // Play coin sound using a temporary audio source so destroying the coin won't stop it.
            if (audioManager != null && audioManager.audioClip != null)
            {
                // Play at camera position to behave like 2D UI sound (full volume, no 3D attenuation).
                Vector3 playPos = Camera.main != null ? Camera.main.transform.position : transform.position;
                AudioSource.PlayClipAtPoint(audioManager.audioClip, playPos);
            }
            else if (audioManager != null)
            {
                // Fallback: attempt to call PlayAudio (may be on a persistent AudioManager)
                audioManager.PlayAudio();
            }

            if (pickupCollectVFX != null)
            {
                Instantiate(pickupCollectVFX, transform.position, Quaternion.identity);
            }

            Destroy(other.gameObject);

            if (coinManager != null)
            {
                coinManager.coinCount++;
            }
        }
    }
}


// ***************  THIS IS THE END OF THE CODE  ************************


// ***************  KEY TERMS  ************************
//  variable
//  inspector
//  declaring
//  initializing
//  public
//  private
//  debug.log
//  string
//  float
//  integer (aka 'int')
//  GameObject
//  Input
//  KeyCode
//  string
//  Rigidbody2D
//  Vector2
//  Vector3
//  ||
//  &&
//  ++
//  *
//  ==
//  =
// !=






// ***************  IGNORE EVERYTHING BELOW THIS LINE!  ************************