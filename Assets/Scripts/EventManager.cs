using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages / stores different events that are used in the game.
/// Each event is stored here and can be used by any script that needs to listen or invoke an event.
/// </summary>
public static class EventManager
{
    public static readonly PlayerEvents E_Player = new PlayerEvents();
    public static readonly ItemEvents E_Item = new ItemEvents();
    public static readonly CraftingEvents E_Crafting = new CraftingEvents();
    
    public class PlayerEvents
    {
        public UnityAction<Item, ItemData> itemPickedUp;
    }

    public class ItemEvents
    {
        public UnityAction<GameObject> itemDestroyed;
    }

    public class CraftingEvents
    {
        // Crafting mode changed, true / false for current crafting mode.
        public UnityAction<bool> modeChanged;
    }
}
