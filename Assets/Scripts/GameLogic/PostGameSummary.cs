using UnityEngine;
using System.Collections.Generic;

public class PostGameSummary : MonoBehaviour
{
    bool showSummary = false;

    public int ComputeSummary(bool win, float timeLeft)
    {
        showSummary = false;
        PrepareDisplay(win, timeLeft); // Prepares the data needed for OnGUI to display
        return win ? Win(timeLeft) : Loose();
    }

    private int Loose()
    {
        int total = -1000;
        showSummary = true;

        return total;
    }

    private int Win(float timeLeft)
    {
        int total = 0;

        // Compute score multiplier
        int multiplier = 1;

        Item[] slots = GetComponent<ItemsManager>().itemsSlots;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] == Item.None)
            {
                multiplier *= (i+2); // slot 1 gives x2, slot 2 gives x3, slot 3 gives x4
            }
        }

        total = Mathf.CeilToInt(timeLeft) * multiplier;

        showSummary = true;

        return total;
    }

    #region Display

    // --- Variables used only for the OnGUI display ---
    private bool _displayIsWin;
    private float _displayTimeLeft;
    private int _displayInitialMoney;

    /// <summary>
    /// Captures the necessary information for OnGUI to use later.
    /// Called before the main calculation happens.
    /// </summary>
    private void PrepareDisplay(bool win, float timeLeft)
    {
        _displayIsWin = win;
        _displayTimeLeft = timeLeft;
        _displayInitialMoney = MoneyManager.Money;
    }

    private void OnGUI()
    {
        if (!showSummary) return;

        // --- Styles (created every frame for independence) ---
        GUIStyle backgroundStyle = GetBackgroundStyle();
        GUIStyle headerStyle = GetHeaderStyle(_displayIsWin);
        GUIStyle lineStyle = GetLineStyle();
        GUIStyle totalStyle = GetTotalStyle();

        // --- Recalculate summary details for display ---
        List<string> summaryLines = new List<string>();
        int scoreChange;

        if (_displayIsWin)
        {
            int baseScore = Mathf.CeilToInt(_displayTimeLeft);
            summaryLines.Add($"Time Bonus:|{baseScore} G");

            int multiplier = 1;
            Item[] slots = GetComponent<ItemsManager>().itemsSlots;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] == Item.None)
                {
                    int slotMultiplier = (i + 2);
                    multiplier *= slotMultiplier;
                    summaryLines.Add($"Empty Slot #{i + 1} Bonus:|x{slotMultiplier}");
                }
            }
            scoreChange = baseScore * multiplier;
        }
        else
        {
            summaryLines.Add($"Penalty:|-1000 G");
            scoreChange = -1000;
        }

        // --- Layout ---
        float boxWidth = 400;
        float boxHeight = 250 + (summaryLines.Count * 22); // Dynamic height
        Rect summaryRect = new Rect((Screen.width - boxWidth) / 2, (Screen.height - boxHeight) / 2, boxWidth, boxHeight);

        // --- Draw UI ---
        GUI.Box(summaryRect, GUIContent.none, backgroundStyle);
        GUILayout.BeginArea(summaryRect);
        {
            GUILayout.BeginVertical();

            GUILayout.Label(_displayIsWin ? "Profit" : "Loss", headerStyle);
            GUILayout.Space(15);

            DrawLine("Balance Before:", $"{_displayInitialMoney} G", lineStyle);
            DrawSeparator(lineStyle);

            foreach (var line in summaryLines)
            {
                string[] parts = line.Split('|');
                DrawLine(parts[0], parts[1], lineStyle);
            }
            DrawSeparator(lineStyle);

            string changePrefix = scoreChange >= 0 ? "+" : "";
            Color goldColor = new Color(0.941f, 0.769f, 0.275f); // #F0C846
            Color lightGold = new Color(0.98f, 0.85f, 0.5f); // Lighter gold for positive
            Color darkGold = new Color(0.8f, 0.6f, 0.2f); // Darker gold for negative
            totalStyle.normal.textColor = scoreChange >= 0 ? lightGold : darkGold;
            DrawLine("Total Change:", $"{changePrefix}{scoreChange} G", totalStyle);

            totalStyle.normal.textColor = goldColor;
            DrawLine("Balance After:", $"{_displayInitialMoney + scoreChange} G", totalStyle);

            GUILayout.FlexibleSpace();
            
            GUILayout.Space(10);

            GUILayout.EndVertical();
        }
        GUILayout.EndArea();
    }

    // --- Helper Methods for OnGUI ---

    private void DrawLine(string label, string value, GUIStyle style)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(label, style, GUILayout.ExpandWidth(true));
        GUILayout.Label(value, style, GUILayout.Width(120));
        GUILayout.EndHorizontal();
    }

    private void DrawSeparator(GUIStyle style)
    {
        GUILayout.Space(5);
        GUILayout.Label("".PadRight(50, '-'), style);
        GUILayout.Space(5);
    }

    // --- Helper Methods for Styling ---

    private GUIStyle GetBackgroundStyle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.box);
        Texture2D bgTexture = new Texture2D(1, 1);
        bgTexture.SetPixel(0, 0, new Color(0f, 0f, 0f, 0f)); // Transparent background
        bgTexture.Apply();
        style.normal.background = bgTexture;
        style.padding = new RectOffset(20, 20, 15, 15);
        
        // Add gold border effect
        style.border = new RectOffset(2, 2, 2, 2);
        return style;
    }

    private GUIStyle GetHeaderStyle(bool isWin)
    {
        Color goldColor = new Color(0.941f, 0.769f, 0.275f); // #F0C846
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = isWin ? goldColor : Color.red }
        };
    }

    private GUIStyle GetLineStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            normal = { textColor = new Color(0.9f, 0.9f, 0.9f) } // Lighter gray for better contrast on black
        };
    }

    private GUIStyle GetTotalStyle()
    {
        Color goldColor = new Color(0.941f, 0.769f, 0.275f); // #F0C846
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 18,
            fontStyle = FontStyle.Bold,
            normal = { textColor = goldColor } // Use gold for totals
        };
    }

    #endregion

    // --- Example Usage (for testing) --- TODO remove when done testing
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            int score = ComputeSummary(true, 125.5f);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            int score = ComputeSummary(false, 0f);
        }
    }
}