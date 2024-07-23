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
    public static readonly PlayerEvents Player = new PlayerEvents();

    public class PlayerEvents
    {
        public UnityAction<Item, ItemData> itemPickedUp;
    }
}
