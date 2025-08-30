using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public float ElapsedGameTime { get; private set; }

    private bool timerRunning = false;

    private float maxLevelTime = 0f;

    public void Pause()
    {
        Time.timeScale = 0f;
        timerRunning = false;
    }

    public void Resume()
    {
        Time.timeScale = 1f;
        timerRunning = false;
    }

    public void StartNewLevel(float maxTime)
    {
        ElapsedGameTime = 0f;
        maxLevelTime = maxTime;
        timerRunning = true;
    }

    private void LevelTimeout()
    {
        timerRunning = false;
        GameManager.Instance.Die();
    }

    /// <summary>
    /// Stops the timer and return the elapsed time when the level was completed.
    /// </summary>
    public float LevelCompleteTime()
    {
        timerRunning = false;
        return ElapsedGameTime;
    }

    private void Update()
    {
        if (timerRunning)
        {
            ElapsedGameTime += Time.deltaTime;

            if(ElapsedGameTime >= maxLevelTime && maxLevelTime > 0f)
            {
                LevelTimeout();
            }
        }
    }
}
