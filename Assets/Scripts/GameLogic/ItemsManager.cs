using UnityEngine;

public enum Item
{
    None,
    Dash,
    DoubleJump,
    Shield,
    SpeedBoost,
    Glider,
    SmallSize,
    SlowMotion,
    ReverseGravity,
    WallJump,
    WalkThroughWalls,
    DepthStrider
}

public class ItemsManager : MonoBehaviour
{
    public Item[] itemsSlots = new Item[3];

    public void EquipItem(Item item, int slot)
    {
        if (slot < 0 || slot >= itemsSlots.Length) return;

        if (itemsSlots[slot] != Item.None)
        {
            Debug.LogWarning($"Slot {slot} is already occupied by {itemsSlots[slot]}");
            return;
        }
        itemsSlots[slot] = item;
    }

    public void UnequipItem(int slot)
    {
        if (slot < 0 || slot >= itemsSlots.Length) return;
        itemsSlots[slot] = Item.None;
    }
}
