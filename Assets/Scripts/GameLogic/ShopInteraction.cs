using UnityEngine;

public class ShopInteraction : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    
    [Header("UI Settings")]
    [SerializeField] private Texture2D promptSprite;
    [SerializeField] private float verticalOffset = 1.5f; // Offset above player's head
    
    private Transform player;
    private bool playerInRange = false;
    private bool showPrompt = false;
    private ItemsManager itemsManager;
    private bool keyProcessedThisFrame = false;
    
    // Events
    public System.Action OnShopOpened;
    public System.Action OnShopClosed;
    
    private void Start()
    {
        // Find player by tag or layer
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log($"ShopInteraction: Found player: {playerObj.name}");
        }
        else
        {
            Debug.LogWarning($"ShopInteraction: Player with 'Player' tag not found! Make sure your player GameObject has the 'Player' tag.");
        }
        
        // Find ItemsManager
        itemsManager = FindObjectOfType<ItemsManager>();
        if (itemsManager == null)
        {
            Debug.LogError("ShopInteraction: ItemsManager not found! Make sure there's an ItemsManager in the scene.");
        }
        else
        {
            Debug.Log($"ShopInteraction: Found ItemsManager on GameObject: {itemsManager.gameObject.name}");
        }
    }
    
    private void Update()
    {
        keyProcessedThisFrame = false; // Reset flag each frame
        CheckPlayerProximity();
        HandleInput();
    }
    
    private void CheckPlayerProximity()
    {
        if (player == null) return;
        
        float distance = Vector3.Distance(transform.position, player.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionRange;
        
        // Update prompt visibility - always show when in range
        showPrompt = playerInRange;
        
        // Optional: Trigger events when entering/exiting range
        if (playerInRange && !wasInRange)
        {
            OnPlayerEnterRange();
            Debug.Log("ShopInteraction: Player entered shop range");
        }
        else if (!playerInRange && wasInRange)
        {
            OnPlayerExitRange();
            Debug.Log("ShopInteraction: Player exited shop range");
        }
    }
    
    private void HandleInput()
    {
        // Handle shop opening when in range
        if (showPrompt && Input.GetKeyDown(interactionKey) && !IsShopOpen() && !keyProcessedThisFrame)
        {
            OpenShop();
            keyProcessedThisFrame = true;
            return; // Exit early to prevent closing in same frame
        }
        
        // Handle shop closing when shop is open
        if (IsShopOpen() && (Input.GetKeyDown(interactionKey) || Input.GetKeyDown(KeyCode.Escape)) && !keyProcessedThisFrame)
        {
            CloseShop();
            keyProcessedThisFrame = true;
        }
    }
    
    private void OpenShop()
    {
        if (itemsManager != null)
        {
            Debug.Log($"ShopInteraction: Setting shopIsOpen to true on {itemsManager.gameObject.name}");
            itemsManager.shopIsOpen = true;
            OnShopOpened?.Invoke();
            Debug.Log("ShopInteraction: Shop opened");
            
            // Pause the game via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.PauseGame();
            }
            else
            {
                Debug.LogError("ShopInteraction: GameManager.Instance is null!");
            }
        }
        else
        {
            Debug.LogError("ShopInteraction: Cannot open shop - itemsManager is null!");
        }
    }
    
    private void CloseShop()
    {
        if (itemsManager != null)
        {
            itemsManager.shopIsOpen = false;
            OnShopClosed?.Invoke();
            Debug.Log("ShopInteraction: Shop closed");
            
            // Resume the game via GameManager
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResumeGame();
            }
        }
    }
    
    private bool IsShopOpen()
    {
        return itemsManager != null && itemsManager.shopIsOpen;
    }
    
    private void OnPlayerEnterRange()
    {
        // Optional: Add enter range effects (sound, animation, etc.)
    }
    
    private void OnPlayerExitRange()
    {
        // Optional: Add exit range effects
        // Optionally close shop when player leaves range
        if (IsShopOpen())
        {
            CloseShop();
        }
    }
    
    private void OnGUI()
    {
        // Only show prompt when in range and shop is closed
        if (!showPrompt || promptSprite == null || player == null || IsShopOpen()) return;
        
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
        
        // Optional: Draw interaction key hint
        string keyHint = $"Open shop";
        GUIStyle hintStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };
        
        Rect hintRect = new Rect(x - 50f, y + spriteHeight + 5f, spriteWidth + 100f, 20f);
        GUI.Label(hintRect, keyHint, hintStyle);
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw interaction range in scene view
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
    
    // Public methods for external access
    public bool IsPlayerInRange => playerInRange;
    public bool CanInteract => showPrompt && !IsShopOpen();
    public float InteractionRange => interactionRange;
    public KeyCode InteractionKey => interactionKey;
}
