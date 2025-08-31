using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Effect script for the Speed Boost item
/// Increases player movement speed when active
/// Only works in WorldMenu and Level scenes
/// </summary>
public class SpeedBoostEffect : MonoBehaviour
{
    [Header("Speed Boost Settings")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [SerializeField] private bool debugMode = false;

    private bool effectActive = false;

    private void OnEnable()
    {
        if (!effectActive && IsValidScene())
        {
            ActivateSpeedBoost();
        }
    }

    private void OnDisable()
    {
        if (effectActive)
        {
            DeactivateSpeedBoost();
        }
    }

    private bool IsValidScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "WorldMenu" || sceneName.StartsWith("Level");
    }

    private void ActivateSpeedBoost()
    {
        if (!IsValidScene()) return;
        
        effectActive = true;
        
        if (debugMode)
        {
            Debug.Log($"SpeedBoostEffect: Activated with {speedMultiplier}x speed multiplier in scene '{SceneManager.GetActiveScene().name}'");
        }
        
        // TODO: Apply speed boost to player character
        // Example implementation:
        // - Find player controller/rigidbody
        // - Multiply movement speed by speedMultiplier
        // - Store original speed for restoration
        
        // Placeholder for actual implementation:
        // PlayerController playerController = FindObjectOfType<PlayerController>();
        // if (playerController != null)
        // {
        //     playerController.ApplySpeedMultiplier(speedMultiplier);
        // }
    }

    private void DeactivateSpeedBoost()
    {
        effectActive = false;
        
        if (debugMode)
        {
            Debug.Log("SpeedBoostEffect: Deactivated, restoring normal speed");
        }
        
        // TODO: Restore original player speed
        // Example implementation:
        // PlayerController playerController = FindObjectOfType<PlayerController>();
        // if (playerController != null)
        // {
        //     playerController.RestoreNormalSpeed();
        // }
    }

    /// <summary>
    /// Debug method to manually test the effect
    /// </summary>
    [ContextMenu("Test Speed Boost")]
    public void TestSpeedBoost()
    {
        if (effectActive)
        {
            DeactivateSpeedBoost();
        }
        else
        {
            ActivateSpeedBoost();
        }
    }
}
