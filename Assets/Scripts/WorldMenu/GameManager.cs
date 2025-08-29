using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game State")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int maxLevels = 5;
    
    public int CurrentLevel => currentLevel;
    public int MaxLevels => maxLevels;
    public bool IsLevelUnlocked(int level) => level <= currentLevel;
    public bool IsMaxLevelReached => currentLevel >= maxLevels;
    
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
            LoadGameData();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void CompleteLevel(int level)
    {
        if (level == currentLevel && level < maxLevels)
        {
            currentLevel++;
            OnLevelCompleted?.Invoke(level);
            OnLevelChanged?.Invoke(currentLevel);
            SaveGameData();
        }
    }
    
    public void SetCurrentLevel(int level)
    {
        if (level >= 1 && level <= maxLevels)
        {
            currentLevel = level;
            OnLevelChanged?.Invoke(currentLevel);
            SaveGameData();
        }
    }
    
    private void SaveGameData()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.Save();
    }
    
    private void LoadGameData()
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
    }
    
    // Debug methods for testing
    [ContextMenu("Reset Progress")]
    public void ResetProgress()
    {
        currentLevel = 1;
        PlayerPrefs.DeleteKey("CurrentLevel");
        
        // Notify listeners that progress has been reset
        OnProgressReset?.Invoke();
        OnLevelChanged?.Invoke(currentLevel);
        
        Debug.Log("GameManager: Progress reset to level 1");
    }
    
    [ContextMenu("Complete Current Level")]
    public void DebugCompleteCurrentLevel()
    {
        CompleteLevel(currentLevel);
    }
    
    [ContextMenu("Complete Level 5")]
    public void DebugCompleteLevel5()
    {
        SetCurrentLevel(5);
        CompleteLevel(5);
        Debug.Log("GameManager: Set to level 5 and completed it - should trigger final completion visuals");
    }
}
