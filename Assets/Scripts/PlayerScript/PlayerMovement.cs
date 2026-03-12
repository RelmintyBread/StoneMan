using UnityEngine;

public class PlayerMovement2D : MonoBehaviour, ISaveable
{
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;

    [Header("Stamina Settings")]
    public float maxStamina = 4f;
    [Range(0, 6)] public float currentStamina;
    public float staminaDrainRate = 1f;
    public float staminaRegenRate = 1f;

    [Header("Directional Sprites")]
    [SerializeField] private Sprite idleUp;
    [SerializeField] private Sprite idleDown;
    [SerializeField] private Sprite idleLeft;
    [SerializeField] private Sprite idleRight;

    [SerializeField] private Sprite walkUp1;
    [SerializeField] private Sprite walkUp2;

    [SerializeField] private Sprite walkDown1;
    [SerializeField] private Sprite walkDown2;

    [SerializeField] private Sprite walkLeft1;
    [SerializeField] private Sprite walkLeft2;

    [SerializeField] private Sprite walkRight1;
    [SerializeField] private Sprite walkRight2;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Walk Animation")]
    [SerializeField] private float walkFrameRate = 0.15f;

    private float walkTimer;
    private int walkFrame;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private PlayerInteract playerInteract;

    private bool isSprinting;
    private bool isExhausted;
    private bool hasLoadedData;

    private enum FacingDirection { Up, Down, Left, Right }
    private FacingDirection lastFacing = FacingDirection.Down;

    public float rotationSpeed = 720f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInteract = GetComponent<PlayerInteract>();

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        SaveManager.RegisterSaveable(this);
    }

    void Start()
    {
        if (!hasLoadedData)
        {
            currentStamina = maxStamina;
        }

        isExhausted = false;
        HandleUpdateUI();
    }

    void Update()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");
        moveInput = moveInput.normalized;

        GuideManager.Instance?.NotifyMovementInput(moveInput);

        if (moveInput.sqrMagnitude > 0f && playerInteract != null)
        {
            playerInteract.SetFacingDirection(moveInput);

            float targetAngle = Mathf.Atan2(moveInput.y, moveInput.x) * Mathf.Rad2Deg;
            float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.deltaTime / 100f);
            rb.rotation = smoothAngle;
        }

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
        HandleSprite();
        HandleFootstepSFX();
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
                isExhausted = true;
                AudioManager.Instance?.PlaySFX(AudioManager.Instance.sfxExhausted);
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
                }

                if (currentStamina > (0.2 * maxStamina) && isExhausted)
                {
                    isExhausted = false;
                }
            }
        }
    }

    void HandleUpdateUI()
    {
        if (UIGameHandler.Instance != null)
        {
            UIGameHandler.Instance.SetStaminaUI(currentStamina, maxStamina);
        }
    }

    void HandleSprite()
    {
        if (spriteRenderer == null) return;

        if (moveInput.sqrMagnitude > 0)
        {
            walkTimer += Time.deltaTime;

            if (walkTimer >= walkFrameRate)
            {
                walkTimer = 0f;
                walkFrame = (walkFrame + 1) % 4;
            }

            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                if (moveInput.x > 0)
                {
                    lastFacing = FacingDirection.Right;
                    spriteRenderer.sprite = GetWalkSprite(walkRight1, walkRight2, idleRight);
                }
                else
                {
                    lastFacing = FacingDirection.Left;
                    spriteRenderer.sprite = GetWalkSprite(walkLeft1, walkLeft2, idleLeft);
                }
            }
            else
            {
                if (moveInput.y > 0)
                {
                    lastFacing = FacingDirection.Up;
                    spriteRenderer.sprite = GetWalkSprite(walkUp1, walkUp2, idleUp);
                }
                else
                {
                    lastFacing = FacingDirection.Down;
                    spriteRenderer.sprite = GetWalkSprite(walkDown1, walkDown2, idleDown);
                }
            }
        }
        else
        {
            walkTimer = 0;
            walkFrame = 0;

            switch (lastFacing)
            {
                case FacingDirection.Up:
                    spriteRenderer.sprite = idleUp;
                    break;
                case FacingDirection.Down:
                    spriteRenderer.sprite = idleDown;
                    break;
                case FacingDirection.Left:
                    spriteRenderer.sprite = idleLeft;
                    break;
                case FacingDirection.Right:
                    spriteRenderer.sprite = idleRight;
                    break;
            }
        }
    }

    void HandleFootstepSFX()
    {
        if (AudioManager.Instance == null) return;

        bool isMoving = moveInput.sqrMagnitude > 0.01f;
        if (isMoving)
        {
            if (AudioManager.Instance.sfxFootStep != null)
            {
                AudioManager.Instance.PlayFootstep(AudioManager.Instance.sfxFootStep);
            }
            return;
        }

        AudioManager.Instance.StopFootstep();
    }

    Sprite GetWalkSprite(Sprite walk1, Sprite walk2, Sprite idle)
    {
        switch (walkFrame)
        {
            case 0: return walk1;
            case 1: return idle;
            case 2: return walk2;
            case 3: return idle;
        }

        return idle;
    }

    public void OnSave(SaveData data)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;
        }

        data.playerPosition = rb.position;
        data.playerStamina = currentStamina;
    }

    public void OnLoad(SaveData data)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;
        }

        rb.position = data.playerPosition;
        currentStamina = data.playerStamina;
        hasLoadedData = true;

        HandleUpdateUI();
    }

    void OnDisable()
    {
        AudioManager.Instance?.StopFootstep();
    }

}
