using System.Collections;
using UnityEngine;

/// <summary>
/// Handles all physics-based movement, rotation, and door interaction for StoneMan.
/// Knows nothing about AI state — it only moves when told to.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class StoneManMover : MonoBehaviour
{
    // ─────────────────────────────────────────────
    //  INSPECTOR FIELDS
    // ─────────────────────────────────────────────

    [Header("Movement")]
    public float moveSpeed = 3f;

    [Header("Door Interaction")]
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private float doorRaycastDistance = 0.8f;
    [SerializeField] private float doorOpenWaitDuration = 0.4f; // Sesuaikan dengan durasi animasi pintu nanti

    [Header("Rotation")]
    [SerializeField] private bool rotateWithMovement = false;

    [Header("Animation Tuning")]
    [SerializeField] private float directionDeadzone = 0.08f;

    [Header("Directional Sprites")]
    [SerializeField] private Sprite idleUp;
    [SerializeField] private Sprite idleDown;
    [SerializeField] private Sprite idleLeft;
    [SerializeField] private Sprite idleRight;

    [SerializeField] private Sprite walkUp1;
    [SerializeField] private Sprite walkUp2;
    [SerializeField] private Sprite walkUp3;

    [SerializeField] private Sprite walkDown1;
    [SerializeField] private Sprite walkDown2;
    [SerializeField] private Sprite walkDown3;

    [SerializeField] private Sprite walkLeft1;
    [SerializeField] private Sprite walkLeft2;
    [SerializeField] private Sprite walkLeft3;

    [SerializeField] private Sprite walkRight1;
    [SerializeField] private Sprite walkRight2;
    [SerializeField] private Sprite walkRight3;

    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Walk Animation")]
    [SerializeField] private float walkFrameRate = 0.15f;

    // ─────────────────────────────────────────────
    //  PRIVATE STATE
    // ─────────────────────────────────────────────

    private Rigidbody2D rb;
    private bool isFrozen;
    private bool isWaitingForDoor;
    private Vector2 currentDirection; // Arah gerak terakhir, dipakai untuk raycast pintu
    private bool isMoveCommanded;
    private Vector2 lastPosition;
    private Vector2 lastMoveDir;
    private float walkTimer;
    private int walkFrame;

    private enum FacingDirection { Up, Down, Left, Right }
    private FacingDirection lastFacing = FacingDirection.Down;

    // ─────────────────────────────────────────────
    //  UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        lastPosition = rb.position;
    }

    void Update()
    {
        UpdateMovementState();
        HandleSprite();
    }

    void FixedUpdate()
    {
        // Akan diset true di MoveTo bila ada perintah gerak pada frame ini.
        isMoveCommanded = false;
    }

    // ─────────────────────────────────────────────
    //  PUBLIC API
    // ─────────────────────────────────────────────

    /// <summary>
    /// Gerakkan Stoneman menuju posisi target satu step (panggil dari FixedUpdate).
    /// Secara otomatis mendeteksi dan membuka pintu yang menghalangi.
    /// </summary>
    public void MoveTo(Vector2 target)
    {
        if (isFrozen || isWaitingForDoor) return;

        Vector2 direction = (target - rb.position).normalized;
        currentDirection = direction;
        isMoveCommanded = true;

        // Cek pintu di depan sebelum bergerak
        if (CheckAndHandleDoor(direction)) return;

        ApplyMovement(direction);
    }

    /// <summary>Freeze / unfreeze movement (misal saat cutscene).</summary>
    public void SetFrozen(bool frozen)
    {
        if (isFrozen == frozen) return;

        isFrozen = frozen;
        if (isFrozen)
            rb.linearVelocity = Vector2.zero;

        if (isFrozen)
        {
            walkTimer = 0f;
            walkFrame = 0;
        }
    }

    public bool IsFrozen => isFrozen;
    public bool IsWaitingForDoor => isWaitingForDoor;

    /// <summary>
    /// Dipanggil setelah teleport agar animasi tidak "kedip" jalan.
    /// </summary>
    public void NotifyTeleported()
    {
        if (rb == null) return;

        lastPosition = rb.position;
        lastMoveDir = Vector2.zero;
        walkTimer = 0f;
        walkFrame = 0;
    }

    // ─────────────────────────────────────────────
    //  MOVEMENT
    // ─────────────────────────────────────────────

    void ApplyMovement(Vector2 direction)
    {
        if (rotateWithMovement)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            rb.rotation = angle;
        }

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
    }

    void UpdateMovementState()
    {
        if (rb == null) return;

        Vector2 currentPos = rb.position;
        lastPosition = currentPos;

        if (isFrozen || isWaitingForDoor || !isMoveCommanded)
        {
            lastMoveDir = Vector2.zero;
            return;
        }

        if (currentDirection.sqrMagnitude > 0.0001f)
        {
            lastMoveDir = currentDirection;
        }
    }

    void HandleSprite()
    {
        if (spriteRenderer == null) return;

        Vector2 dir = lastMoveDir;
        bool isMoving = dir.sqrMagnitude > 0.0001f;

        if (isMoving)
        {
            walkTimer += Time.deltaTime;

            if (walkTimer >= walkFrameRate)
            {
                walkTimer = 0f;
                walkFrame = (walkFrame + 1) % 4;
            }

            float absX = Mathf.Abs(dir.x);
            float absY = Mathf.Abs(dir.y);
            float axisDelta = absX - absY;

            if (axisDelta > directionDeadzone)
            {
                if (dir.x > 0)
                {
                    lastFacing = FacingDirection.Right;
                    spriteRenderer.sprite = GetWalkSprite(walkRight1, walkRight2, walkRight3);
                }
                else
                {
                    lastFacing = FacingDirection.Left;
                    spriteRenderer.sprite = GetWalkSprite(walkLeft1, walkLeft2, walkLeft3);
                }
            }
            else if (-axisDelta > directionDeadzone)
            {
                if (dir.y > 0)
                {
                    lastFacing = FacingDirection.Up;
                    spriteRenderer.sprite = GetWalkSprite(walkUp1, walkUp2, walkUp3);
                }
                else
                {
                    lastFacing = FacingDirection.Down;
                    spriteRenderer.sprite = GetWalkSprite(walkDown1, walkDown2, walkDown3);
                }
            }
            else
            {
                switch (lastFacing)
                {
                    case FacingDirection.Up:
                        spriteRenderer.sprite = GetWalkSprite(walkUp1, walkUp2, walkUp3);
                        break;
                    case FacingDirection.Down:
                        spriteRenderer.sprite = GetWalkSprite(walkDown1, walkDown2, walkDown3);
                        break;
                    case FacingDirection.Left:
                        spriteRenderer.sprite = GetWalkSprite(walkLeft1, walkLeft2, walkLeft3);
                        break;
                    case FacingDirection.Right:
                        spriteRenderer.sprite = GetWalkSprite(walkRight1, walkRight2, walkRight3);
                        break;
                }
            }
        }
        else
        {
            walkTimer = 0f;
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

    Sprite GetWalkSprite(Sprite walk1, Sprite walk2, Sprite walk3)
    {
        switch (walkFrame)
        {
            case 0: return walk1;
            case 1: return walk2;
            case 2: return walk3;
            case 3: return walk2;
        }

        return walk1;
    }

    // ─────────────────────────────────────────────
    //  DOOR HANDLING
    // ─────────────────────────────────────────────

    /// <summary>
    /// Raycast ke arah gerak. Jika kena pintu yang tertutup, buka pintunya
    /// dan tunggu sebentar sebelum Stoneman melanjutkan gerak.
    /// </summary>
    /// <returns>True jika ada pintu yang sedang ditangani (gerak ditahan).</returns>
    bool CheckAndHandleDoor(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(
            rb.position,
            direction,
            doorRaycastDistance,
            doorLayer
        );

        if (hit.collider == null) return false;

        Door door = hit.collider.GetComponent<Door>();
        if (door == null) return false;

        // Hanya interact kalau pintu masih tertutup
        if (!door.IsOpen)
        {
            door.Interact();
            StartCoroutine(WaitForDoorRoutine());
            return true;
        }

        return false; // Pintu sudah terbuka, lanjut jalan
    }

    IEnumerator WaitForDoorRoutine()
    {
        isWaitingForDoor = true;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(doorOpenWaitDuration);

        isWaitingForDoor = false;
    }
}
