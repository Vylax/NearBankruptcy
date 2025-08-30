using UnityEngine;
using UnityHFSM;
using System.Collections;

public class WorldLevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SimpleAnimationSequencer progressAnimationSequencer;
    [SerializeField] private LevelBump[] levelBumps = new LevelBump[5];
    
    // State Machine
    private StateMachine fsm;
    private int currentCompletedLevel = -1; // Store the level that was just completed
    
    // States
    private static class States
    {
        public const string Initialize = "Initialize";
        public const string Idle = "Idle";
        public const string PlayingProgressAnimation = "PlayingProgressAnimation";
        public const string PlayerInteracting = "PlayerInteracting";
    }
    

    
    private void Start()
    {
        InitializeStateMachine();
        SetupEventListeners();
        // Wait for scene to be fully loaded before initializing
        StartCoroutine(WaitForSceneLoadAndInitialize());
    }
    
    /// <summary>
    /// Wait for the scene to be fully loaded and rendered before initializing the world state
    /// </summary>
    private IEnumerator WaitForSceneLoadAndInitialize()
    {
        // Wait for end of frame to ensure all GameObjects are initialized
        yield return new WaitForEndOfFrame();
        
        // Wait an additional frame to ensure all Start() methods have been called
        yield return new WaitForEndOfFrame();
        
        // Wait a bit more to ensure scene is fully rendered and stable
        //yield return new WaitForSeconds(0.2f);
        
        // Now it's safe to initialize the state machine
        fsm.Init();
    }
    
    private void OnDestroy()
    {
        CleanupEventListeners();
        // fsm?.Stop(); // UnityHFSM doesn't have a Stop method
    }
    
    private void Update()
    {
        fsm?.OnLogic();
    }
    
    private void InitializeStateMachine()
    {
        fsm = new StateMachine();
        
        // Initialize State
        fsm.AddState(States.Initialize, onEnter: state =>
        {
            Debug.Log("WorldLevelManager: Initializing...");
            InitializeWorld();
        });
        
        // Idle State - Normal gameplay state
        fsm.AddState(States.Idle, onEnter: state =>
        {
            Debug.Log("WorldLevelManager: Entering Idle state");
        });
        
        // Playing Progress Animation State
        fsm.AddState(States.PlayingProgressAnimation, onEnter: state =>
        {
            Debug.Log("WorldLevelManager: Playing progress animation");
            PlayProgressAnimation();
        });
        
        // Player Interacting State
        fsm.AddState(States.PlayerInteracting, onEnter: state =>
        {
            Debug.Log("WorldLevelManager: Player interacting");
        });
        
        // Transitions using shortcut methods
        fsm.AddTriggerTransition("InitComplete", States.Initialize, States.Idle);
        fsm.AddTriggerTransition("PlayProgress", States.Idle, States.PlayingProgressAnimation);
        fsm.AddTriggerTransition("ProgressComplete", States.PlayingProgressAnimation, States.Idle);
        fsm.AddTriggerTransition("StartInteraction", States.Idle, States.PlayerInteracting);
        fsm.AddTriggerTransition("EndInteraction", States.PlayerInteracting, States.Idle);
        
        // Set initial state
        fsm.SetStartState(States.Initialize);
    }
    
    private void SetupEventListeners()
    {
        // Listen to GameManager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged += OnLevelChanged;
            GameManager.Instance.OnLevelCompleted += OnLevelCompleted;
            GameManager.Instance.OnProgressReset += OnProgressReset;
        }
        
        // Listen to animation sequencer events
        if (progressAnimationSequencer != null)
        {
            progressAnimationSequencer.OnSequenceComplete += OnAnimationSequenceComplete;
        }
        
        // Listen to level bump events
        foreach (var levelBump in levelBumps)
        {
            if (levelBump != null)
            {
                levelBump.OnLevelStartRequested += OnLevelStartRequested;
            }
        }
    }
    
    private void CleanupEventListeners()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged -= OnLevelChanged;
            GameManager.Instance.OnLevelCompleted -= OnLevelCompleted;
            GameManager.Instance.OnProgressReset -= OnProgressReset;
        }
        
        if (progressAnimationSequencer != null)
        {
            progressAnimationSequencer.OnSequenceComplete -= OnAnimationSequenceComplete;
        }
        
        foreach (var levelBump in levelBumps)
        {
            if (levelBump != null)
            {
                levelBump.OnLevelStartRequested -= OnLevelStartRequested;
            }
        }
    }
    
        private void InitializeWorld()
    {
        // Setup initial world state based on current progress
        if (GameManager.Instance != null && progressAnimationSequencer != null)
        {
            int currentLevel = GameManager.Instance.CurrentLevel;

            // Show existing progress immediately (without animation) for completed levels
            if (currentLevel > 1)
            {
                int completedLevels = currentLevel - 1;
                progressAnimationSequencer.SetLevelsToDrawnState(completedLevels);
                Debug.Log($"World initialized - showing existing progress for completed levels 1-{completedLevels}");
                
                // Also set current level lines to drawn (they should have been drawn during previous level completion)
                progressAnimationSequencer.SetCurrentLevelLinesToDrawnState(currentLevel);
                Debug.Log($"World initialized - current level {currentLevel} lines shown as drawn (were drawn when previous level completed)");
            }
            else if (currentLevel == 1)
            {
                // For level 1, always check if lines should be drawn with delay (only first time)
                Debug.Log("World initialized - on level 1, checking if lines need to be drawn");
                progressAnimationSequencer.DrawCurrentLevelLinesWithDelay();
            }
        }

        // Complete initialization after a brief delay to ensure everything is ready
        Invoke(nameof(CompleteInitialization), 0.3f);
    }
    
    private void CompleteInitialization()
    {
        fsm.Trigger("InitComplete");
    }
    
    private void PlayProgressAnimation()
    {
        if (progressAnimationSequencer != null && currentCompletedLevel >= 1)
        {
            // Play animation for the level that was just completed
            progressAnimationSequencer.PlayLevelCompletionAnimation(currentCompletedLevel);
        }
        else
        {
            OnAnimationSequenceComplete();
        }
    }
    
    // Event Handlers
    private void OnLevelChanged(int newLevel)
    {
        Debug.Log($"WorldLevelManager: Level changed to {newLevel}");
        
        // Trigger progress animation if we're in idle state
        if (fsm.ActiveStateName == States.Idle)
        {
            fsm.Trigger("PlayProgress");
        }
    }
    
    private void OnLevelCompleted(int completedLevel)
    {
        Debug.Log($"WorldLevelManager: Level {completedLevel} completed");
        
        // Store the completed level for use in PlayProgressAnimation
        currentCompletedLevel = completedLevel;
        
        // Play progress animation for the newly unlocked level
        if (fsm.ActiveStateName == States.Idle)
        {
            fsm.Trigger("PlayProgress");
        }
    }
    
    private void OnAnimationSequenceComplete()
    {
        Debug.Log("WorldLevelManager: Progress animation complete");
        
        if (fsm.ActiveStateName == States.PlayingProgressAnimation)
        {
            fsm.Trigger("ProgressComplete");
        }
    }
    
    private void OnLevelStartRequested(int levelNumber)
    {
        Debug.Log($"WorldLevelManager: Level {levelNumber} start requested");
        
        if (fsm.ActiveStateName == States.Idle)
        {
            fsm.Trigger("StartInteraction");
            
            // Handle level transition logic here
            HandleLevelStart(levelNumber);
        }
    }
    
    private void HandleLevelStart(int levelNumber)
    {
        // Validate level can be started
        if (GameManager.Instance != null && 
            GameManager.Instance.CurrentLevel == levelNumber)
        {
            Debug.Log($"Starting level {levelNumber}");
            
            // Additional logic before level starts can go here
            // The actual scene loading is handled by LevelBump script
            
            // Return to idle after a brief moment
            Invoke(nameof(ReturnToIdle), 0.5f);
        }
        else
        {
            Debug.LogWarning($"Cannot start level {levelNumber}. Current level is {GameManager.Instance?.CurrentLevel}");
            ReturnToIdle();
        }
    }
    
    private void ReturnToIdle()
    {
        if (fsm.ActiveStateName == States.PlayerInteracting)
        {
            fsm.Trigger("EndInteraction");
        }
    }
    
    private void OnProgressReset()
    {
        Debug.Log("WorldLevelManager: Progress reset - resetting animation states and drawing level 1");
        
        // RESET THE COMPLETED LEVEL TRACKING - this is crucial!
        currentCompletedLevel = -1;
        
        // Reset all animation states back to idle and immediately start level 1 drawing
        if (progressAnimationSequencer != null)
        {
            progressAnimationSequencer.ResetSequence();
        }
    }
    
    // Public interface
    public bool IsInitialized => fsm.ActiveStateName != States.Initialize;
    public bool IsPlayingAnimation => fsm.ActiveStateName == States.PlayingProgressAnimation;
    public string CurrentState => fsm.ActiveStateName;
    
    // Debug methods
    [ContextMenu("Force Progress Animation")]
    public void ForceProgressAnimation()
    {
        if (fsm.ActiveStateName == States.Idle)
        {
            fsm.Trigger("PlayProgress");
        }
    }
    
    [ContextMenu("Show Current Progress")]
    public void ShowCurrentProgress()
    {
        if (GameManager.Instance != null && progressAnimationSequencer != null)
        {
            int currentLevel = GameManager.Instance.CurrentLevel;
            if (currentLevel > 1)
            {
                // Show all progress up to the current level
                progressAnimationSequencer.PlayAllAnimationsUpToLevel(currentLevel - 1);
            }
        }
    }
    

}
