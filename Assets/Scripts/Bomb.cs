using UnityEngine;

public class Bomb : MonoBehaviour
{
    public static AudioClip tickingClip;
    public static AudioClip explosionClip;

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
            FindObjectOfType<GameManager>().Explode();

            // Find and call ResetGame on the Spawner
            Spawner spawner = FindObjectOfType<Spawner>();
            if (spawner != null)
            {
                spawner.ResetGame();
            }
        }
    }
}
