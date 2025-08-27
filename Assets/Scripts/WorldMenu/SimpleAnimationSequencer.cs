using System.Collections;
using UnityEngine;

[System.Serializable]
public class LevelAnimationGroup
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";
    
    [Header("Sprites for this Level")]
    public Animator[] animators;
}

public class SimpleAnimationSequencer : MonoBehaviour
{
    [Header("Level Animation Groups")]
    [SerializeField] private LevelAnimationGroup[] levelGroups;
    
    [Header("Animation Settings")]
    [SerializeField] private string animationStateName = "Line";
    [SerializeField] private string idleStateName = "Line_Idle";
    [SerializeField] private string drawnStateName = "Line_Drawn";
    [SerializeField] private float delayBetweenAnimations = 0.1f;
    
    // Events
    public System.Action OnSequenceComplete;
    
    private void Start()
    {
        // Initialize all sprites to idle state on scene start
        InitializeToIdleState();
    }
    
    /// <summary>
    /// Play animations for a specific level completion (1-based level number)
    /// </summary>
    public void PlayLevelCompletionAnimation(int completedLevel)
    {
        StartCoroutine(PlayLevelAnimation(completedLevel));
    }
    
    /// <summary>
    /// Play animations for all levels up to the specified level
    /// </summary>
    public void PlayAllAnimationsUpToLevel(int targetLevel)
    {
        StartCoroutine(PlayAllLevelsAnimation(targetLevel));
    }
    
    /// <summary>
    /// Legacy method - play all animations in all groups
    /// </summary>
    public void PlaySequence()
    {
        if (levelGroups != null && levelGroups.Length > 0)
        {
            PlayAllAnimationsUpToLevel(levelGroups.Length);
        }
    }
    
    /// <summary>
    /// Legacy method for backward compatibility
    /// </summary>
    public void PlaySequenceToStep(int targetStep)
    {
        // Convert old step-based to level-based
        PlayLevelCompletionAnimation(targetStep + 1);
    }
    
    /// <summary>
    /// Reset all animations to idle state
    /// </summary>
    public void ResetSequence()
    {
        InitializeToIdleState();
    }
    
    /// <summary>
    /// Initialize all sprites to idle state (called on Start and Reset)
    /// </summary>
    private void InitializeToIdleState()
    {
        if (levelGroups == null || levelGroups.Length == 0)
        {
            Debug.LogWarning("SimpleAnimationSequencer: No level groups assigned for initialization!");
            return;
        }
        
        int initializedCount = 0;
        foreach (var group in levelGroups)
        {
            if (group?.animators != null)
            {
                foreach (var animator in group.animators)
                {
                    if (animator != null)
                    {
                        animator.Play(idleStateName, 0, 0f);
                        animator.speed = 1f;
                        initializedCount++;
                    }
                }
            }
        }
        Debug.Log($"SimpleAnimationSequencer: {initializedCount} sprites initialized to idle state");
    }
    
    private IEnumerator PlayLevelAnimation(int levelNumber)
    {
        // Find the level group
        LevelAnimationGroup targetGroup = null;
        foreach (var group in levelGroups)
        {
            if (group != null && group.levelNumber == levelNumber)
            {
                targetGroup = group;
                break;
            }
        }
        
        if (targetGroup == null)
        {
            Debug.LogWarning($"SimpleAnimationSequencer: Level {levelNumber} not found! Skipping animation.");
            OnSequenceComplete?.Invoke();
            yield break;
        }
        
        if (targetGroup.animators == null || targetGroup.animators.Length == 0)
        {
            Debug.LogWarning($"SimpleAnimationSequencer: Level {levelNumber} has no animators! Skipping animation.");
            OnSequenceComplete?.Invoke();
            yield break;
        }
        
        string levelDisplayName = string.IsNullOrEmpty(targetGroup.levelName) ? $"Level {levelNumber}" : targetGroup.levelName;
        Debug.Log($"SimpleAnimationSequencer: Playing animation for {levelDisplayName}");
        
        // Play animations sequentially with ZERO delay
        for (int i = 0; i < targetGroup.animators.Length; i++)
        {
            if (targetGroup.animators[i] != null)
            {
                Debug.Log($"SimpleAnimationSequencer: Starting animation {i + 1}/{targetGroup.animators.Length} on {targetGroup.animators[i].name}");
                yield return StartCoroutine(PlaySingleAnimation(targetGroup.animators[i]));
                // NO DELAY - immediately start next animation when previous finishes
            }
            else
            {
                Debug.Log($"SimpleAnimationSequencer: Skipping null animator at index {i} in level {levelNumber}");
            }
        }
        
        Debug.Log($"SimpleAnimationSequencer: {levelDisplayName} animation complete!");
        OnSequenceComplete?.Invoke();
    }
    
