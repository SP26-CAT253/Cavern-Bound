using UnityEngine;
using UnityEngine.Events;

public class ButtonTrigger : MonoBehaviour
{
    public UnityEvent onPressed; // Drag door function here
    public AudioManager audioManager; // Reference to AudioManager

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onPressed.Invoke();
            Debug.Log("Button Pressed!");
            audioManager.PlayAudio();
            // Optional: Visual feedback for button pressed
        }
    }
}
