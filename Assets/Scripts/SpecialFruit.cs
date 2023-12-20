using UnityEngine;

public class SpecialFruit : Fruit
{
    public GameObject[] miniFruitPrefabs; // Prefabs for mini fruits
    public int miniFruitCount = 3;

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        // Perform the base slice actions
        base.Slice(direction, position, force); 

        // Spawn mini fruits as part of the special fruit slicing effect
        SpawnMiniFruits(position, force);

        // Destroy the Special Fruit object
        Destroy(gameObject);
    }

    private void SpawnMiniFruits(Vector3 position, float force)
    {
        for (int i = 0; i < miniFruitCount; i++)
        {
            if(miniFruitPrefabs.Length > 0)
            {
                // Instantiate a random mini fruit prefab at the position of the cut
                GameObject miniFruitPrefab = miniFruitPrefabs[Random.Range(0, miniFruitPrefabs.Length)];
                GameObject miniFruit = Instantiate(miniFruitPrefab, position, Quaternion.identity);
                Rigidbody miniFruitRb = miniFruit.GetComponent<Rigidbody>();

                // If the mini fruits have rigidbodies, apply forces to them
                if (miniFruitRb != null)
                {
                    // Calculate a random direction to apply the force
                    Vector3 randomDirection = Random.insideUnitSphere.normalized;
                    randomDirection.y = Mathf.Abs(randomDirection.y); // Ensure they pop upwards
                    miniFruitRb.AddForce(randomDirection * force, ForceMode.Impulse);
                }
            }
            else
            {
                Debug.LogWarning("Fruit prefabs array is empty. Assign prefabs in the inspector.");
            }
        }
    }
}