    private IEnumerator PlayAllLevelsAnimation(int maxLevel)
    {
        if (levelGroups == null || levelGroups.Length == 0)
        {
            Debug.LogWarning("SimpleAnimationSequencer: No level groups assigned! Skipping animation.");
            OnSequenceComplete?.Invoke();
            yield break;
        }
        
        // Play each level in order up to maxLevel
        for (int level = 1; level <= maxLevel; level++)
        {
            LevelAnimationGroup targetGroup = null;
            foreach (var group in levelGroups)
            {
                if (group != null && group.levelNumber == level)
                {
                    targetGroup = group;
                    break;
                }
            }
            
            if (targetGroup?.animators != null)
            {
                Debug.Log($"SimpleAnimationSequencer: Playing {targetGroup.levelName}");
                
                for (int i = 0; i < targetGroup.animators.Length; i++)
                {
                    if (targetGroup.animators[i] != null)
                    {
                        yield return StartCoroutine(PlaySingleAnimation(targetGroup.animators[i]));
                        
                        if (delayBetweenAnimations > 0)
                        {
                            yield return new WaitForSeconds(delayBetweenAnimations);
                        }
                    }
                }
            }
        }
        
        Debug.Log($"SimpleAnimationSequencer: All animations complete up to level {maxLevel}!");
        OnSequenceComplete?.Invoke();
    }
    
    private IEnumerator PlaySingleAnimation(Animator animator)
    {
        // Start the drawing animation
        animator.speed = 1f; // Make sure it's not paused
        animator.Play(animationStateName, 0, 0f); // Play from start
        
        // Wait for the animation to finish
        yield return new WaitForEndOfFrame(); // Wait one frame for the state to start
        
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(animationStateName) && 
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // Wait until animation completes
        }
        
        // After animation completes, try to go to drawn state (if it exists)
        if (animator.HasState(0, Animator.StringToHash(drawnStateName)))
        {
            animator.Play(drawnStateName, 0, 0f);
            Debug.Log($"Animation complete on {animator.name}, now in drawn state");
        }
        else
        {
            // If Line_Drawn doesn't exist, just stay where we are (animation completed)
            Debug.Log($"Animation complete on {animator.name}, Line_Drawn state not found - animation stays completed");
        }
    }
    
    private bool HasAnimatorState(Animator animator, string stateName)
    {
        if (animator == null || animator.runtimeAnimatorController == null) return false;
        
        // Check all states in all layers
        for (int layer = 0; layer < animator.layerCount; layer++)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
            if (stateInfo.IsName(stateName)) return true;
            
            // Also check if we can transition to this state
            if (animator.HasState(layer, Animator.StringToHash(stateName))) return true;
        }
        
        return false;
    }
    
    // Debug method for testing
    [ContextMenu("Test Play Sequence")]
    public void TestPlaySequence()
    {
        PlaySequence();
    }
    
    [ContextMenu("Test Play Level 1")]
    public void TestPlayLevel1()
    {
        PlayLevelCompletionAnimation(1);
    }
    
    [ContextMenu("Test Play Level 2")]
    public void TestPlayLevel2()
    {
        PlayLevelCompletionAnimation(2);
    }
    
    [ContextMenu("Force Zero Delay")]
    public void ForceZeroDelay()
    {
        delayBetweenAnimations = 0f;
        Debug.Log("SimpleAnimationSequencer: Delay between animations set to 0 (no delay)");
    }
    
    [ContextMenu("Test Reset")]
    public void TestReset()
    {
        ResetSequence();
    }
    
    [ContextMenu("Debug Level Groups")]
    public void DebugLevelGroups()
    {
        if (levelGroups == null)
        {
            Debug.Log("SimpleAnimationSequencer: Level groups array is NULL!");
            return;
        }
        
        Debug.Log($"SimpleAnimationSequencer: Total level groups: {levelGroups.Length}");
        
        int totalAnimators = 0;
        for (int i = 0; i < levelGroups.Length; i++)
        {
            var group = levelGroups[i];
            if (group == null)
            {
                Debug.Log($"  Group [{i}] is NULL");
                continue;
            }
            
            int validCount = 0;
            if (group.animators != null)
            {
                foreach (var animator in group.animators)
                {
                    if (animator != null) validCount++;
                }
                totalAnimators += validCount;
            }
            
            Debug.Log($"  Group [{i}] Level {group.levelNumber}: '{group.levelName}' - {validCount} valid animators");
        }
        
        Debug.Log($"SimpleAnimationSequencer: Total valid animators across all levels: {totalAnimators}");
    }
}
