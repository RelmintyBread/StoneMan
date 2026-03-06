using UnityEngine;

public class HidingObject : MonoBehaviour, IInteractable
{
    [SerializeField] public PlayerHide playerHide; // Referensi ke script PlayerHide untuk memanggil fungsi sembunyi

    public void ShowInteractUI()
    {
        if (playerHide == null) return;

        // Saat player sedang ngumpet di object ini, prompt tetap disembunyikan.
        bool hiddenInThisObject = playerHide.IsHidden && playerHide.CurrentHidingSpot == this;
        if (hiddenInThisObject)
        {
            UIGameHandler.Instance?.HideInteractPrompt();
        }
        else
        {
            UIGameHandler.Instance?.ShowInteractPrompt();
        }
    }

    public void HideInteractUI()
    {
        UIGameHandler.Instance?.HideInteractPrompt();
    }

    public void StartInteract()
    {
        // Karena ini press/instant, langsung panggil eksekusi
        Interact();
    }

    public void StopInteract()
    {
        // Dibiarkan kosong karena interaksi instan tidak peduli tombol dilepas
    }

    public void Interact()
    {
        if (playerHide == null) return;

        // Toggle (bolak-balik) state sembunyi, tapi state tunggal ada di PlayerHide.
        bool isHiddenInThisObject = playerHide.IsHidden && playerHide.CurrentHidingSpot == this;
        if (!isHiddenInThisObject)
        {
            MasukPersembunyian();
        }
        else
        {
            KeluarPersembunyian();
        }
    }

    private void MasukPersembunyian()
    {
        if (playerHide == null) return;
        if (!playerHide.HidePlayer(this)) return;

        HideInteractUI(); // Sembunyikan prompt "E" agar layar bersih saat ngumpet
        Debug.Log("Syuut... Player sembunyi!");
    }

    private void KeluarPersembunyian()
    {
        if (playerHide == null) return;
        if (!playerHide.UnhidePlayer(this)) return;

        ShowInteractUI(); // Munculkan lagi prompt "E" untuk opsi ngumpet lagi nanti
        Debug.Log("Player keluar dari tempat persembunyian!");
    }

    public void KeluarPersembunyianDariPlayer()
    {
        KeluarPersembunyian();
    }
}
