using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AnimationStep
{
    public Animator animator;
    public string animationTrigger = "Play";
    public float duration = 1f;
    
    [Header("Optional")]
    public bool waitForAnimationComplete = true;
    public string completeStateName = ""; // Leave empty to use duration
}

public class AnimationSequencer : MonoBehaviour
{
    [Header("Animation Sequence")]
    [SerializeField] private List<AnimationStep> animationSteps = new List<AnimationStep>();
    
    [Header("Settings")]
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool loop = false;
    
    // Events
    public System.Action OnSequenceComplete;
    public System.Action<int> OnStepComplete;
    
    private Coroutine currentSequence;
    private bool isPlaying = false;
    
    public bool IsPlaying => isPlaying;
    public int CurrentStep { get; private set; } = -1;
    
    private void Start()
    {
        if (playOnStart)
        {
            PlaySequence();
        }
    }
    
    public void PlaySequence()
    {
        if (isPlaying) return;
        
        currentSequence = StartCoroutine(PlaySequenceCoroutine());
    }
    
    public void PlaySequenceToStep(int targetStep)
    {
        if (isPlaying) return;
        
        currentSequence = StartCoroutine(PlaySequenceToStepCoroutine(targetStep));
    }
    
    public void StopSequence()
    {
        if (currentSequence != null)
        {
            StopCoroutine(currentSequence);
            currentSequence = null;
        }
        isPlaying = false;
    }
    
    public void ResetSequence()
    {
        StopSequence();
        CurrentStep = -1;
    }
    
    private IEnumerator PlaySequenceCoroutine()
    {
        isPlaying = true;
        CurrentStep = -1;
        
        do
        {
            for (int i = 0; i < animationSteps.Count; i++)
            {
                CurrentStep = i;
                yield return PlayStep(animationSteps[i]);
                OnStepComplete?.Invoke(i);
            }
        } while (loop);
        
        isPlaying = false;
        OnSequenceComplete?.Invoke();
    }
    
    private IEnumerator PlaySequenceToStepCoroutine(int targetStep)
    {
        isPlaying = true;
        int startStep = CurrentStep + 1;
        
        for (int i = startStep; i <= targetStep && i < animationSteps.Count; i++)
        {
            CurrentStep = i;
            yield return PlayStep(animationSteps[i]);
            OnStepComplete?.Invoke(i);
        }
        
        isPlaying = false;
        OnSequenceComplete?.Invoke();
    }
    
    private IEnumerator PlayStep(AnimationStep step)
    {
        if (step.animator == null) yield break;
        
        // Trigger the animation
        step.animator.SetTrigger(step.animationTrigger);
        
        if (step.waitForAnimationComplete && !string.IsNullOrEmpty(step.completeStateName))
        {
            // Wait for specific animation state to complete
            yield return WaitForAnimationState(step.animator, step.completeStateName);
        }
        else
        {
            // Wait for specified duration
            yield return new WaitForSeconds(step.duration);
        }
    }
    
    private IEnumerator WaitForAnimationState(Animator animator, string stateName)
    {
        // Wait one frame for the transition to start
        yield return null;
        
        // Wait for the animation to start
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            yield return null;
        }
        
        // Wait for the animation to complete
        while (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) && 
               animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
    }
    
    // Editor helper methods
    [ContextMenu("Play Sequence")]
    public void EditorPlaySequence()
    {
        PlaySequence();
    }
    
    [ContextMenu("Stop Sequence")]
    public void EditorStopSequence()
    {
        StopSequence();
    }
}
