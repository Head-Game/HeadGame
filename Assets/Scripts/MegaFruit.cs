using UnityEngine;

public class MegaFruit : Fruit
{
    public int sliceCount = 0;
    private int baseScore = 10; // Base score for the first slice
    private int maxSlices = 15;

    public override void Slice(Vector3 direction, Vector3 position, float force)
    {
        base.Slice(direction, position, force);

        // Increase the score every time it is sliced
        IncreaseScorePerSlice();

        // Optionally, do something when the maximum number of slices is reached
        // This part can be removed if you don't need a specific action after a certain number of slices
        sliceCount++;
        if (sliceCount >= maxSlices)
        {
            // Perform any special actions, if needed
            // For example, destroying the MegaFruit or converting it into smaller fruits
        }
    }

    private void IncreaseScorePerSlice()
    {
        // Double the score cumulatively for each slice
        int scoreForThisSlice = baseScore * (1 << sliceCount); // Equivalent to Math.Pow(2, sliceCount)

        // Update the player's score
        FindObjectOfType<GameManager>().IncreaseScore(scoreForThisSlice);
    }
}
