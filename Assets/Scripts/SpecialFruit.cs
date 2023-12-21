using UnityEngine;

public class SpecialFruit : Fruit
{
    public GameObject[] fruitPrefabs; // Use the same fruit prefabs array as the Spawner
    public int spawnFruitCount = 3; // Number of fruits to spawn
    public static AudioClip fruitSpawnSound; // Sound when new fruits spawn

    public static AudioClip powerUpSound;
    private static AudioSource audioSource;

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

        SpawnFruits(position, force);

        Destroy(gameObject);
    }

    private void SpawnFruits(Vector3 position, float force)
    {
        for (int i = 0; i < spawnFruitCount; i++)
        {
            if (fruitPrefabs.Length > 0)
            {
                GameObject fruitPrefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
                GameObject fruit = Instantiate(fruitPrefab, position, Quaternion.identity);
                Rigidbody fruitRb = fruit.GetComponent<Rigidbody>();

                if (fruitRb != null)
                {
                    Vector3 randomDirection = Random.insideUnitSphere.normalized;
                    randomDirection.y = Mathf.Abs(randomDirection.y);
                    fruitRb.AddForce(randomDirection * force, ForceMode.Impulse);
                }

                PlaySound(fruitSpawnSound);
            }
            else
            {
                Debug.LogWarning("Fruit prefabs array is empty. Assign prefabs in the inspector.");
            }
        }
    }

    private new void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
