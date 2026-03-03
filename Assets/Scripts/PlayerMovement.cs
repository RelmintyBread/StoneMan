using UnityEngine;

public class PlayerMovement2D : MonoBehaviour
{
    public float walkSpeed = 5f; //Speed ketika berjalan
    public float sprintSpeed = 10f; //speed ketika berlari

    [Header("Stamina Settings")]
    public float maxStamina = 4f; //maks stamina (6 detik)
    [Range(0, 6)] public float currentStamina; //stamina saat ini
    public float staminaDrainRate = 1f; //habis dalam 6 detik
    public float staminaRegenRate = 1f; //regen dalam 6 detik

    private Rigidbody2D rb; //variable yang akan menyatu dengan Rigidbody2D rb;
    private Vector2 moveInput; //variable yang akan menghubungkan input gerakan player 2D
    private PlayerInteract playerInteract;

    private bool isSprinting; //status sprint
    private bool isExhausted; //status exhausted (tidak bisa sprint)
    public float rotationSpeed = 720f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); //menyimpan dalam variable rb 
        playerInteract = GetComponent<PlayerInteract>();
        currentStamina = maxStamina; //set stamina penuh saat mulai
        isExhausted = false;
        HandleUpdateUI();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        if (moveInput.sqrMagnitude > 0f && playerInteract != null)
        {
            playerInteract.SetFacingDirection(moveInput);

            if (moveInput != Vector2.zero)
            {
                float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
                float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime / 100f);
                rb.rotation = smoothAngle;
            }
        }

        //cek sprint hanya jika stamina ada, tidak exhausted, dan player bergerak
        if (Input.GetKey(KeyCode.LeftShift) && currentStamina > 0 && !isExhausted && moveInput.magnitude > 0)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        HandleStamina();
        HandleUpdateUI();
    }

    void FixedUpdate()
    {
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;
        rb.linearVelocity = moveInput * currentSpeed;
    }

    void HandleStamina()
    {
        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;

            if (currentStamina <= 0)
            {
                currentStamina = 0;
                isExhausted = true; //player exhausted
            }
        }
        else
        {
            if (currentStamina < maxStamina)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;

                if (currentStamina >= maxStamina)
                {
                    currentStamina = maxStamina;
                    isExhausted = false; //player can sprint again
                }
            }
        }
    }

    void HandleUpdateUI()
    {
        UIHandler.Instance.SetStaminaUI(currentStamina, maxStamina);
    }
}
