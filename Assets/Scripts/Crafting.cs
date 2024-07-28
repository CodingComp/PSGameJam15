using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public class ItemLocations
{
    public ItemData itemData;
    
    /// <summary>
    /// Item Locations to be displayed in the crafting fabricator view.
    /// </summary>
    public List<Transform> itemLocations;
}

public class Crafting : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    [SerializeField] private Light hoverOverLight;

    [Header("Camera Properties")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private CinemachineVirtualCamera playerCamCM;
    [SerializeField] private CinemachineVirtualCamera craftingCamera;

    [Header("Player Inventory Elements")]
    [SerializeField] private GameObject playerItemsDisplay;
    [SerializeField] private Transform onScreenPosition;
    [SerializeField] private Transform offScreenPosition;
    public Vector3 targetPos;
    private float itemsDisplayTransitionTime = 0.5f;
    private float lerpSpeed = 1.0f;

    [Header("Crafting Item Locations")] 
    [SerializeField] private List<ItemLocations> craftingItemsLocations;

    private bool inCraftingMode;
    private bool cursorHovered;

    // Current items in the crafting fabricator
    private List<ItemData> craftingItems;
    public LayerMask craftingItemInsertLayer;

    // List of all items that can be crafted / all recipes.
    public List<ItemData> recipes;

    [SerializeField] private Transform leverTransform;
    [SerializeField] private CraftingLever lever;
    
    private void Start()
    {
        hoverOverLight.intensity = 0.0f;
        targetPos = offScreenPosition.position;
        craftingItems = new List<ItemData>();
        
        // Loads each item from the game, checks if said item can be crafted and adds it to the recipes list.
        ItemData[] gameItems = Resources.LoadAll<ItemData>("Items/");
        for (int i = 0; i < gameItems.Length; i++)
        {
            if (gameItems[i].canBeCrafted)
            {
                recipes.Add(gameItems[i]);
            }
        }
    }

    private void Update()
    {
        if (!inCraftingMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) LeaveCraftingMode();
        if (Input.GetKeyDown(KeyCode.Alpha2)) Craft();
    }
    
    void EnterCraftingMode()
    {
        playerCamCM.Priority = 1;
        craftingCamera.Priority = 10;

        player.StartAction();
        inCraftingMode = true;

        updateDisplayItems();

        EventManager.E_Crafting.modeChanged(true);
        StartCoroutine(displayItems());
    }

    void LeaveCraftingMode()
    {
        craftingCamera.Priority = 1;
        playerCamCM.Priority = 10;
        
        player.EndAction();
        inCraftingMode = false;
        
        StartCoroutine(removeDisplayItems());
        
        if (cursorHovered) hoverOverLight.intensity = 5.0f;
    }

    void CreateDisplayItem(ItemData itemData, ItemLocations itemLocations, int locationIndex)
    {
        GameObject displayedItem = Instantiate(itemData.mesh, itemLocations.itemLocations[locationIndex]);
        DisplayedItem dItem = displayedItem.AddComponent<DisplayedItem>();
        dItem.crafting = this;
        dItem.itemData = itemData;
    }
    
    private void updateDisplayItems()
    {
        // Creates ItemDisplay objects to be displayed
        foreach (ItemLocations itemLocations in craftingItemsLocations)
        {
            int itemCount = player.inventory.items[itemLocations.itemData];
            ItemData itemData = itemLocations.itemData;
            
            // For each amount of current item. Spawn said item
            for (int locationIndex = 0; locationIndex < itemCount; locationIndex++)
            {
                if (itemLocations.itemLocations[locationIndex].childCount == 0)
                    CreateDisplayItem(itemData, itemLocations, locationIndex);
            }
        }
    }
    
    public bool AddCraftingItem(ItemData craftingItemData)
    {
        if (craftingItems.Count == 3) return false;
        
        craftingItems.Add(craftingItemData);
        player.inventory.RemoveItem(craftingItemData);
        return true;
    }

    public void Craft()
    {
        if (craftingItems.Count != 3) CraftFailed();

        // Checks Recipes
        for (int i = 0; i < recipes.Count; i++)
        {
            if (IsRecipe(recipes[i].craftRecipe))
            {
                // Makes sure item can be added to inventory
                if (recipes[i].maxStackSize == player.inventory.items[recipes[i]])
                {
                    CraftFailed();
                    return;
                }

                CraftSucceeded(recipes[i]);
                return;
            }
        }
        
        CraftFailed();
    }
    
    void CraftSucceeded(ItemData itemCrafted)
    {
        // Adds crafted item to player inventory
        player.inventory.items[itemCrafted]++;
        
        craftingItems = new List<ItemData>();
        updateDisplayItems();
    }

    void CraftFailed()
    {
        // Adds inserted items back to player inventory
        for (int i = 0; i < craftingItems.Count; i++)
        {
            player.inventory.items[craftingItems[i]]++;    
        }
        
        craftingItems = new List<ItemData>();
        updateDisplayItems();
    }

    bool IsRecipe(Recipe itemRecipe)
    {
        List<ItemData> itemChecklist = new List<ItemData>() { itemRecipe.item1, itemRecipe.item2, itemRecipe.item3 };

        for (int i = 0; i < craftingItems.Count; i++)       // Inserted Items
        {
            for (int j = 0; j < itemChecklist.Count; j++)   // Recipe Items
            {
                if (craftingItems[i] == itemChecklist[j])
                {
                    itemChecklist.Remove(itemChecklist[j]);
                    break;
                }
            }
        }

        return itemChecklist.Count == 0;
    }
    
    /*
     * Coroutines
     */
    
    /// <summary>
    /// Smoothly moves the crafting items to the players view after the camera is zoomed in.
    /// </summary>
    private IEnumerator displayItems()
    {
        yield return new WaitForSeconds(0.75f);
        
        playerItemsDisplay.SetActive(true);
        targetPos = onScreenPosition.position;

        float time = 0.0f;
        while (time < itemsDisplayTransitionTime)
        {
            // Lerps player items display to target position
            playerItemsDisplay.transform.position = Vector3.Lerp(playerItemsDisplay.transform.position, targetPos, time);
            time += Time.deltaTime * lerpSpeed;
            yield return null;
        }
    }

    /// <summary>
    /// Smoothly removes the player crafting items from view once the player exits the crafting fabricator.
    /// </summary>
    private IEnumerator removeDisplayItems()
    {
        targetPos = offScreenPosition.position;

        float time = 0.0f;
        while (time < itemsDisplayTransitionTime)
        {
            // Lerps player items display to target position
            playerItemsDisplay.transform.position = Vector3.Lerp(playerItemsDisplay.transform.position, targetPos, time);
            time += Time.deltaTime * lerpSpeed;
            yield return null;
        }

        // Loops over each items locations and destroys the Item GameObject if present
        foreach (ItemLocations itemLocations in craftingItemsLocations)
        {
            foreach (Transform location in itemLocations.itemLocations)
            {
                if (location.childCount == 0) continue;
                EventManager.E_Item.itemDestroyed.Invoke(location.GetChild(0).gameObject);
                Destroy(location.GetChild(0).gameObject);
            }
        }
        
        EventManager.E_Crafting.modeChanged(false);
        playerItemsDisplay.SetActive(false);
    }

    /*
     * Interact Interface
     */
    
    public void Interact()
    {
        if (inCraftingMode) return;
        EnterCraftingMode();
        hoverOverLight.intensity = 0.0f;
    }

    public void MouseEnter()
    {
        cursorHovered = true;
        if (inCraftingMode) return;
        
        hoverOverLight.intensity = 5.0f;
    }

    public void MouseExit()
    {
        cursorHovered = false;
        hoverOverLight.intensity = 0.0f;
    }

    public void MouseDown()
    {
        
    }

    public void MouseReleased()
    {
        
    }
}
