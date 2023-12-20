using UnityEngine;
using System.Collections;
using System.Collections.Generic; // For List<T>

public class SlowFruit : Fruit
{
    public float slowDownDuration = 10f;
    public float slowDownFactor = 0.1f; // More significant slow down
    private static bool isPowerUpActive = false; // Static to keep track across all instances

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        base.Slice(direction, position, force);
        if (!isPowerUpActive)
        {
            ActivatePowerUp();
        }
        Destroy(gameObject);
    }

    private void ActivatePowerUp()
    {
        isPowerUpActive = true;
        Fruit[] fruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in fruits)
        {
            Rigidbody rb = fruit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity *= slowDownFactor;
                rb.useGravity = false; // Turn off gravity to make fruits suspend
            }
        }
        StartCoroutine(ResetFruitSpeedAfterDelay(slowDownDuration));
    }

    private IEnumerator ResetFruitSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        Fruit[] fruits = FindObjectsOfType<Fruit>();
        foreach (Fruit fruit in fruits)
        {
            Rigidbody rb = fruit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity /= slowDownFactor;
                rb.useGravity = true; // Re-enable gravity
            }
        }
        isPowerUpActive = false;
    }

    public static bool IsPowerUpActive()
    {
        return isPowerUpActive;
    }

}
