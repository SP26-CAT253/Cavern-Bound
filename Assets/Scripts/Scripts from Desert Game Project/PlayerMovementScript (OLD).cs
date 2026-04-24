using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementScript : MonoBehaviour
{
    private Rigidbody2D rb;
    public Animator anim;
    private float moveSpeed;
    private float dirX;
    private bool facingRight = true;
    private Vector3 localScale;

    public CoinManager cm;
    public AudioManager audioManager;
    public GameObject pickupCollectVFX; // assign prefab in Inspector

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        localScale = transform.localScale;
        moveSpeed = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        dirX = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetButtonDown("Jump") && rb.linearVelocity.y == 0)
        {
            rb.AddForce(Vector2.up * 1000f);
        }


        //Animation Code
        if (Mathf.Abs(dirX) > 0 && rb.linearVelocity.y == 0)
        {
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
        if (rb.linearVelocity.y == 0)
        {
            anim.SetBool("isJumping", false);
            //anim.SetBool("isFalling", false);    <----If you want to use this, get a sprite sheet of the player falling
        }
        if (rb.linearVelocity.y > 0)
        {
            anim.SetBool("isJumping", true);
        }
        if (rb.linearVelocity.y < 0)
        {
            anim.SetBool("isJumping", false);
            //anim.SetBool("isFalling", true);    <----If you want to use this, get a sprite sheet of the player falling
        }

        // ... (other animation conditions)

        // End of animation code

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
        }


    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(dirX, rb.linearVelocity.y);
    }

    void LateUpdate()
    {
        if (dirX > 0)
        {
            facingRight = true;
            //transform.rotation = Quaternion.Euler(0, 0, 0); //mod
        }
        else if (dirX < 0)
        {
            facingRight = false;
            //transform.rotation = Quaternion.Euler(0, -180, 0); //mod
        }

        if (((facingRight) && (localScale.x < 0)) || ((!facingRight) && (localScale.x > 0)))
        {
            localScale.x *= -1;
        }

        transform.localScale = localScale;
    }

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

            Instantiate(pickupCollectVFX, transform.position, Quaternion.identity);

            Destroy(other.gameObject);
            cm.coinCount++;
        }
    }
}
