using UnityEngine;
using System.Collections;

public class SlowFruit : Fruit
{
    public float slowDownDuration = 10f;
    public float slowDownFactor = 0.1f;
    private static bool isPowerUpActive = false;

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

        if (!isPowerUpActive)
        {
            ActivatePowerUp();
            PlaySound(powerUpSound);
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
                rb.useGravity = false;
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
