using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script uses the OnGUI system to draw "Try Again" and "Quit" buttons.
/// It's designed for a lose screen and provides custom styling for the buttons.
/// Attach this component to any active GameObject in your lose scene, like the Camera.
/// </summary>
public class LoseScreenGUI : MonoBehaviour
{
    /// <summary>
    /// Optional: Assign a custom font in the Inspector to be used for the buttons.
    /// If left empty, it will use the default Unity font.
    /// </summary>
    public Font buttonFont;

    private GUIStyle buttonStyle;

    // This is the core of the immediate-mode GUI system.
    // It runs every frame, similar to Update().
    void OnGUI()
    {
        // Initialize the custom style for our buttons if it hasn't been already.
        // We do this here because OnGUI is the only place we can safely access GUI.skin.
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

        // We'll stack them vertically. Check if the Quit button should be shown to calculate the total height.
        bool isQuitButtonVisible = IsQuitAvailable();
        float totalUIHeight = isQuitButtonVisible ? (buttonHeight * 2 + spacing) : buttonHeight;

        // Start drawing below the center of the screen to leave space for "Bankrupt!" text.
        float startY = (Screen.height / 2f) - (totalUIHeight / 2f) + 100;


        // --- Draw the "Try Again" Button ---

        Rect tryAgainRect = new Rect(centerX - (buttonWidth / 2), startY, buttonWidth, buttonHeight);

        if (GUI.Button(tryAgainRect, "Try Again", buttonStyle))
        {
            // If the button is clicked...

            // First, call the GameManager to reset progress.
            // A check is included to prevent errors if the instance doesn't exist.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetProgress();
            }
            else
            {
                Debug.LogWarning("GameManager.Instance not found! Cannot reset progress.");
            }

            // Then, load the main menu scene.
            SceneManager.LoadScene("WorldMenu");
        }

        // --- Draw the "Quit" Button (Platform-Dependent) ---

        // We only show the quit button if it's available on the current platform.
        if (isQuitButtonVisible)
        {
            Rect quitRect = new Rect(centerX - (buttonWidth / 2), startY + buttonHeight + spacing, buttonWidth, buttonHeight);

            if (GUI.Button(quitRect, "Quit", buttonStyle))
            {
                // If we are running in the Unity Editor, stop playing.
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;

                // If we are in a standalone build (Windows, Mac, Linux), quit the application.
#else
                Application.Quit();
#endif
            }
        }
    }

    /// <summary>
    /// Checks if the quit functionality is available. It's disabled for WebGL builds.
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
    /// Creates the custom GUIStyle for our buttons.
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
        buttonStyle.normal.textColor = new Color(1f, 0.85f, 0.2f); // A nice gold/yellow
        buttonStyle.hover.textColor = Color.white;
        buttonStyle.active.textColor = Color.gray;

        // Create simple colored textures for the button's background states.
        // This gives it a clean, modern look that fits your neon theme.
        buttonStyle.normal.background = MakeTex(2, 2, new Color(0.15f, 0.15f, 0.15f, 0.9f));
        buttonStyle.hover.background = MakeTex(2, 2, new Color(0.25f, 0.25f, 0.25f, 0.9f));
        buttonStyle.active.background = MakeTex(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.9f));
    }

    /// <summary>
    /// A helper function to create a simple 2x2 texture of a solid color.
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
