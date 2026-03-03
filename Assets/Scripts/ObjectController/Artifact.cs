using UnityEngine;

public class Artifact : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject interactUI;
    [SerializeField] private float requiredHoldTime = 3f; // Butuh 1.5 detik untuk mengambil artefak

    private float currentHoldTime = 0f;
    private bool isHolding = false;
    public static int collectedArtifacts = 0;
    public static int totalArtifactsRequired = 5;

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

        HandleUpdateUI();
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

        collectedArtifacts++;

        Debug.Log("Artefak berhasil diekstrak dan masuk inventory! Total: " + collectedArtifacts + "/5");

        // Logika game: tambah skor, play SFX, masuk inventory, dll.

        Destroy(gameObject); // Hancurkan objek artefak di scene
    }

    void HandleUpdateUI()
    {
        UIHandler.Instance.SetArtifactUI(collectedArtifacts, totalArtifactsRequired);
    }
}