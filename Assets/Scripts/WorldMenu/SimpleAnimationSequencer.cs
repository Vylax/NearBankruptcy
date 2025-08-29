using System.Collections;
using UnityEngine;

[System.Serializable]
public class LevelAnimationGroup
{
    [Header("Level Info")]
    public int levelNumber = 1;
    public string levelName = "Level 1";
    
    [Header("Line Sprites for this Level")]
    public Animator[] lineAnimators;
    
    [Header("Level Bump (Final Animation)")]
    public Animator bumpAnimator;
}

public class SimpleAnimationSequencer : MonoBehaviour
{
    [Header("Level Animation Groups")]
    [SerializeField] private LevelAnimationGroup[] levelGroups;
    
    [Header("Line Animation Settings")]
    [SerializeField] private string lineAnimationStateName = "Line";
    [SerializeField] private string lineIdleStateName = "Line_Idle";
    [SerializeField] private string lineDrawnStateName = "Line_drawn";
    
    [Header("Bump Animation Settings")]
    [SerializeField] private string bumpAnimationStateName = "Drawing";
    [SerializeField] private string bumpIdleStateName = "Hidden";
    [SerializeField] private string bumpDrawnStateName = "Drawn";
    
    [Header("Timing Settings")]
    [SerializeField] private float delayBetweenAnimations = 0.1f;
    
    [Header("Bump Color Settings")]
    [SerializeField] private Color doneColor = Color.green;
    [SerializeField] private Color lockedColor = Color.gray;
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private float colorFadeDuration = 1.0f;
    
    [Header("Final Completion Visuals")]
    [SerializeField] private Animator[] finalCompletionSprites;
    
    // Events
    public System.Action OnSequenceComplete;
    
    private void Start()
    {
        // Initialize all sprites to idle state on scene start
        InitializeToIdleState();
        
        // Set initial bump colors based on level status
        Invoke(nameof(UpdateBumpColors), 0.1f); // Small delay to ensure GameManager is ready
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
    /// Set COMPLETED levels to drawn state instantly (lines + bumps) - for showing existing progress
    /// </summary>
    public void SetLevelsToDrawnState(int maxCompletedLevel)
    {
        if (levelGroups == null || levelGroups.Length == 0)
        {
            Debug.LogWarning("SimpleAnimationSequencer: No level groups assigned!");
            return;
        }

        int setCount = 0;
        
        // Set completed levels (lines + bumps to drawn)
        for (int level = 1; level <= maxCompletedLevel; level++)
        {
            LevelAnimationGroup targetGroup = FindLevelGroup(level);
            if (targetGroup != null)
            {
                // Set line sprites to drawn state
                if (targetGroup.lineAnimators != null)
                {
                    foreach (var animator in targetGroup.lineAnimators)
                    {
                        if (animator != null && animator.HasState(0, Animator.StringToHash(lineDrawnStateName)))
                        {
                            animator.Play(lineDrawnStateName, 0, 0f);
                            setCount++;
                        }
                    }
                }
                
                // Set bump to drawn state (only for completed levels)
                if (targetGroup.bumpAnimator != null && targetGroup.bumpAnimator.HasState(0, Animator.StringToHash(bumpDrawnStateName)))
                {
                    targetGroup.bumpAnimator.Play(bumpDrawnStateName, 0, 0f);
                    setCount++;
                }
            }
        }

        Debug.Log($"SimpleAnimationSequencer: Set {setCount} sprites to drawn state for completed levels 1-{maxCompletedLevel}");
        
        // Update bump colors after setting existing progress
        UpdateBumpColors();
    }
    
    /// <summary>
    /// Set ONLY line sprites to drawn state for current level (no bump) - for initial scene drawing
    /// </summary>
    public void SetCurrentLevelLinesToDrawnState(int currentLevel)
    {
        LevelAnimationGroup targetGroup = FindLevelGroup(currentLevel);
        if (targetGroup?.lineAnimators != null)
        {
            int setCount = 0;
            foreach (var animator in targetGroup.lineAnimators)
            {
                if (animator != null && animator.HasState(0, Animator.StringToHash(lineDrawnStateName)))
                {
                    animator.Play(lineDrawnStateName, 0, 0f);
                    setCount++;
                }
            }
            Debug.Log($"SimpleAnimationSequencer: Set {setCount} line sprites to drawn state for current level {currentLevel} (no bump)");
        }
    }
    
    /// <summary>
    /// Draw current level lines with animation (3s after scene load)
    /// </summary>
    public void DrawCurrentLevelLinesWithDelay()
    {
        StartCoroutine(DrawCurrentLevelLinesCoroutine());
    }
    
    private IEnumerator DrawCurrentLevelLinesCoroutine()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("SimpleAnimationSequencer: GameManager not available for initial line drawing");
            yield break;
        }
        
