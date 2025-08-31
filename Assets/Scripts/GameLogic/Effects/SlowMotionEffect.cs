using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Effect script for the Slow Motion item
/// Slows down time for a short period - can only be used once per scene
/// </summary>
public class SlowMotionEffect : MonoBehaviour
{
    [Header("Slow Motion Settings")]
    [SerializeField] private float slowdownFactor = 0.5f; // Half speed
    [SerializeField] private float slowdownDuration = 5f; // 5 real-world seconds
    [SerializeField] private KeyCode slowMoKey = KeyCode.Space;
    [SerializeField] private bool debugMode = false;

    private bool effectActive = false;
    private bool hasBeenUsedThisScene = false;
    private bool slowMotionInProgress = false;
    private string currentSceneName = "";

    private void OnEnable()
    {
        effectActive = true;
        
        // Check if we're in a new scene
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName != currentSceneName)
        {
            currentSceneName = sceneName;
            hasBeenUsedThisScene = false; // Reset usage for new scene
        }
        
        if (debugMode)
        {
            Debug.Log($"SlowMotionEffect: Activated in scene '{sceneName}', used this scene: {hasBeenUsedThisScene}");
        }
    }

    private void OnDisable()
    {
        effectActive = false;
        
        // If slow motion is in progress when disabled, restore normal time
        if (slowMotionInProgress)
        {
            StopAllCoroutines();
            Time.timeScale = 1.0f;
            slowMotionInProgress = false;
        }
        
        if (debugMode)
        {
            Debug.Log("SlowMotionEffect: Deactivated");
        }
    }

    private void Update()
    {
        if (!effectActive || !IsValidScene()) return;

        // Check for slow motion input
        if (Input.GetKeyDown(slowMoKey) && CanActivateSlowMotion())
        {
            ActivateSlowdown();
        }
    }

    private bool IsValidScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "WorldMenu" || sceneName.StartsWith("Level");
    }

    private bool CanActivateSlowMotion()
    {
        return !hasBeenUsedThisScene && !slowMotionInProgress;
    }

    /// <summary>
    /// Activates the slow motion effect
    /// </summary>
    public void ActivateSlowdown()
    {
        if (!effectActive || !CanActivateSlowMotion() || !IsValidScene())
        {
            if (debugMode)
            {
                Debug.Log($"SlowMotionEffect: Cannot activate - Active: {effectActive}, CanUse: {CanActivateSlowMotion()}, ValidScene: {IsValidScene()}");
            }
            return;
        }

        hasBeenUsedThisScene = true;
        StartCoroutine(SlowdownRoutine());
    }

    private IEnumerator SlowdownRoutine()
    {
        slowMotionInProgress = true;
        
        if (debugMode)
        {
            Debug.Log($"SlowMotionEffect: Time slowdown ACTIVATED! Factor: {slowdownFactor}, Duration: {slowdownDuration}s");
        }
        
        // Slow time down
        Time.timeScale = slowdownFactor;

        // Wait for the duration in real-world time, not game time
        yield return new WaitForSecondsRealtime(slowdownDuration);

        // Return time to normal
        Time.timeScale = 1.0f;
        slowMotionInProgress = false;
        
        if (debugMode)
        {
            Debug.Log("SlowMotionEffect: Time slowdown ENDED.");
        }
    }

    /// <summary>
    /// Check if slow motion can be activated
    /// </summary>
    public bool CanUseSlowMotion => effectActive && CanActivateSlowMotion() && IsValidScene();
    
    /// <summary>
    /// Check if slow motion has been used in current scene
    /// </summary>
    public bool HasBeenUsedThisScene => hasBeenUsedThisScene;
    
    /// <summary>
    /// Check if slow motion is currently in progress
    /// </summary>
    public bool IsSlowMotionActive => slowMotionInProgress;

    /// <summary>
    /// Get remaining slow motion duration
    /// </summary>
    public float GetRemainingDuration()
    {
        // This would need to be implemented with a timer if you want to show remaining time
        // For now, just return whether it's active or not
        return slowMotionInProgress ? 1f : 0f;
    }

    /// <summary>
    /// Force reset the usage for testing
    /// </summary>
    [ContextMenu("Reset Usage")]
    public void ResetUsage()
    {
        hasBeenUsedThisScene = false;
        if (debugMode)
        {
            Debug.Log("SlowMotionEffect: Usage reset for current scene");
        }
    }

    /// <summary>
    /// Debug method to manually test slow motion
    /// </summary>
    [ContextMenu("Test Slow Motion")]
    public void TestSlowMotion()
    {
        if (effectActive)
        {
            if (CanUseSlowMotion)
            {
                ActivateSlowdown();
            }
            else if (hasBeenUsedThisScene)
            {
                Debug.Log("SlowMotionEffect: Already used in this scene");
            }
            else if (slowMotionInProgress)
            {
                Debug.Log("SlowMotionEffect: Slow motion already in progress");
            }
            else if (!IsValidScene())
            {
                Debug.Log($"SlowMotionEffect: Invalid scene '{SceneManager.GetActiveScene().name}'");
            }
        }
        else
        {
            Debug.Log("SlowMotionEffect: Effect not active");
        }
    }
}
