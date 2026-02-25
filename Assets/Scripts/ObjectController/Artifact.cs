using UnityEngine;

public class Artifact : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactUI;
    [SerializeField] private float requiredHoldTime = 1.5f; // Butuh 1.5 detik untuk mengambil artefak

    private float currentHoldTime = 0f;
    private bool isHolding = false;

    void Update()
    {
        // Hanya menghitung waktu jika tombol sedang ditahan
        if (isHolding)
        {
            currentHoldTime += Time.deltaTime;

            // Jika waktu hold sudah tercapai, eksekusi pengambilan
            if (currentHoldTime >= requiredHoldTime)
            {
                Interact();
            }
        }
    }

    public void ShowInteractUI()
    {
        if (interactUI != null) interactUI.SetActive(true);
    }

    public void HideInteractUI()
    {
        if (interactUI != null) interactUI.SetActive(false);
    }

    public void StartInteract()
    {
        // Mulai proses ekstraksi saat tombol ditekan
        isHolding = true;
        currentHoldTime = 0f;
    }

    public void StopInteract()
    {
        // Batalkan proses dan reset waktu jika tombol dilepas sebelum selesai
        isHolding = false;
        currentHoldTime = 0f;
    }

    public void Interact()
    {
        isHolding = false; // Hentikan perhitungan
        Debug.Log("Artefak berhasil diekstrak dan masuk inventory!");

        // Logika game: tambah skor, play SFX, masuk inventory, dll.

        Destroy(gameObject); // Hancurkan objek artefak di scene
    }
}