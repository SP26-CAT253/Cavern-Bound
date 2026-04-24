using UnityEngine;

public class GateController : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OpenGate()
    {
        animator.SetTrigger("Open");
        // Or disable collider: GetComponent<BoxCollider2D>().enabled = false;
    }
}
