using UnityEngine;

public class MegaFruit : Fruit
{
    public int sliceCount = 0;
    private int baseScore = 10; // Base score for the first slice
    private int maxSlices = 15;

    public static AudioClip powerUpSound;
    private static AudioSource audioSource; // AudioSource component to play sounds


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    protected new void Start()
    {
        PlaySound(powerUpSound);
    }
    

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        base.Slice(direction, position, force);

        IncreaseScorePerSlice();

        sliceCount++;
        if (sliceCount >= maxSlices)
        {
            Destroy(gameObject); // Destroy the MegaFruit game object if max slices reached
        }
    }

    private void IncreaseScorePerSlice()
    {
        int scoreForThisSlice = baseScore * (1 << sliceCount);
        FindObjectOfType<GameManager>().IncreaseScore(scoreForThisSlice);
    }

    private new void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
