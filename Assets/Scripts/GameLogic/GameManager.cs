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
        GetComponent<PostGameSummary>().showSummary = false;

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

    [ContextMenu("Win Level")]
    public void Win()
    {
        float timeLeft = GetComponent<LevelManager>().LevelCompleteTime();

        // Display post-game summary and update money
        StartCoroutine(PostGameSummaryCoroutine(true, timeLeft));
    }

    [ContextMenu("Die")]
    public void Die()
    {
        // Display post-game summary and update money
        StartCoroutine(PostGameSummaryCoroutine(false, 0));
    }

    private IEnumerator PostGameSummaryCoroutine(bool win, float timeLeft)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("PostGameSummary");
        int coinsDelta = GetComponent<PostGameSummary>().ComputeSummary(win, timeLeft);

        // Wait a bit then when summary has displayed for a bit update money, if not bankrupt then return to worldmenu otherwise bankruptcy is handled on its own
        yield return new WaitForSeconds(2.5f);

        // Update money
        MoneyManager.AlterMoney(coinsDelta);

        // TODO Wait a bit and if the scene wasn't changed to bankruptcy then return to worldmenu
        yield return new WaitForSeconds(2.5f);
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Bankruptcy")
        {
            GetComponent<PostGameSummary>().showSummary = false;
            UnityEngine.SceneManagement.SceneManager.LoadScene("WorldMenu");

            // Wait until the scene is fully loaded then complete the level
            yield return null; // Wait one frame to ensure scene is fully loaded and initialized
            yield return new WaitForSeconds(0.5f); // Wait another 0.5 seconds to ensure the scene is fully loaded and initialized
            if (win)
            {
                CompleteLevel(currentLevel);
            }
        }
    }

    private void OnGUI()
    {
        GUILayout.Label($"[Debug] Money: {MoneyManager.Money}");
    }
}
