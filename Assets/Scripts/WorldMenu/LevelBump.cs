using UnityEngine;

public class LevelBump : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelNumber = 1;
    
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.F;
    
    [Header("UI Settings")]
    [SerializeField] private Texture2D promptSprite;
    [SerializeField] private float verticalOffset = 1.5f; // Offset above player's head
    
    [Header("Scene Management")]
    [SerializeField] private string levelSceneName = "";
    
    private Transform player;
    private bool playerInRange = false;
    private bool showPrompt = false;
    
    // Events
    public System.Action<int> OnLevelStartRequested;
    
    private void Start()
    {
        // Find player by tag or layer
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"LevelBump {levelNumber}: Found player: {playerObj.name}");
        }
        else
        {
            Debug.LogWarning($"LevelBump {levelNumber}: Player with 'Player' tag not found! Make sure your player GameObject has the 'Player' tag.");
        }
        
        // Debug current level at startup
        if (GameManager.Instance != null)
        {
            Debug.Log($"LevelBump {levelNumber}: GameManager current level is {GameManager.Instance.CurrentLevel}");
        }
        
        // Subscribe to game manager events
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged += OnLevelChanged;
            OnLevelChanged(GameManager.Instance.CurrentLevel);
        }
    }
    
    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnLevelChanged -= OnLevelChanged;
        }
    }
    
    private void Update()
    {
        CheckPlayerProximity();
        HandleInput();
    }
    
    private void CheckPlayerProximity()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;
        
        // Update prompt visibility
        bool canInteract = GameManager.Instance != null && 
                          GameManager.Instance.CurrentLevel == levelNumber;
        
        // Special case: Level 5 cannot be interacted with if it's fully completed (including final visuals)
        if (levelNumber == 5 && GameManager.Instance != null && GameManager.Instance.IsFinalLevelFullyCompleted)
        {
            canInteract = false;
        }
        
        showPrompt = playerInRange && canInteract;
        
        // Optional: Trigger events when entering/exiting range
        if (playerInRange && !wasInRange)
        {
            OnPlayerEnterRange();
            
            // Debug log when entering range (only once, not every frame)
            int currentLevel = GameManager.Instance?.CurrentLevel ?? -1;
            if (canInteract)
            {
                Debug.Log($"LevelBump {levelNumber}: ✅ Showing interaction sprite (Current level: {currentLevel})");
            }
            else if (levelNumber > currentLevel)
            {
                Debug.Log($"LevelBump {levelNumber}: 🔒 This level is locked (Current level: {currentLevel})");
            }
            else if (levelNumber < currentLevel)
            {
                Debug.Log($"LevelBump {levelNumber}: ✅ This level is already completed (Current level: {currentLevel})");
            }
            else if (levelNumber == 5 && GameManager.Instance.IsFinalLevelFullyCompleted)
            {
                Debug.Log($"LevelBump {levelNumber}: 🏆 Final level fully completed - interaction disabled until reset");
            }
        }
        else if (!playerInRange && wasInRange)
        {
            OnPlayerExitRange();
        }
    }
    
    private void HandleInput()
    {
        if (showPrompt && Input.GetKeyDown(interactionKey))
        {
            StartLevel();
        }
    }
    
    private void StartLevel()
    {
        OnLevelStartRequested?.Invoke(levelNumber);
        
        if (!string.IsNullOrEmpty(levelSceneName))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(levelSceneName);
        }
        else
        {
            Debug.Log($"Starting Level {levelNumber}");
        }

        GameManager.Instance.LevelStarted();
    }
    
    private void OnLevelChanged(int currentLevel)
    {
        // Level status changed - colors are handled by SimpleAnimationSequencer
        // You can add other visual feedback here (e.g., enable/disable glow, particles, etc.)
    }
    
    private void OnPlayerEnterRange()
    {
        // Optional: Add enter range effects (sound, animation, etc.)
    }
    
    private void OnPlayerExitRange()
    {
        // Optional: Add exit range effects
    }
    
    private void OnGUI()
    {
        if (!showPrompt || promptSprite == null || player == null) return;
        
        // Calculate position above player's head
        Vector3 playerWorldPos = player.position + Vector3.up * verticalOffset;
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(playerWorldPos);
        
        // Check if position is visible on screen
        if (playerScreenPos.z < 0) return; // Behind camera
        
        // Calculate sprite dimensions (half size)
        float spriteWidth = promptSprite.width * 0.5f;
        float spriteHeight = promptSprite.height * 0.5f;
        
        // Center the sprite above player's head (GUI coordinates have Y=0 at top)
        float x = playerScreenPos.x - spriteWidth * 0.5f;
        float y = Screen.height - playerScreenPos.y - spriteHeight * 0.5f;
        
        Rect spriteRect = new Rect(x, y, spriteWidth, spriteHeight);
        
        // Draw the sprite texture
        GUI.DrawTexture(spriteRect, promptSprite);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw interaction range in scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    
    // Public methods for external access
    public bool IsPlayerInRange => playerInRange;
    public int LevelNumber => levelNumber;
    public bool CanInteract => showPrompt;
}
