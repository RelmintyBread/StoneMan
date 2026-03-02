using UnityEngine;

public class HidingObject : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactUI;
    [SerializeField] private PlayerHide playerHide; // Referensi ke script PlayerHide untuk memanggil fungsi sembunyi

    public void ShowInteractUI()
    {
        if (interactUI == null) return;
        if (playerHide == null) return;

        // Saat player sedang ngumpet di object ini, prompt tetap disembunyikan.
        bool hiddenInThisObject = playerHide.IsHidden && playerHide.CurrentHidingSpot == this;
        interactUI.SetActive(!hiddenInThisObject);
    }

    public void HideInteractUI()
    {
        if (interactUI != null) interactUI.SetActive(false);
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
