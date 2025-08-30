using System.Collections;
using UnityEngine;

public class SlowMoItem : MonoBehaviour
{
    public float slowdownFactor = 0.5f; // Half speed
    public float slowdownDuration = 5f; // 5 real-world seconds

    public void ActivateSlowdown()
    {
        StartCoroutine(SlowdownRoutine());
    }

    private IEnumerator SlowdownRoutine()
    {
        Debug.Log("Time slowdown ACTIVATED!");
        // Slow time down
        Time.timeScale = slowdownFactor;

        // Wait for the duration in real-world time, not game time
        yield return new WaitForSecondsRealtime(slowdownDuration);

        // Return time to normal
        Time.timeScale = 1.0f;
        Debug.Log("Time slowdown ENDED.");
    }
}