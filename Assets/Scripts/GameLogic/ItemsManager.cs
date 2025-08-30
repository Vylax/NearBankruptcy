using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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

public static class ShopItemPrices
{
    public static readonly Dictionary<Item, int> ItemPrices = new Dictionary<Item, int>
    {
        { Item.None, 0 },
        { Item.Dash, 2250 },
        { Item.DoubleJump, 3250 },
        { Item.Shield, 3500 },
        { Item.SpeedBoost, 1750 },
        { Item.Glider, 1500 },
        { Item.SmallSize, 1250 },
        { Item.SlowMotion, 4000 },
        { Item.ReverseGravity, 2250 },
        { Item.WallJump, 2500 },
        { Item.WalkThroughWalls, 4500 },
        { Item.DepthStrider, 1000 }
    };

    public static bool TryGetPrice(Item item, out int price)
    {
        return ItemPrices.TryGetValue(item, out price);
    }
}

public static class ItemDescriptions
{
    public static readonly Dictionary<Item, string> Descriptions = new Dictionary<Item, string>
    {
        { Item.None, "Empty slot" },
        { Item.Dash, "Quick forward dash ability" },
        { Item.DoubleJump, "Jump again while in mid-air" },
        { Item.Shield, "Protects from one hit" },
        { Item.SpeedBoost, "Increases movement speed" },
        { Item.Glider, "Slow fall and glide horizontally" },
        { Item.SmallSize, "Reduces player hitbox size" },
        { Item.SlowMotion, "Slows down time when activated" },
        { Item.ReverseGravity, "Flip gravity direction" },
        { Item.WallJump, "Jump off walls" },
        { Item.WalkThroughWalls, "Phase through solid objects" },
        { Item.DepthStrider, "Move faster through water/liquids" }
    };

    public static string GetDescription(Item item)
    {
        return Descriptions.TryGetValue(item, out string description) ? description : "Unknown item";
    }
}

public class ItemsManager : MonoBehaviour
{
    [Header("Item Slots")]
    public Item[] itemsSlots = new Item[3];           // Active items
    public Item[] inventorySlots = new Item[3];       // Inventory items
    public Item[] shopSlots = new Item[3];            // Shop items
    public Item sellSlot = Item.None;                 // Sell slot
    
    [Header("Shop Management")]
    public HashSet<Item> purchasedItems = new HashSet<Item>();
    private List<Item> availableItems = new List<Item>();
    
    [Header("Drag and Drop")]
    private Item draggedItem = Item.None;
    private int draggedFromSlot = -1;
    private SlotType draggedFromType = SlotType.None;
    private bool isDragging = false;
    
    [Header("GUI Settings")]
    private const float slotSize = 80f;
    private const float slotSpacing = 10f;
    private const float rowSpacing = 20f;
    private Rect shopArea;
    private Rect activeArea;
    private Rect inventoryArea;
    private Rect sellArea;
    private Rect effectsArea;
    
    public enum SlotType
    {
        None,
        Shop,
        Active,
        Inventory,
        Sell
    }

    private void Start()
    {
        InitializeShop();
        CalculateAreas();
    }
    
    private void InitializeShop()
    {
        // Get all items except None
        availableItems = System.Enum.GetValues(typeof(Item))
            .Cast<Item>()
            .Where(item => item != Item.None)
            .ToList();
        
        // Fill initial shop slots
        RefillShopSlots();
    }
    
    private void RefillShopSlots()
    {
        var unpurchasedItems = availableItems.Where(item => !purchasedItems.Contains(item)).ToList();
        
        for (int i = 0; i < shopSlots.Length; i++)
        {
            if (shopSlots[i] == Item.None && unpurchasedItems.Count > 0)
            {
                shopSlots[i] = unpurchasedItems[0];
                unpurchasedItems.RemoveAt(0);
            }
        }
    }
    
    private void CalculateAreas()
    {
        float totalWidth = 3 * slotSize + 2 * slotSpacing;
        float startX = (Screen.width - totalWidth) / 2f - 200f; // Offset left for effects panel
        
        // Shop area (top row)
        shopArea = new Rect(startX, 50f, totalWidth, slotSize);
        
        // Active items area (middle row)
        activeArea = new Rect(startX, shopArea.y + slotSize + rowSpacing, totalWidth, slotSize);
        
        // Inventory area (bottom row)
        inventoryArea = new Rect(startX, activeArea.y + slotSize + rowSpacing, totalWidth, slotSize);
        
        // Sell slot (right of inventory, vertically centered between inventory and active)
        float sellX = inventoryArea.x + totalWidth + 30f;
        float sellY = activeArea.y + (inventoryArea.y - activeArea.y) / 2f;
        sellArea = new Rect(sellX, sellY, slotSize, slotSize);
        
        // Effects area (right side)
        effectsArea = new Rect(Screen.width - 300f, 50f, 280f, 400f);
    }

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
    
