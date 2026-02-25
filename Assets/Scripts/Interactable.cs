using UnityEngine;

public interface IInteractable
{
    // Method untuk memulai interaksi, bisa digunakan untuk setup awal
    void StartInteract();
    void StopInteract();
    void Interact();

    // Method untuk menampilkan dan menyembunyikan UI interaksi
    void ShowInteractUI();
    void HideInteractUI();
}
