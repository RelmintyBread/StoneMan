using UnityEngine;

public class PlayerLife : MonoBehaviour
{
    public bool IsDead { get; private set; }

    private Rigidbody2D rb;
    private PlayerMovement2D movement;
    private PlayerInteract interact;
    private FlashlightController flashlight;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movement = GetComponent<PlayerMovement2D>();
        interact = GetComponent<PlayerInteract>();
        flashlight = GetComponent<FlashlightController>();
    }

    public void Die()
    {
        if (IsDead) return;

        IsDead = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (movement != null) movement.enabled = false;
        if (interact != null) interact.enabled = false;
        if (flashlight != null) flashlight.enabled = false;

        UIGameHandler.Instance?.ShowGameOverPanel();
    }
}
