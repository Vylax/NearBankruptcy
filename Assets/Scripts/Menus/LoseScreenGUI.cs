using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script uses the OnGUI system to draw navigation buttons like "Play Again" or "Quit".
/// It's designed for end screens (both win and lose) and provides custom styling.
/// Attach this component to an active GameObject in your scene, like the Camera.
/// </summary>
public class LoseScreenGUI : MonoBehaviour
{
    [Tooltip("The text to display on the main action button (e.g., 'Try Again' or 'Play Again').")]
    public string mainButtonText = "Try Again"; // Default text

    [Tooltip("Optional: Assign a custom font for the buttons.")]
    public Font buttonFont;

    private GUIStyle buttonStyle;

    // This is the core of the immediate-mode GUI system.
    // It runs every frame, similar to Update().
    void OnGUI()
    {
        // Initialize the custom style for our buttons if it hasn't been already.
        if (buttonStyle == null)
        {
            CreateButtonStyle();
        }

        // --- Button Layout and Positioning ---
        int buttonWidth = 280;
        int buttonHeight = 60;
        int spacing = 25;

        // Center the buttons on the screen.
        float centerX = Screen.width / 2f;

        // Determine total UI height based on whether the Quit button is visible.
        bool isQuitButtonVisible = IsQuitAvailable();
        float totalUIHeight = isQuitButtonVisible ? (buttonHeight * 2 + spacing) : buttonHeight;

        // Start drawing below the vertical center to leave space for title text.
        float startY = (Screen.height / 2f) - (totalUIHeight / 2f) + 100;

        // --- Draw the Main Action Button ("Play Again" / "Try Again") ---
        Rect mainButtonRect = new Rect(centerX - (buttonWidth / 2), startY, buttonWidth, buttonHeight);

        // Use the public mainButtonText variable here
        if (GUI.Button(mainButtonRect, mainButtonText, buttonStyle))
        {
            // Reset progress via the GameManager if it exists.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetProgress();
            }
            else
            {
                Debug.LogWarning("GameManager.Instance not found! Cannot reset progress.");
            }

            // Load the main menu scene.
            SceneManager.LoadScene("WorldMenu");
        }

        // --- Draw the "Quit" Button (Platform-Dependent) ---
        if (isQuitButtonVisible)
        {
            Rect quitRect = new Rect(centerX - (buttonWidth / 2), startY + buttonHeight + spacing, buttonWidth, buttonHeight);

            if (GUI.Button(quitRect, "Quit", buttonStyle))
            {
                // If running in the Unity Editor, stop playing.
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                // If in a standalone build, quit the application.
#else
                Application.Quit();
#endif
            }
        }
    }

    /// <summary>
    /// Checks if the quit functionality is available (disabled for WebGL).
    /// </summary>
    private bool IsQuitAvailable()
    {
#if UNITY_WEBGL
        return false;
#else
        return true;
#endif
    }

    /// <summary>
    /// Creates the custom GUIStyle for the buttons.
    /// </summary>
    private void CreateButtonStyle()
    {
        buttonStyle = new GUIStyle(GUI.skin.button);

        if (buttonFont != null)
        {
            buttonStyle.font = buttonFont;
        }

        buttonStyle.fontSize = 24;
        buttonStyle.alignment = TextAnchor.MiddleCenter;
        buttonStyle.normal.textColor = new Color(1f, 0.85f, 0.2f); // Gold/yellow
        buttonStyle.hover.textColor = Color.white;
        buttonStyle.active.textColor = Color.gray;

        // Create simple colored textures for the button's background states.
        buttonStyle.normal.background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 0.9f));
        buttonStyle.hover.background = MakeTex(2, 2, new Color(0.25f, 0.25f, 0.25f, 0.9f));
        buttonStyle.active.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.9f));
    }

    /// <summary>
    /// Helper function to create a simple 2x2 texture of a solid color.
    /// </summary>
    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = col;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }
}