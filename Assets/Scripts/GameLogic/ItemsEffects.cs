using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class ItemsEffects : MonoBehaviour
{
    [Header("Effect Scripts References")]
    [SerializeField] private MonoBehaviour dashEffect;
    [SerializeField] private MonoBehaviour doubleJumpEffect;
    [SerializeField] private MonoBehaviour shieldEffect;
    [SerializeField] private MonoBehaviour speedBoostEffect;
    [SerializeField] private MonoBehaviour gliderEffect;
    [SerializeField] private MonoBehaviour smallSizeEffect;
    [SerializeField] private MonoBehaviour slowMotionEffect;
    [SerializeField] private MonoBehaviour reverseGravityEffect;
    [SerializeField] private MonoBehaviour wallJumpEffect;
    [SerializeField] private MonoBehaviour walkThroughWallsEffect;
    [SerializeField] private MonoBehaviour depthStriderEffect;

    [Header("Debug")]
    [SerializeField] private bool debugMode = false;

    private ItemsManager itemsManager;
    private GameManager gameManager;
    
    // Dictionary to map items to their effect scripts
    private Dictionary<Item, MonoBehaviour> effectScripts;
    
    // Keep track of currently active effects to avoid unnecessary enable/disable calls
    private HashSet<Item> currentlyActiveEffects = new HashSet<Item>();

    private void Awake()
    {
        // Get component references
        itemsManager = GetComponent<ItemsManager>();
        gameManager = GetComponent<GameManager>();
        
        if (itemsManager == null)
        {
            Debug.LogError("ItemsEffects: ItemsManager component not found on the same GameObject!");
        }
        
        if (gameManager == null)
        {
            Debug.LogError("ItemsEffects: GameManager component not found on the same GameObject!");
        }
        
        // Initialize the effect scripts dictionary
        InitializeEffectScripts();
        
        if (debugMode)
        {
            Debug.Log("ItemsEffects: Initialized with effect scripts dictionary");
        }
    }

    private void Start()
    {
        // Subscribe to GameManager events for level transitions
        if (gameManager != null)
        {
            gameManager.OnProgressReset += OnProgressReset;
        }
        
        // Initial update of effects
        UpdateActiveEffects();
    }

    private void OnDestroy()
    {
        // Unsubscribe from events
        if (gameManager != null)
        {
            gameManager.OnProgressReset -= OnProgressReset;
        }
    }

    private void InitializeEffectScripts()
    {
        effectScripts = new Dictionary<Item, MonoBehaviour>
        {
            { Item.Dash, dashEffect },
            { Item.DoubleJump, doubleJumpEffect },
            { Item.Shield, shieldEffect },
            { Item.SpeedBoost, speedBoostEffect },
            { Item.Glider, gliderEffect },
            { Item.SmallSize, smallSizeEffect },
            { Item.SlowMotion, slowMotionEffect },
            { Item.ReverseGravity, reverseGravityEffect },
            { Item.WallJump, wallJumpEffect },
            { Item.WalkThroughWalls, walkThroughWallsEffect },
            { Item.DepthStrider, depthStriderEffect }
        };
        
        // Initially disable all effect scripts
        DisableAllEffects();
    }

    private void Update()
    {
        // Continuously monitor for changes in active items
        // This ensures effects are updated even when items are changed during gameplay
        UpdateActiveEffects();
    }

    /// <summary>
    /// Check if current scene allows item effects to work
    /// </summary>
    private bool IsValidScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "WorldMenu" || sceneName.StartsWith("Level");
    }

    /// <summary>
    /// Updates which effect scripts are active based on the current items in ItemsManager's active slots
    /// Only works in WorldMenu and Level scenes
    /// </summary>
    private void UpdateActiveEffects()
    {
        if (itemsManager == null) return;
        
        // Only update effects in valid scenes
        if (!IsValidScene())
        {
            // If we're in an invalid scene, disable all effects
            if (currentlyActiveEffects.Count > 0)
            {
                if (debugMode)
                {
                    Debug.Log($"ItemsEffects: Invalid scene '{SceneManager.GetActiveScene().name}' - disabling all effects");
                }
                DisableAllEffects();
            }
            return;
        }

        // Get current active items from ItemsManager
        HashSet<Item> newActiveEffects = new HashSet<Item>();
        
        for (int i = 0; i < itemsManager.itemsSlots.Length; i++)
        {
            Item currentItem = itemsManager.itemsSlots[i];
            if (currentItem != Item.None)
            {
                newActiveEffects.Add(currentItem);
            }
        }
        
        // Check if there are any changes
        if (newActiveEffects.SetEquals(currentlyActiveEffects))
        {
            return; // No changes, skip update
        }
        
        if (debugMode)
        {
            Debug.Log($"ItemsEffects: Updating effects in scene '{SceneManager.GetActiveScene().name}'. New active items: [{string.Join(", ", newActiveEffects)}]");
        }
        
        // Disable effects that are no longer active
        foreach (Item item in currentlyActiveEffects)
        {
            if (!newActiveEffects.Contains(item))
            {
                DisableEffect(item);
            }
        }
        
        // Enable new effects
        foreach (Item item in newActiveEffects)
        {
            if (!currentlyActiveEffects.Contains(item))
            {
                EnableEffect(item);
            }
        }
        
        // Update current active effects
        currentlyActiveEffects = newActiveEffects;
    }

    /// <summary>
    /// Enables the effect script for a specific item
    /// </summary>
    private void EnableEffect(Item item)
    {
        if (effectScripts.TryGetValue(item, out MonoBehaviour effectScript))
        {
            if (effectScript != null)
            {
                effectScript.enabled = true;
                if (debugMode)
                {
                    Debug.Log($"ItemsEffects: Enabled effect for {item}");
                }
            }
            else
            {
                Debug.LogWarning($"ItemsEffects: Effect script for {item} is not assigned in inspector!");
            }
        }
    }

    /// <summary>
    /// Disables the effect script for a specific item
    /// </summary>
    private void DisableEffect(Item item)
    {
        if (effectScripts.TryGetValue(item, out MonoBehaviour effectScript))
        {
            if (effectScript != null)
            {
                effectScript.enabled = false;
                if (debugMode)
                {
                    Debug.Log($"ItemsEffects: Disabled effect for {item}");
                }
            }
        }
    }

    /// <summary>
    /// Disables all effect scripts
    /// </summary>
    private void DisableAllEffects()
    {
        foreach (var kvp in effectScripts)
        {
            if (kvp.Value != null)
            {
                kvp.Value.enabled = false;
            }
        }
        
        currentlyActiveEffects.Clear();
        
        if (debugMode)
        {
            Debug.Log("ItemsEffects: Disabled all effects");
        }
    }

    /// <summary>
    /// Called when GameManager resets progress
    /// </summary>
    private void OnProgressReset()
    {
        if (debugMode)
        {
            Debug.Log("ItemsEffects: Progress reset - disabling all effects");
        }
        
        DisableAllEffects();
    }

    /// <summary>
    /// Manually force an update of active effects (useful for debugging or external calls)
    /// </summary>
    [ContextMenu("Force Update Effects")]
    public void ForceUpdateEffects()
    {
        currentlyActiveEffects.Clear(); // Force a full refresh
        UpdateActiveEffects();
    }

    /// <summary>
    /// Debug method to log current active effects
    /// </summary>
    [ContextMenu("Log Current Active Effects")]
    public void LogCurrentActiveEffects()
    {
        if (itemsManager == null)
        {
            Debug.Log("ItemsEffects: ItemsManager is null");
            return;
        }

        Debug.Log("=== ItemsEffects Debug Info ===");
        Debug.Log($"Active items in ItemsManager: [{string.Join(", ", itemsManager.itemsSlots)}]");
        Debug.Log($"Currently active effects: [{string.Join(", ", currentlyActiveEffects)}]");
        
        // Check effect script assignments
        foreach (var kvp in effectScripts)
        {
            string status = kvp.Value != null ? (kvp.Value.enabled ? "ENABLED" : "disabled") : "NULL";
            Debug.Log($"  {kvp.Key}: {status}");
        }
    }
}
