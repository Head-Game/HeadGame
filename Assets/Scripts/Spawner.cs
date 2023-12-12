using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)] public float bombChance = 0.05f;

    public float minSpawnDelay = 0.25f;
    public float maxSpawnDelay = 1f;

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float maxLifetime = 5f;

    public float spawnRadius = 5f; // Public property to easily adjust the spawn radius

    private void Start()
    {
        // Initialize the spawn sounds for the fruits
        Fruit.spawnClips = new AudioClip[]
        {
            Resources.Load<AudioClip>("temp1"),
            Resources.Load<AudioClip>("temp2"),
            Resources.Load<AudioClip>("temp3"),
            Resources.Load<AudioClip>("temp4")
        };

        // Initialize the slice sound for the fruits
        Fruit.sliceClip = Resources.Load<AudioClip>("example_sound");

        StartCoroutine(Spawn());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Spawn()
    {
        yield return new WaitForSeconds(2f);

        while (enabled)
        {
            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];

            if (Random.value < bombChance)
            {
                prefab = bombPrefab;
            }

            // Generate a random position on a circle in the XZ plane
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(angle) * spawnRadius, transform.position.y, Mathf.Sin(angle) * spawnRadius);

            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Destroy(fruit, maxLifetime);

            float force = Random.Range(minForce, maxForce);
            fruit.GetComponent<Rigidbody>().AddForce(Vector3.up * force, ForceMode.Impulse);

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
        }
    }
}
