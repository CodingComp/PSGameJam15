using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Player _player;
    private bool inInventory = false;

    [SerializeField] private GameObject inventoryObject;
    
    [Header("Cameras")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera inventoryCamera;

    // Items held by the player.
    public Dictionary<ItemData, int> items;
    
    private void OnEnable()
    {
        EventManager.Player.itemPickedUp += AddItem;
    }

    private void OnDisable()
    {
        EventManager.Player.itemPickedUp -= AddItem;
    }
    
    private void Start()
    {
        _player = GetComponent<Player>();
        items = new Dictionary<ItemData, int>();
        
        // Loads each item from the game and puts it into the inventory dictionary.
        ItemData[] gameItems = Resources.LoadAll<ItemData>("Items/");
        for (int i = 0; i < gameItems.Length; i++)
        {
            items.Add(gameItems[i], 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2)) DebugPrintInventory();
        
        if (!(Input.GetKeyDown(KeyCode.Tab) && (!_player.isBusy || inInventory))) return;
        
        if (!inInventory)
        {
            inInventory = true;
            inventoryObject.SetActive(true);
            
            inventoryCamera.Priority = 10;
            playerCamera.Priority = 1;
            
            _player.StartAction();
        }
        else
        {
            inInventory = false;
            
            playerCamera.Priority = 10;
            inventoryCamera.Priority = 1;
            
            inventoryObject.SetActive(false);
            _player.EndAction();
        }
    }


    /// <summary>
    /// Tries to add an item to the players inventory. If the player has a full stack of said item the item won't be added.
    /// Otherwise the item is added to the inventory and the proper callbacks are called.
    /// </summary>
    /// <param name="item">New Item's Object</param>
    /// <param name="itemData">New Item's ItemData</param>
    void AddItem(Item item, ItemData itemData)
    {
        if (items[itemData] < itemData.maxStackSize)
        {
            items[itemData]++;
            item.ItemAdded();
        }
        else
        {
            item.ItemNotAdded();
        }
    }
    
    /// <summary>
    /// Removes item from the player's inventory.
    /// </summary>
    /// <param name="removedItem">Item to remove</param>
    void RemoveItem(ItemData removedItem)
    {
        items[removedItem]--;
    }
    
    /// <summary>
    /// Prints out amount of each item in inventory.
    /// </summary>
    void DebugPrintInventory()
    {
        print("Player Inventory \n =================");
        
        foreach (KeyValuePair<ItemData, int> item in items)
            print(item.Key.itemName + " : " + item.Value);
        
        print("=================");
    }
}