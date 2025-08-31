using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Effect script for the Shield item
/// Protects player from one hit when active - can only be used once per scene
/// Provides 0.2s invincibility window when triggered
/// Only works in WorldMenu and Level scenes
/// </summary>
public class ShieldEffect : MonoBehaviour
{
    [Header("Shield Settings")]
    [SerializeField] private float invincibilityDuration = 0.2f;
    [SerializeField] private bool debugMode = false;

    private bool effectActive = false;
    private bool hasBeenUsedThisScene = false;
    private bool isInvincible = false;
    private string currentSceneName = "";

    private void OnEnable()
    {
        if (IsValidScene())
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
                Debug.Log($"ShieldEffect: Activated in scene '{sceneName}', used this scene: {hasBeenUsedThisScene}");
            }
        }
    }

    private void OnDisable()
    {
        effectActive = false;
        
        // Stop any ongoing invincibility coroutine
        if (isInvincible)
        {
            StopAllCoroutines();
            isInvincible = false;
        }
        
        if (debugMode)
        {
            Debug.Log("ShieldEffect: Deactivated");
        }
    }

    private bool IsValidScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "WorldMenu" || sceneName.StartsWith("Level");
    }

    /// <summary>
    /// Called when player takes damage - returns true if damage was blocked
    /// </summary>
    public bool TryBlockDamage()
    {
        if (!effectActive || !IsValidScene() || hasBeenUsedThisScene || isInvincible) 
        {
            return false; // Cannot block
        }
        
        // Shield can be used - trigger it
        TriggerShield();
        return true; // Damage blocked
    }

    /// <summary>
    /// Triggers the shield effect - provides invincibility window then uses up the shield
    /// </summary>
    private void TriggerShield()
    {
        hasBeenUsedThisScene = true;
        StartCoroutine(InvincibilityWindow());
        
        if (debugMode)
        {
            Debug.Log($"ShieldEffect: Shield triggered! {invincibilityDuration}s invincibility window activated");
        }
        
        // TODO: Add shield activation visual/audio effects
        // Example:
        // - Play shield activation sound
        // - Show shield activation particle effect
        // - Flash player sprite or show shield overlay
    }

    /// <summary>
    /// Provides invincibility window for the specified duration
    /// </summary>
    private IEnumerator InvincibilityWindow()
    {
        isInvincible = true;
        
        if (debugMode)
        {
            Debug.Log("ShieldEffect: Invincibility window started");
        }
        
        yield return new WaitForSeconds(invincibilityDuration);
        
        isInvincible = false;
        
        if (debugMode)
        {
            Debug.Log("ShieldEffect: Invincibility window ended - shield used up for this scene");
        }
        
        // TODO: Add invincibility end visual/audio effects
        // Example:
        // - Play shield wear-off sound
        // - Show shield dissipation effect
        // - Update UI to show shield is no longer available
    }

    /// <summary>
    /// Get shield status information
    /// </summary>
    public bool CanUseShield 
    {
        get { return effectActive && !hasBeenUsedThisScene && IsValidScene(); }
    }
    
    public bool HasBeenUsedThisScene 
    {
        get { return hasBeenUsedThisScene; }
    }
    
    public bool IsInvincible 
    {
        get { return isInvincible; }
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
            Debug.Log("ShieldEffect: Usage reset for current scene");
        }
    }

    /// <summary>
    /// Debug method to manually test shield activation
    /// </summary>
    [ContextMenu("Test Shield")]
    public void TestShield()
    {
        if (effectActive)
        {
            if (CanUseShield)
            {
                bool blocked = TryBlockDamage();
                Debug.Log($"ShieldEffect: Damage blocked: {blocked}");
            }
            else if (hasBeenUsedThisScene)
            {
                Debug.Log("ShieldEffect: Shield already used in this scene");
            }
            else if (!IsValidScene())
            {
                Debug.Log($"ShieldEffect: Invalid scene '{SceneManager.GetActiveScene().name}'");
            }
        }
        else
        {
            Debug.Log("ShieldEffect: Effect not active");
        }
    }
}
