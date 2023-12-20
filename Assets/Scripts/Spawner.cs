using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Spawner : MonoBehaviour
{
    public GameObject[] fruitPrefabs;
    public GameObject bombPrefab;
    public GameObject megaFruitPrefab;
    public GameObject specialFruitPrefab;
    public GameObject slowFruitPrefab;
    public GameObject fruitPrefab; 
    private Vector3 spawnPosition; 
    private Quaternion spawnRotation = Quaternion.identity; 
    public float slowDownFactor = 0.5f;
    private Vector3 initialVelocity; 

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

    // Performance tracking variables
    private int successfulFruitCollections = 0;
    private float totalCutTime = 0f;
    private int cutFruitsCount = 0;

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
        Fruit.sliceClip = Resources.Load<AudioClip>("squelch");
        Bomb.explosionClip = Resources.Load<AudioClip>("gameover");
        Bomb.tickingClip = Resources.Load<AudioClip>("tick");

        spawnPosition = GenerateSpawnPosition(); // If you want a default spawn position
        initialVelocity = new Vector3(0, 10, 0); 
    }

    public void StartSpawning()
    {
        StartCoroutine(Spawn());
        nextLevelUp = Time.time + levelUpTime;
        UpdateLevelText();
    }

    private void SpawnFruit()
    {
        GameObject fruitGO = Instantiate(fruitPrefab, spawnPosition, spawnRotation);
        Rigidbody fruitRB = fruitGO.GetComponent<Rigidbody>();

        if (SlowFruit.IsPowerUpActive())
        {
            fruitRB.velocity *= slowDownFactor; // Use the same slow down factor as in SlowFruit
            fruitRB.useGravity = false; // Suspend new fruits in the air
        }
        else
        {
            // Normal spawning logic
            fruitRB.velocity = initialVelocity;
        }
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

            GameObject prefab = ChoosePrefabToSpawn();
            Vector3 position = GenerateSpawnPosition();
            Quaternion rotation = Quaternion.Euler(0f, 0f, Random.Range(minAngle, maxAngle));

            GameObject fruit = Instantiate(prefab, position, rotation);
            Rigidbody fruitRb = fruit.GetComponent<Rigidbody>();

            // If the slow-down power-up is active, apply the effects
            if (SlowFruit.IsPowerUpActive())
            {
                fruitRb.velocity *= slowDownFactor; // Use the same slow down factor as in SlowFruit
                fruitRb.useGravity = false; // Suspend new fruits in the air
            }
            else
            {
                // Apply the initial force as normal
                float force = Mathf.Lerp(minForce, maxForce, level / 10f);
                fruitRb.AddForce(Vector3.up * force, ForceMode.Impulse);
            }

            fruitRb.mass = Mathf.Lerp(maxGravity, minGravity, 1f - (level / 10f));

            Destroy(fruit, maxLifetime);

            float spawnDelay = Mathf.Lerp(initialMaxSpawnDelay, finalMinSpawnDelay, level / 10f);
            yield return new WaitForSeconds(spawnDelay);
        }
    }


    private GameObject ChoosePrefabToSpawn()
    {
        if (level >= 3 && Random.value < 0.1f) // Chance to spawn Special Fruit
        {
            return specialFruitPrefab;
        }
        else if (level >= 5 && Random.value < 0.1f) // Chance to spawn Mega Fruit
        {
            return megaFruitPrefab;
        }
        else if (level >= 7 && Random.value < 0.1f) // Chance to spawn Slow Fruit
        {
            return slowFruitPrefab;
        }
        else if (Random.value < bombChance)
        {
            return bombPrefab;
        }
        else
        {
            return fruitPrefabs[Random.Range(0, fruitPrefabs.Length)];
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
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
            levelText.text = "Level: " + level;
        else
            Debug.LogError("Level text UI is not assigned!");
    }

    public void ResetGame()
    {
        ResetLevel();
        StopAllCoroutines();
        StartCoroutine(Spawn()); // Restart spawning
    }

    // New method for tracking fruit collection performance
    public void OnFruitCut(float cutTime)
    {
        cutFruitsCount++;
        totalCutTime += cutTime;

        if (cutFruitsCount >= 10) // Check performance every 10 fruits
        {
            float averageCutTime = totalCutTime / cutFruitsCount;
            AdjustDifficultyBasedOnPerformance(averageCutTime);
            cutFruitsCount = 0; // Reset for the next batch
            totalCutTime = 0f; // Reset total cut time
        }
    }

    // New method for adjusting difficulty
    private void AdjustDifficultyBasedOnPerformance(float averageCutTime)
    {
        // Example: Increase difficulty if the player is cutting fruits quickly
        if (averageCutTime < 2.0f) // Example threshold, adjust as needed
        {
            LevelUp();
        }
    }
}
