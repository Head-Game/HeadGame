using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    [Range(0f, 1f)] public float bombChance = 0.05f;

    public float initialMinSpawnDelay = 1f;   // Higher initial spawn delay
    public float initialMaxSpawnDelay = 2f;   // Higher initial spawn delay
    public float finalMinSpawnDelay = 0.25f;  // Lower final spawn delay
    public float finalMaxSpawnDelay = 1f;     // Lower final spawn delay

    public float minAngle = -15f;
    public float maxAngle = 15f;

    public float minForce = 18f;
    public float maxForce = 22f;

    public float minGravity = 0.5f;
    public float maxGravity = 2f;

    public float maxLifetime = 5f;

    public float spawnRadius = 5f;

    public Text levelText;

    private int level = 1;
    private float levelUpTime = 30f;
    private float nextLevelUp;

    private void Start()
    {
        // Set up audio clips
        Fruit.spawnClips = new AudioClip[]
        {
            Resources.Load<AudioClip>("temp1"),
            Resources.Load<AudioClip>("temp2"),
            Resources.Load<AudioClip>("temp3"),
            Resources.Load<AudioClip>("temp4")
        };
        Fruit.sliceClip = Resources.Load<AudioClip>("example_sound");
        Bomb.explosionClip = Resources.Load<AudioClip>("gameover");
        Bomb.tickingClip = Resources.Load<AudioClip>("tick");

        StartCoroutine(Spawn());
        nextLevelUp = Time.time + levelUpTime;
        UpdateLevelText();
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
            if (Time.time >= nextLevelUp)
            {
                LevelUp();
            }

            GameObject prefab = fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
            if (Random.value < bombChance) prefab = bombPrefab;

            Vector3 position = GenerateSpawnPosition();
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Rigidbody fruitRb = fruit.GetComponent<Rigidbody>();

            float force = Mathf.Lerp(minForce, maxForce, level / 10f);
            fruitRb.AddForce(Vector3.up * force, ForceMode.Impulse);

            float gravityScale = Mathf.Lerp(maxGravity, minGravity, 1f - (level / 10f));
            fruitRb.mass = gravityScale;

            Destroy(fruit, maxLifetime);

            float spawnDelay = Mathf.Lerp(initialMaxSpawnDelay, finalMinSpawnDelay, level / 10f);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private Vector3 GenerateSpawnPosition()
    {
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(angle) * spawnRadius, transform.position.y, Mathf.Sin(angle) * spawnRadius);
    }

    private void LevelUp()
    {
        level++;
        nextLevelUp = Time.time + levelUpTime;
        bombChance = Mathf.Min(bombChance * 2, 0.5f); // Double the bomb chance with a cap

        UpdateLevelText();
    }

    public void ResetLevel()
    {
        level = 1;
        bombChance = 0.05f; // Reset bomb chance
        UpdateLevelText();
        // Reset other parameters if needed
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
        else
            Debug.LogError("Level text UI is not assigned!");
    }
}
