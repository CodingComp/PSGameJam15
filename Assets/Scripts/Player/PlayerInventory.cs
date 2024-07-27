using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private Player _player;
    private bool inInventory = false;

    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject inventoryObject;
    public GameObject droppedBackpackPrefab;
    
    [Header("Cameras")]
    public CinemachineVirtualCamera playerCamera;
    public CinemachineVirtualCamera inventoryCamera;

    // Items held by the player.
    public Dictionary<ItemData, int> items = new Dictionary<ItemData, int>();
    
    private void OnEnable()
    {
        EventManager.E_Player.itemPickedUp += AddItem;
    }

    private void OnDisable()
    {
        EventManager.E_Player.itemPickedUp -= AddItem;
    }
    
    private void Awake()
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
        if (Input.GetKeyDown(KeyCode.Alpha3)) DebugPrintInventory();
        
        if (!(Input.GetKeyDown(KeyCode.Tab) && (!_player.isBusy || inInventory))) return;
        
        if (!inInventory)
        {
            inInventory = true;
            inventoryObject.SetActive(true);
            inventoryUI.SetActive(true);
            
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
            inventoryUI.SetActive(false);
            _player.EndAction();
        }
    }


    /// <summary>
    /// Tries to add an item to the players inventory. If the player has a full stack of said item the item won't be added.
    /// Otherwise the item is added to the inventory and the proper callbacks are called.
    /// </summary>
    /// <param name="item">New Item's Object</param>
    /// <param name="itemData">New Item's ItemData</param>
    public void AddItem(Item item, ItemData itemData)
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
    public void RemoveItem(ItemData removedItem)
    {
        items[removedItem]--;
    }

    /// <summary>
    /// Sets count of all items to zero. Called when the player dies.
    /// </summary>
    public void DropAllItems()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[items.ElementAt(i).Key] = 0;
        }
    }

    /// <summary>
    /// Prints out amount of each item in inventory.
    /// </summary>
    void DebugPrintInventory()
    {
        print("Player Inventory \n =================");
        
        foreach (KeyValuePair<ItemData, int> item in items)
            print(item.Key.itemName + " : " + item.Value + "\n-----------------\n");
        
        print("=================");
    }
}
