using UnityEngine;

public class HidingObject : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactUI;
    private bool isHidden = false; // Status apakah player sedang di dalam

    public void ShowInteractUI()
    {
        // Hanya munculkan UI "E" jika player belum sembunyi
        if (interactUI != null && !isHidden) interactUI.SetActive(true);
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
        // Toggle (bolak-balik) state sembunyi
        if (!isHidden)
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
        isHidden = true;
        HideInteractUI(); // Sembunyikan prompt "E" agar layar bersih saat ngumpet
        Debug.Log("Syuut... Player sembunyi!");

        // Logika sembunyi: Matikan SpriteRenderer player, disable script movement, ubah tag/layer physics
    }

    private void KeluarPersembunyian()
    {
        isHidden = false;
        ShowInteractUI(); // Munculkan lagi prompt "E" untuk opsi ngumpet lagi nanti
        Debug.Log("Player keluar dari tempat persembunyian!");

        // Logika keluar: Nyalakan lagi SpriteRenderer player, enable script movement
    }
}