using UnityEngine;

public class Bomb : MonoBehaviour
{
    public static AudioClip tickingClip;
    public static AudioClip explosionClip;
    public static float volumeMultiplier = 50f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure audio source for 3D sound
        audioSource.spatialBlend = 1.0f;
        audioSource.rolloffMode = AudioRolloffMode.Logarithmic;
        audioSource.minDistance = 1.0f;
        audioSource.maxDistance = 50.0f;
        Debug.Log("Volume multiplier is set to: " + volumeMultiplier);
        audioSource.volume *= volumeMultiplier;
        Debug.Log("Volume for " + gameObject.name + " after applying multiplier is: " + audioSource.volume);
    }

    private void Start()
    {
        PlaySound(tickingClip);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;
            PlaySound(explosionClip);
            // Get the GameManager instance and call RestartGame
            GameManager gameManager = FindObjectOfType<GameManager>();
            if (gameManager != null)
            {
                gameManager.Explode();
                gameManager.OnGameOver();
            }
        }
    }
}
