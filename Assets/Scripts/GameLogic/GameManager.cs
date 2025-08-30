using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevels = 5;
    
    private bool isFinalLevelFullyCompleted = false; // Tracks if Level 5 + final visuals are complete
    
    public int CurrentLevel => currentLevel;
    public int MaxLevels => maxLevels;
    public bool IsLevelUnlocked(int level) => level <= currentLevel;
    public bool IsMaxLevelReached => currentLevel >= maxLevels;
    public bool IsFinalLevelFullyCompleted => isFinalLevelFullyCompleted;
    
    // Events
    public System.Action<int> OnLevelChanged;
    public System.Action<int> OnLevelCompleted;
    public System.Action OnProgressReset;
    
    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void CompleteLevel(int level)
    {
        if (level == currentLevel && level <= maxLevels)
        {
            if (level < maxLevels)
            {
                currentLevel++;
            }
            OnLevelCompleted?.Invoke(level);
            if (level < maxLevels)
            {
                OnLevelChanged?.Invoke(currentLevel);
            }
            Debug.Log($"GameManager: Completed level {level}, current level is now {currentLevel}");
        }
    }
    
    public void SetCurrentLevel(int level)
    {
        if (level >= 1 && level <= maxLevels)
        {
            currentLevel = level;
            OnLevelChanged?.Invoke(currentLevel);
        }
    }
    
    // Debug methods for testing
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        currentLevel = 1;
        isFinalLevelFullyCompleted = false; // Reset final completion flag
        
        // Notify listeners that progress has been reset
        OnProgressReset?.Invoke();
        OnLevelChanged?.Invoke(currentLevel);
        
        Debug.Log("GameManager: Progress reset to level 1");
    }
    
    /// <summary>
    /// Mark Level 5 as fully completed (including final visuals) - prevents further interaction
    /// </summary>
    public void MarkFinalLevelFullyCompleted()
    {
        isFinalLevelFullyCompleted = true;
        Debug.Log("GameManager: Final level fully completed - Level 5 interaction disabled");
    }
    
    [ContextMenu("Complete Current Level")]
    public void DebugCompleteCurrentLevel()
    {
        CompleteLevel(currentLevel);
    }

    #region Bankrupt

    [ContextMenu("Trigger Bankruptcy")]
    public void Bankrupt() {
        ResetProgress();
        MoneyManager.ResetMoney();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Bankruptcy");
    }

    #endregion

    /// <summary>
    ///  Called when a level is started from a levelbump
    /// </summary>
    public void LevelStarted()
    {
        GetComponent<LevelManager>().StartNewLevel(300); // TODO un-hardcode the level duration and adapt it to the current level
    }

    public void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PostGameSummary");
        int coinsDelta = GetComponent<PostGameSummary>().ComputeSummary(false, 0);

        // Wait a bit then when summary has displayed for a bit update money, if not bankrupt then return to worldmenu otherwise bankruptcy is handled on its own
        yield return new WaitForSeconds(5f);

        // Update money
        MoneyManager.AlterMoney(coinsDelta);
    }
}
