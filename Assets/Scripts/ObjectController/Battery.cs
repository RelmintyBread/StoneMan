using UnityEngine;

public class Battery : MonoBehaviour, IInteractable
{
    [SerializeField] private int batteryAmount = 20; // Jumlah baterai yang diberikan saat diambil
    [SerializeField] private float requiredHoldTime = 1.5f; // Waktu yang dibutuhkan untuk mengambil baterai, bisa diatur sesuai kebutuhan
    [SerializeField] private FlashlightController flashlightController;

    private float currentHoldTime = 0f; // Waktu saat ini yang telah dipegang
    private bool isHolding = false; // Apakah tombol interaksi sedang ditekan


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
        UIHandler.Instance?.ShowInteractPrompt();
    }

    public void HideInteractUI()
    {
        UIHandler.Instance?.HideInteractPrompt();
    }

    public void StartInteract()
    {
        isHolding = true;
        currentHoldTime = 0f; // Reset waktu hold saat mulai interaksi
    }

    public void StopInteract()
    {
        // Tidak perlu aksi saat tombol dilepas untuk interaksi instan
        isHolding = false;
        currentHoldTime = 0f; // Reset waktu hold saat interaksi dihentikan
    }

    public void Interact()
    {
        Debug.Log("Player mengambil baterai!");
        // Logika untuk menambahkan baterai ke inventory player atau mengaktifkan sesuatu
        flashlightController.RechargeBattery(batteryAmount); // Contoh: Menambahkan baterai ke flashlight controller
        Destroy(gameObject); // Hapus objek baterai setelah diambil
    }
}
