using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Effect script for the Dash item
/// Allows player to perform a forward dash when active
/// Only works in WorldMenu and Level scenes
/// </summary>
public class DashEffect : MonoBehaviour
{
    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 10f;
    [SerializeField] private float dashCooldown = 1f;
    [SerializeField] private KeyCode dashKey = KeyCode.LeftShift;
    [SerializeField] private bool debugMode = false;

    private bool effectActive = false;
    private float lastDashTime = 0f;

    private void OnEnable()
    {
        if (IsValidScene())
        {
            effectActive = true;
            
            if (debugMode)
            {
                Debug.Log($"DashEffect: Activated with {dashForce} dash force, {dashCooldown}s cooldown in scene '{SceneManager.GetActiveScene().name}'");
            }
        }
    }

    private void OnDisable()
    {
        effectActive = false;
        
        if (debugMode)
        {
            Debug.Log("DashEffect: Deactivated");
        }
    }

    private bool IsValidScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "WorldMenu" || sceneName.StartsWith("Level");
    }

    private void Update()
    {
        if (!effectActive || !IsValidScene()) return;

        // Check for dash input
        if (Input.GetKeyDown(dashKey) && CanDash())
        {
            PerformDash();
        }
    }

    private bool CanDash()
    {
        return Time.time >= lastDashTime + dashCooldown;
    }

    private void PerformDash()
    {
        lastDashTime = Time.time;
        
        if (debugMode)
        {
            Debug.Log("DashEffect: Performing dash!");
        }
        
        // TODO: Apply dash force to player
        // Example implementation:
        // - Find player rigidbody
        // - Apply forward force in the direction player is facing
        // - Consider adding dash animation/effect
        
        // Placeholder for actual implementation:
        // Rigidbody playerRb = FindObjectOfType<PlayerController>()?.GetComponent<Rigidbody>();
        // if (playerRb != null)
        // {
        //     Vector3 dashDirection = transform.forward; // or player's forward direction
        //     playerRb.AddForce(dashDirection * dashForce, ForceMode.Impulse);
        // }
    }

    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    public float GetRemainingCooldown()
    {
        if (!effectActive) return 0f;
        return Mathf.Max(0f, (lastDashTime + dashCooldown) - Time.time);
    }

    /// <summary>
    /// Debug method to manually test dash
    /// </summary>
    [ContextMenu("Test Dash")]
    public void TestDash()
    {
        if (effectActive && CanDash())
        {
            PerformDash();
        }
        else if (effectActive)
        {
            Debug.Log($"DashEffect: Dash on cooldown, {GetRemainingCooldown():F1}s remaining");
        }
        else
        {
            Debug.Log("DashEffect: Effect not active");
        }
    }
}
