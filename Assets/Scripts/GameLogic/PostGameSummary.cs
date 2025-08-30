using UnityEngine;

public class PostGameSummary : MonoBehaviour
{
    bool showSummary = false;

    public int ComputeSummary(bool win, float timeLeft)
    {
        showSummary = false;
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

    private void OnGUI()
    {
        if (!showSummary) return;

        // TODO display summary here
    }
}
