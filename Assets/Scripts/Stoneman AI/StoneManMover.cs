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

    // ─────────────────────────────────────────────
    //  PRIVATE STATE
    // ─────────────────────────────────────────────

    private Rigidbody2D rb;
    private bool isFrozen;
    private bool isWaitingForDoor;
    private Vector2 currentDirection; // Arah gerak terakhir, dipakai untuk raycast pintu

    // ─────────────────────────────────────────────
    //  UNITY LIFECYCLE
    // ─────────────────────────────────────────────

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
    }

    public bool IsFrozen => isFrozen;
    public bool IsWaitingForDoor => isWaitingForDoor;

    // ─────────────────────────────────────────────
    //  MOVEMENT
    // ─────────────────────────────────────────────

    void ApplyMovement(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.rotation = angle;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);
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