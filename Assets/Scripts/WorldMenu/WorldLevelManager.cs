using UnityEngine;
using HFSM;

public class WorldLevelManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AnimationSequencer progressAnimationSequencer;
    [SerializeField] private LevelBump[] levelBumps = new LevelBump[5];
    
    [Header("Settings")]
    [SerializeField] private float animationDelay = 0.5f;
    
    // State Machine
    private StateMachine fsm;
    
    // States
    private enum States
    {
        Initialize,
        Idle,
        PlayingProgressAnimation,
        PlayerInteracting
    }
    
    // Events
    public System.Action OnInitializationComplete;
    public System.Action OnProgressAnimationComplete;
    
    private void Start()
    {
        InitializeStateMachine();
        SetupEventListeners();
        fsm.Start();
    }
    
    private void OnDestroy()
    {
        CleanupEventListeners();
        fsm?.Stop();
    }
    
    private void Update()
    {
        fsm?.Update();
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
        
        // Transitions
        fsm.AddTransition(States.Initialize, States.Idle, 
            trigger: "InitComplete");
        
        fsm.AddTransition(States.Idle, States.PlayingProgressAnimation, 
            trigger: "PlayProgress");
        
        fsm.AddTransition(States.PlayingProgressAnimation, States.Idle, 
            trigger: "ProgressComplete");
        
        fsm.AddTransition(States.Idle, States.PlayerInteracting, 
            trigger: "StartInteraction");
        
        fsm.AddTransition(States.PlayerInteracting, States.Idle, 
            trigger: "EndInteraction");
        
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
        if (GameManager.Instance != null)
        {
            int currentLevel = GameManager.Instance.CurrentLevel;
            
            // Play progress animation up to current level (instant or delayed)
            if (currentLevel > 1 && progressAnimationSequencer != null)
            {
                // Show progress immediately for levels already completed
                progressAnimationSequencer.PlaySequenceToStep(currentLevel - 2);
            }
        }
        
        // Complete initialization
        Invoke(nameof(CompleteInitialization), 0.1f);
    }
    
    private void CompleteInitialization()
    {
        fsm.Trigger("InitComplete");
        OnInitializationComplete?.Invoke();
    }
    
    private void PlayProgressAnimation()
    {
        if (progressAnimationSequencer != null && GameManager.Instance != null)
        {
            int targetStep = GameManager.Instance.CurrentLevel - 2;
            if (targetStep >= 0)
            {
                progressAnimationSequencer.PlaySequenceToStep(targetStep);
            }
            else
            {
                // No animation needed, complete immediately
                OnAnimationSequenceComplete();
            }
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
        if (fsm.ActiveStateName == States.Idle.ToString())
        {
            fsm.Trigger("PlayProgress");
        }
    }
    
    private void OnLevelCompleted(int completedLevel)
    {
        Debug.Log($"WorldLevelManager: Level {completedLevel} completed");
        
        // Play progress animation for the newly unlocked level
        if (fsm.ActiveStateName == States.Idle.ToString())
        {
            fsm.Trigger("PlayProgress");
        }
    }
    
    private void OnAnimationSequenceComplete()
    {
        Debug.Log("WorldLevelManager: Progress animation complete");
        
        if (fsm.ActiveStateName == States.PlayingProgressAnimation.ToString())
        {
            fsm.Trigger("ProgressComplete");
        }
        
        OnProgressAnimationComplete?.Invoke();
    }
    
    private void OnLevelStartRequested(int levelNumber)
    {
        Debug.Log($"WorldLevelManager: Level {levelNumber} start requested");
        
        if (fsm.ActiveStateName == States.Idle.ToString())
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
        if (fsm.ActiveStateName == States.PlayerInteracting.ToString())
        {
            fsm.Trigger("EndInteraction");
        }
    }
    
    // Public interface
    public bool IsInitialized => fsm.ActiveStateName != States.Initialize.ToString();
    public bool IsPlayingAnimation => fsm.ActiveStateName == States.PlayingProgressAnimation.ToString();
    public string CurrentState => fsm.ActiveStateName;
    
    // Debug methods
    [ContextMenu("Force Progress Animation")]
    public void ForceProgressAnimation()
    {
        if (fsm.ActiveStateName == States.Idle.ToString())
        {
            fsm.Trigger("PlayProgress");
        }
    }
    
    [ContextMenu("Reset World")]
    public void ResetWorld()
    {
        if (progressAnimationSequencer != null)
        {
            progressAnimationSequencer.ResetSequence();
        }
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResetProgress();
        }
    }
}