        int currentLevel = GameManager.Instance.CurrentLevel;
        
        // Wait 3 seconds after scene load (only for fresh start, not after reset)
        yield return new WaitForSeconds(3f);
        
        // Only draw if this level hasn't been completed yet
        if (currentLevel == 1 || !IsLevelCompleted(currentLevel - 1))
        {
            Debug.Log($"SimpleAnimationSequencer: Drawing initial lines for current level {currentLevel} (after 3s delay)");
            yield return StartCoroutine(PlayLevelLinesOnly(currentLevel));
        }
        else
        {
            Debug.Log($"SimpleAnimationSequencer: Skipping initial line drawing - level {currentLevel} lines already drawn");
        }
    }
    
    /// <summary>
    /// Play only the line animations for a level (no bump)
    /// </summary>
    private IEnumerator PlayLevelLinesOnly(int levelNumber)
    {
        LevelAnimationGroup targetGroup = FindLevelGroup(levelNumber);
        if (targetGroup?.lineAnimators == null)
        {
            Debug.LogWarning($"SimpleAnimationSequencer: No line animators for level {levelNumber}");
            yield break;
        }
        
        Debug.Log($"SimpleAnimationSequencer: Playing line animations only for level {levelNumber}");
        
        // Play line animations sequentially
        for (int i = 0; i < targetGroup.lineAnimators.Length; i++)
        {
            if (targetGroup.lineAnimators[i] != null)
            {
                Debug.Log($"SimpleAnimationSequencer: Drawing line {i + 1}/{targetGroup.lineAnimators.Length} for level {levelNumber}");
                yield return StartCoroutine(PlaySingleLineAnimation(targetGroup.lineAnimators[i]));
            }
        }
        
        Debug.Log($"SimpleAnimationSequencer: Line drawing complete for level {levelNumber} (bump remains hidden)");
    }
    
    /// <summary>
    /// Check if a level has been completed (has both lines and bump drawn)
    /// </summary>
    private bool IsLevelCompleted(int levelNumber)
    {
        LevelAnimationGroup targetGroup = FindLevelGroup(levelNumber);
        if (targetGroup?.bumpAnimator == null) return false;
        
        // Check if bump is in drawn state
        return targetGroup.bumpAnimator.GetCurrentAnimatorStateInfo(0).IsName(bumpDrawnStateName);
    }
    

    
    /// <summary>
    /// Reset all animations to idle state
    /// </summary>
    public void ResetSequence()
    {
        InitializeToIdleState();
        UpdateBumpColors();
        
        // After reset, immediately start drawing level 1 lines (no delay)
        if (GameManager.Instance != null && GameManager.Instance.CurrentLevel == 1)
        {
            Debug.Log("SimpleAnimationSequencer: Progress reset - immediately drawing level 1 lines");
            StartCoroutine(PlayLevelLinesOnly(1));
        }
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
            if (group != null)
            {
                // Initialize line sprites to idle
                if (group.lineAnimators != null)
                {
                    foreach (var animator in group.lineAnimators)
                    {
                        if (animator != null)
                        {
                            animator.Play(lineIdleStateName, 0, 0f);
                            animator.speed = 1f;
                            initializedCount++;
                        }
                    }
                }
                
                // Initialize bump to hidden/idle
                if (group.bumpAnimator != null)
                {
                    group.bumpAnimator.Play(bumpIdleStateName, 0, 0f);
                    group.bumpAnimator.speed = 1f;
                    initializedCount++;
                }
            }
        }
        
        // ALSO RESET FINAL COMPLETION SPRITES
        if (finalCompletionSprites != null)
        {
            foreach (var animator in finalCompletionSprites)
            {
                if (animator != null)
                {
                    animator.Play(lineIdleStateName, 0, 0f);
                    animator.speed = 1f;
                    initializedCount++;
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
        
        string levelDisplayName = string.IsNullOrEmpty(targetGroup.levelName) ? $"Level {levelNumber}" : targetGroup.levelName;
        Debug.Log($"SimpleAnimationSequencer: Level {levelNumber} completion - drawing bump + next level lines");
        
        // Skip drawing lines for completed level (they should already be drawn)
        // Go straight to drawing the bump for the completed level
        if (targetGroup.bumpAnimator != null)
        {
            Debug.Log($"SimpleAnimationSequencer: Drawing bump for completed level {levelNumber}");
            yield return StartCoroutine(PlaySingleBumpAnimation(targetGroup.bumpAnimator, levelNumber));
        }
        else
        {
            Debug.LogWarning($"SimpleAnimationSequencer: No bump animator for completed level {levelNumber}");
        }

        // Check what to do after bump animation
        if (GameManager.Instance != null)
        {
            int maxLevels = GameManager.Instance.MaxLevels;
            
            Debug.Log($"SimpleAnimationSequencer: Completed level {levelNumber}, max levels = {maxLevels}");
            
            // Check if we just completed the final level
            if (levelNumber >= maxLevels)
            {
                Debug.Log($"SimpleAnimationSequencer: Just completed the final level ({levelNumber})! Going straight to final completion visuals");
                yield return StartCoroutine(PlayFinalCompletionVisuals());
            }
            else
            {
                // Not the final level - draw lines for the next level
                int nextLevel = GameManager.Instance.CurrentLevel; // This is the new current level after completion
                Debug.Log($"SimpleAnimationSequencer: Drawing lines for next level {nextLevel}");
                
                // Start both line drawing AND color fade simultaneously for smoother flow
                StartCoroutine(PlayLevelLinesOnly(nextLevel));
                
                // Start color fade for next level bump (parallel with line drawing)
                var nextLevelGroup = FindLevelGroup(nextLevel);
                if (nextLevelGroup?.bumpAnimator != null)
                {
                    Debug.Log($"SimpleAnimationSequencer: Starting color fade for level {nextLevel} bump (parallel with line drawing)");
                    StartCoroutine(FadeBumpColor(nextLevelGroup.bumpAnimator, lockedColor, unlockedColor));
                }
                
                // Wait for lines to finish drawing
                yield return StartCoroutine(WaitForLevelLinesComplete(nextLevel));
            }
        }

        Debug.Log($"SimpleAnimationSequencer: {levelDisplayName} completion sequence finished!");
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
            
            if (targetGroup != null)
            {
                Debug.Log($"SimpleAnimationSequencer: Playing {targetGroup.levelName}");
                
                // Play line animations
                if (targetGroup.lineAnimators != null)
                {
                    for (int i = 0; i < targetGroup.lineAnimators.Length; i++)
                    {
                        if (targetGroup.lineAnimators[i] != null)
                        {
                            yield return StartCoroutine(PlaySingleLineAnimation(targetGroup.lineAnimators[i]));
                            
                            if (delayBetweenAnimations > 0)
                            {
                                yield return new WaitForSeconds(delayBetweenAnimations);
                            }
                        }
                    }
                }
                
                // Play bump animation
                if (targetGroup.bumpAnimator != null)
                {
                    yield return StartCoroutine(PlaySingleBumpAnimation(targetGroup.bumpAnimator, level));
                }
            }
        }
        
        Debug.Log($"SimpleAnimationSequencer: All animations complete up to level {maxLevel}!");
        OnSequenceComplete?.Invoke();
    }
    
    private IEnumerator PlaySingleLineAnimation(Animator animator)
    {
        // Start the line drawing animation
        animator.speed = 1f; // Make sure it's not paused
        animator.Play(lineAnimationStateName, 0, 0f); // Play from start
        
        // Wait for the animation to finish - improved timing to prevent jittering
        yield return new WaitForEndOfFrame(); // Wait one frame for the state to start
        yield return new WaitForEndOfFrame(); // Extra frame to ensure state is active
        
        // Wait until animation is truly complete
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(lineAnimationStateName) && 
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.98f) // Stop slightly before 1.0 to prevent jitter
        {
            yield return null;
        }
        
        // Wait one more frame to ensure animation is fully complete
        yield return new WaitForEndOfFrame();
        
        // After animation completes, try to go to drawn state (if it exists)
        if (animator.HasState(0, Animator.StringToHash(lineDrawnStateName)))
        {
            animator.Play(lineDrawnStateName, 0, 1f); // Play at end of animation (1f) to avoid reset
            Debug.Log($"Line animation complete on {animator.name}, now in drawn state");
        }
        else
        {
            // If drawn state doesn't exist, just stay where we are (animation completed)
            Debug.Log($"Line animation complete on {animator.name}, {lineDrawnStateName} state not found - animation stays completed");
        }
    }
    
    private IEnumerator PlaySingleBumpAnimation(Animator animator, int levelNumber)
    {
        // Start the bump drawing animation
        animator.speed = 1f; // Make sure it's not paused
        animator.Play(bumpAnimationStateName, 0, 0f); // Play from start
        
        // Wait for the animation to finish
        yield return new WaitForEndOfFrame(); // Wait one frame for the state to start
        
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(bumpAnimationStateName) && 
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null; // Wait until animation completes
        }
        
        // After animation completes, try to go to drawn state (if it exists)
        if (animator.HasState(0, Animator.StringToHash(bumpDrawnStateName)))
        {
            animator.Play(bumpDrawnStateName, 0, 0f);
            Debug.Log($"Bump animation complete on {animator.name}, now in drawn state");
        }
        else
        {
            // If drawn state doesn't exist, just stay where we are (animation completed)
            Debug.Log($"Bump animation complete on {animator.name}, {bumpDrawnStateName} state not found - animation stays completed");
        }
        
        // After bump animation completes, just update colors (fading will happen during line drawing)
        if (GameManager.Instance != null)
        {
            int currentLevel = GameManager.Instance.CurrentLevel;
            Debug.Log($"Level {levelNumber} bump animation complete - updating all bump colors for new current level {currentLevel}");
            
            // Update all colors to their correct state (completed level will be green)
            UpdateBumpColors();
            // Note: Next level color fade will happen simultaneously with line drawing for smoother flow
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
    

    

    
    /// <summary>
    /// Update bump colors based on current level status
    /// </summary>
    private void UpdateBumpColors()
    {
        if (GameManager.Instance == null || levelGroups == null)
        {
            Debug.LogWarning("SimpleAnimationSequencer: Cannot update bump colors - GameManager or levelGroups missing");
            return;
        }
        
        int currentLevel = GameManager.Instance.CurrentLevel;
        int coloredCount = 0;
        
        foreach (var group in levelGroups)
        {
            if (group?.bumpAnimator != null)
            {
                // Get the SpriteRenderer from the PARENT transform (not the animator's GameObject)
                SpriteRenderer spriteRenderer = group.bumpAnimator.transform.parent?.GetComponent<SpriteRenderer>();
                if (spriteRenderer != null)
                {
                    Color targetColor;
                    string statusText;
                    
                    if (group.levelNumber < currentLevel)
                    {
                        // Completed level - done color
                        targetColor = doneColor;
                        statusText = "done";
                    }
                    else if (group.levelNumber == currentLevel)
                    {
                        // Current available level - unlocked color
                        targetColor = unlockedColor;
                        statusText = "unlocked";
                    }
                    else
                    {
                        // Future level - locked color
                        targetColor = lockedColor;
                        statusText = "locked";
                    }
                    
                    spriteRenderer.color = targetColor;
                    coloredCount++;
                    Debug.Log($"Set bump {group.bumpAnimator.name} parent (Level {group.levelNumber}) to {statusText} color");
                }
                else
                {
                    Debug.LogWarning($"Bump animator {group.bumpAnimator.name} parent missing SpriteRenderer component");
                }
            }
        }
        
        Debug.Log($"SimpleAnimationSequencer: Updated {coloredCount} bump colors based on current level {currentLevel}");
    }
    
    /// <summary>
    /// SEPARATE LEVEL 5 COMPLETION LOGIC - Set Level 5 bump to done color when completed
    /// </summary>
    private void SetLevel5BumpToDoneColor()
    {
        if (GameManager.Instance == null || levelGroups == null) return;
        
        // Find Level 5 group
        LevelAnimationGroup level5Group = null;
        foreach (var group in levelGroups)
        {
            if (group?.levelNumber == 5)
            {
                level5Group = group;
                break;
            }
        }
        
        if (level5Group?.bumpAnimator != null)
        {
            SpriteRenderer spriteRenderer = level5Group.bumpAnimator.transform.parent?.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.color = doneColor;
                Debug.Log($"Set Level 5 bump to done color (separate from main logic)");
            }
        }
    }
    
    /// <summary>
    /// Fade bump color from one color to another
    /// </summary>
    private IEnumerator FadeBumpColor(Animator bumpAnimator, Color fromColor, Color toColor)
    {
        // Get the SpriteRenderer from the PARENT transform (not the animator's GameObject)
        SpriteRenderer spriteRenderer = bumpAnimator.transform.parent?.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning($"Cannot fade bump color - {bumpAnimator.name} parent missing SpriteRenderer component");
            yield break;
        }
        
        float elapsed = 0f;
        spriteRenderer.color = fromColor;
        
        while (elapsed < colorFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / colorFadeDuration;
            spriteRenderer.color = Color.Lerp(fromColor, toColor, t);
            yield return null;
        }
        
        spriteRenderer.color = toColor;
        Debug.Log($"Bump color fade complete on {bumpAnimator.name}");
    }
    
    /// <summary>
    /// Find a level group by level number
    /// </summary>
    private LevelAnimationGroup FindLevelGroup(int levelNumber)
    {
        if (levelGroups == null) return null;
        
        foreach (var group in levelGroups)
        {
            if (group != null && group.levelNumber == levelNumber)
            {
                return group;
            }
        }
        return null;
    }
    

    
    /// <summary>
    /// Play final completion visuals after the last level is completed
    /// </summary>
    private IEnumerator PlayFinalCompletionVisuals()
    {
        Debug.Log("SimpleAnimationSequencer: Starting final completion visuals...");
        
        // SEPARATE LEVEL 5 LOGIC: Set Level 5 bump to done color when final visuals start
        SetLevel5BumpToDoneColor();
        
        // Mark Level 5 as fully completed - this disables further interaction with Level 5 bump
        if (GameManager.Instance != null)
        {
            GameManager.Instance.MarkFinalLevelFullyCompleted();
        }
        
        if (finalCompletionSprites == null || finalCompletionSprites.Length == 0)
        {
            Debug.LogWarning("SimpleAnimationSequencer: No final completion sprites assigned! Add sprites to Final Completion Sprites array.");
            yield break;
        }
        
        Debug.Log($"SimpleAnimationSequencer: Playing final completion visuals ({finalCompletionSprites.Length} sprites)");
        
        // Play each completion sprite animation sequentially
        for (int i = 0; i < finalCompletionSprites.Length; i++)
        {
            if (finalCompletionSprites[i] != null)
            {
                Debug.Log($"SimpleAnimationSequencer: Playing final completion sprite {i + 1}/{finalCompletionSprites.Length} ({finalCompletionSprites[i].name})");
                yield return StartCoroutine(PlaySingleLineAnimation(finalCompletionSprites[i]));
            }
            else
            {
                Debug.LogWarning($"SimpleAnimationSequencer: Final completion sprite {i} is null - check Final Completion Sprites array");
            }
        }
        
        Debug.Log("SimpleAnimationSequencer: Final completion visuals complete! Game 100% finished! 🎉");
    }
    
    /// <summary>
    /// Wait for level lines to complete drawing (used for parallel execution)
    /// </summary>
    private IEnumerator WaitForLevelLinesComplete(int levelNumber)
    {
        LevelAnimationGroup targetGroup = FindLevelGroup(levelNumber);
        if (targetGroup?.lineAnimators == null)
        {
            yield break;
        }
        
        // Wait for all line animations to complete
        bool allComplete = false;
        while (!allComplete)
        {
            allComplete = true;
            foreach (var animator in targetGroup.lineAnimators)
            {
                if (animator != null)
                {
                    var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName(lineAnimationStateName) && stateInfo.normalizedTime < 0.98f)
                    {
                        allComplete = false;
                        break;
                    }
                }
            }
            yield return null;
        }
        
        Debug.Log($"SimpleAnimationSequencer: Level {levelNumber} lines finished drawing");
    }
    

}