    private bool HasFreeSlot()
    {
        return itemsSlots.Contains(Item.None) || inventorySlots.Contains(Item.None);
    }
    
    private void AddItemToFreeSlot(Item item)
    {
        // Try active slots first
        for (int i = 0; i < itemsSlots.Length; i++)
        {
            if (itemsSlots[i] == Item.None)
            {
                itemsSlots[i] = item;
                return;
            }
        }
        
        // Try inventory slots
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] == Item.None)
            {
                inventorySlots[i] = item;
                return;
            }
        }
    }
    
    private void PurchaseItem(int shopSlotIndex)
    {
        if (sellSlot != Item.None) return; // Cannot buy when sell slot is occupied
        if (shopSlotIndex < 0 || shopSlotIndex >= shopSlots.Length) return;
        
        Item item = shopSlots[shopSlotIndex];
        if (item == Item.None) return;
        
        if (!ShopItemPrices.TryGetPrice(item, out int price)) return;
        if (MoneyManager.Money < price) return;
        if (!HasFreeSlot()) return;
        
        // Purchase successful
        MoneyManager.RemoveMoney(price);
        purchasedItems.Add(item);
        AddItemToFreeSlot(item);
        shopSlots[shopSlotIndex] = Item.None;
        RefillShopSlots();
    }
    
    private void SellItem()
    {
        if (sellSlot == Item.None) return;
        
        if (ShopItemPrices.TryGetPrice(sellSlot, out int price))
        {
            int sellPrice = Mathf.RoundToInt(price * 0.5f); // Sell for half price
            MoneyManager.AddMoney(sellPrice);
            sellSlot = Item.None;
        }
    }
    
    private void OnGUI()
    {
        DrawShopArea();
        DrawActiveArea();
        DrawInventoryArea();
        DrawSellArea();
        DrawEffectsArea();
        HandleDragAndDrop();
    }
    
    private void DrawShopArea()
    {
        GUI.Label(new Rect(shopArea.x, shopArea.y - 25f, shopArea.width, 20f), "SHOP", GetHeaderStyle());
        
        for (int i = 0; i < shopSlots.Length; i++)
        {
            Rect slotRect = GetSlotRect(shopArea, i);
            DrawSlot(slotRect, shopSlots[i], SlotType.Shop, i);
            
            // Draw price
            if (shopSlots[i] != Item.None && ShopItemPrices.TryGetPrice(shopSlots[i], out int price))
            {
                GUI.Label(new Rect(slotRect.x, slotRect.y + slotSize + 2f, slotSize, 20f), 
                    $"{price}G", GetPriceStyle());
            }
        }
    }
    
    private void DrawActiveArea()
    {
        GUI.Label(new Rect(activeArea.x, activeArea.y - 25f, activeArea.width, 20f), "ACTIVE ITEMS", GetHeaderStyle());
        
        for (int i = 0; i < itemsSlots.Length; i++)
        {
            Rect slotRect = GetSlotRect(activeArea, i);
            DrawSlot(slotRect, itemsSlots[i], SlotType.Active, i);
            
            // Draw multiplier for empty slots
            if (itemsSlots[i] == Item.None)
            {
                string multiplier = $"x{i + 2}";
                GUI.Label(slotRect, multiplier, GetMultiplierStyle());
            }
        }
    }
    
    private void DrawInventoryArea()
    {
        GUI.Label(new Rect(inventoryArea.x, inventoryArea.y - 25f, inventoryArea.width, 20f), "INVENTORY", GetHeaderStyle());
        
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            Rect slotRect = GetSlotRect(inventoryArea, i);
            DrawSlot(slotRect, inventorySlots[i], SlotType.Inventory, i);
        }
    }
    
    private void DrawSellArea()
    {
        GUI.Label(new Rect(sellArea.x, sellArea.y - 25f, sellArea.width, 20f), "SELL", GetHeaderStyle());
        DrawSlot(sellArea, sellSlot, SlotType.Sell, 0);
        
        // Sell button
        Rect sellButtonRect = new Rect(sellArea.x, sellArea.y + slotSize + 10f, slotSize, 30f);
        if (GUI.Button(sellButtonRect, "SELL") && sellSlot != Item.None)
        {
            SellItem();
        }
        
        // Show sell price
        if (sellSlot != Item.None && ShopItemPrices.TryGetPrice(sellSlot, out int price))
        {
            int sellPrice = Mathf.RoundToInt(price * 0.5f);
            GUI.Label(new Rect(sellArea.x, sellArea.y + slotSize + 45f, slotSize, 20f), 
                $"{sellPrice}G", GetPriceStyle());
        }
    }
    
    private void DrawEffectsArea()
    {
        GUI.Box(effectsArea, "", GUI.skin.box);
        
        GUILayout.BeginArea(effectsArea);
        GUILayout.Label("ACTIVE EFFECTS", GetHeaderStyle());
        GUILayout.Space(10f);
        
        // Show active item effects
        for (int i = 0; i < itemsSlots.Length; i++)
        {
            if (itemsSlots[i] != Item.None)
            {
                string description = ItemDescriptions.GetDescription(itemsSlots[i]);
                GUILayout.Label($"• {itemsSlots[i]}: {description}", GetEffectStyle());
            }
            else
            {
                GUILayout.Label($"• Empty Slot {i + 1}: x{i + 2} multiplier", GetMultiplierEffectStyle());
            }
        }
        
        GUILayout.Space(20f);
        GUILayout.Label($"Money: {MoneyManager.Money}G", GetMoneyStyle());
        
        GUILayout.EndArea();
    }
    
    private void DrawSlot(Rect rect, Item item, SlotType slotType, int slotIndex)
    {
        // Determine if this slot can be clicked
        bool canClick = false;
        if (slotType == SlotType.Shop && item != Item.None && sellSlot == Item.None)
            canClick = true;
        
        // Draw slot background
        Color originalColor = GUI.backgroundColor;
        if (isDragging && draggedFromType == slotType && draggedFromSlot == slotIndex)
            GUI.backgroundColor = Color.yellow;
        else if (canClick && rect.Contains(Event.current.mousePosition))
            GUI.backgroundColor = Color.green;
        
        GUI.Box(rect, "", GUI.skin.button);
        GUI.backgroundColor = originalColor;
        
        // Draw item
        if (item != Item.None)
        {
            GUI.Label(rect, item.ToString(), GetItemStyle());
        }
        
        // Handle clicks
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            if (slotType == SlotType.Shop && canClick)
            {
                PurchaseItem(slotIndex);
            }
            else if (item != Item.None)
            {
                StartDrag(item, slotType, slotIndex);
            }
        }
        
        // Handle drop
        if (isDragging && Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
        {
            HandleDrop(slotType, slotIndex);
        }
    }
    
    private void StartDrag(Item item, SlotType fromType, int fromSlot)
    {
        if (fromType == SlotType.Shop) return; // Cannot drag from shop
        
        draggedItem = item;
        draggedFromType = fromType;
        draggedFromSlot = fromSlot;
        isDragging = true;
    }
    
    private void HandleDrop(SlotType toType, int toSlot)
    {
        if (!isDragging) return;
        if (toType == SlotType.Shop) return; // Cannot drop to shop
        
        // Get target item
        Item targetItem = GetItemInSlot(toType, toSlot);
        
        // Set dragged item in target slot
        SetItemInSlot(toType, toSlot, draggedItem);
        
        // Set target item in source slot (swap)
        SetItemInSlot(draggedFromType, draggedFromSlot, targetItem);
        
        // End drag
        isDragging = false;
        draggedItem = Item.None;
        draggedFromType = SlotType.None;
        draggedFromSlot = -1;
    }
    
    private void HandleDragAndDrop()
    {
        if (isDragging && Event.current.type == EventType.MouseUp)
        {
            // Dropped outside any slot - cancel drag
            isDragging = false;
            draggedItem = Item.None;
            draggedFromType = SlotType.None;
            draggedFromSlot = -1;
        }
    }
    
    private Item GetItemInSlot(SlotType slotType, int slotIndex)
    {
        switch (slotType)
        {
            case SlotType.Active: return itemsSlots[slotIndex];
            case SlotType.Inventory: return inventorySlots[slotIndex];
            case SlotType.Sell: return sellSlot;
            default: return Item.None;
        }
    }
    
    private void SetItemInSlot(SlotType slotType, int slotIndex, Item item)
    {
        switch (slotType)
        {
            case SlotType.Active: itemsSlots[slotIndex] = item; break;
            case SlotType.Inventory: inventorySlots[slotIndex] = item; break;
            case SlotType.Sell: sellSlot = item; break;
        }
    }
    
    private Rect GetSlotRect(Rect area, int index)
    {
        float x = area.x + index * (slotSize + slotSpacing);
        return new Rect(x, area.y, slotSize, slotSize);
    }
    
    // GUI Styles
    private GUIStyle GetHeaderStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };
    }
    
    private GUIStyle GetItemStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
            wordWrap = true
        };
    }
    
    private GUIStyle GetPriceStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.yellow }
        };
    }
    
    private GUIStyle GetMultiplierStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 16,
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.cyan }
        };
    }
    
    private GUIStyle GetEffectStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            normal = { textColor = Color.white },
            wordWrap = true
        };
    }
    
    private GUIStyle GetMultiplierEffectStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            normal = { textColor = Color.cyan },
            wordWrap = true
        };
    }
    
    private GUIStyle GetMoneyStyle()
    {
        return new GUIStyle(GUI.skin.label)
        {
            fontSize = 14,
            fontStyle = FontStyle.Bold,
            normal = { textColor = Color.yellow }
        };
    }
}
