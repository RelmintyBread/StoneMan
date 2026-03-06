using UnityEngine;

public class PlayerHide : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private bool isHidden = false; // Status apakah player sedang di dalam
    private HidingObject currentHidingSpot;

    private SpriteRenderer playerSpriteRenderer;
    private PlayerMovement2D playerMovement;
    private FlashlightController flashlightController;

    public bool IsHidden => isHidden;
    public HidingObject CurrentHidingSpot => currentHidingSpot;

    void Awake()
    {
        if (player == null)
        {
            player = gameObject;
        }

        playerSpriteRenderer = player.GetComponent<SpriteRenderer>();
        playerMovement = player.GetComponent<PlayerMovement2D>();
        flashlightController = player.GetComponent<FlashlightController>();
    }

    public bool HidePlayer(HidingObject hidingSpot)
    {
        if (isHidden) return false;
        if (hidingSpot == null) return false;

        isHidden = true;
        currentHidingSpot = hidingSpot;
        Debug.Log("Syuut... Player sembunyi!");

        // Logika sembunyi: Matikan visual dan kontrol utama player.
        if (playerSpriteRenderer != null) playerSpriteRenderer.enabled = false;
        if (playerMovement != null) playerMovement.enabled = false;
        if (flashlightController != null) flashlightController.enabled = false;
        return true;
    }

    public bool UnhidePlayer(HidingObject hidingSpot)
    {
        if (!isHidden) return false;
        if (currentHidingSpot != hidingSpot) return false;

        isHidden = false;
        currentHidingSpot = null;
        Debug.Log("Player keluar dari tempat persembunyian!");

        // Logika keluar: Nyalakan lagi visual dan kontrol utama player.
        if (playerSpriteRenderer != null) playerSpriteRenderer.enabled = true;
        if (playerMovement != null) playerMovement.enabled = true;
        if (flashlightController != null) flashlightController.enabled = true;

        return true;
    }

    public bool TryExitCurrentHidingSpot()
    {
        if (!isHidden || currentHidingSpot == null) return false;

        currentHidingSpot.KeluarPersembunyianDariPlayer();
        return true;
    }

    public bool IsHiding()
    {
        return isHidden;
    }
}
