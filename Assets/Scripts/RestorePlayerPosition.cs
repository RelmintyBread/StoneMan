using UnityEngine;

public class PlayerSpawnLoader2D : MonoBehaviour
{
    private void Awake()
    {
        if (PlayerPrefs.HasKey("PlayerX"))
        {
            float x = PlayerPrefs.GetFloat("PlayerX");
            float y = PlayerPrefs.GetFloat("PlayerY");

            Rigidbody2D rb = GetComponent<Rigidbody2D>();

            if (rb != null)
                rb.position = new Vector2(x, y);
            else
                transform.position = new Vector2(x, y);

            Debug.Log("Player position restored");
        }
        Artifact.collectedArtifacts = PlayerPrefs.GetInt("CollectedArtifacts", 0);
    }
}