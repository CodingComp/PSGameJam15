using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

[Serializable]
public class ItemLocations
{
    public ItemData itemData;

    /// <summary>
    ///     Item Locations to be displayed in the crafting fabricator view.
    /// </summary>
    public List<Transform> itemLocations;
}

public class Crafting : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;
    [SerializeField] private Light hoverOverLight;

    [Header("Camera Properties")] [SerializeField]
    private Camera mainCam;

    [SerializeField] private CinemachineVirtualCamera playerCamCm;
    [SerializeField] private CinemachineVirtualCamera craftingCamera;

    [Header("Player Inventory Elements")] [SerializeField]
    private GameObject playerItemsDisplay;

    [SerializeField] private Transform onScreenPosition;
    [SerializeField] private Transform offScreenPosition;
    [SerializeField] private Material hoverMaterial;

    [Header("Crafting Item Locations")] [SerializeField]
    private List<Transform> insertedItemLocations;

    [SerializeField] private Transform craftedItemLocation;
    [SerializeField] private List<ItemLocations> craftingItemsLocations;

    // Current items in the crafting fabricator
    public List<ItemData> craftingItems;
    public LayerMask craftingItemInsertLayer;

    // List of all items that can be crafted / all recipes.
    public List<ItemData> recipes;

    [Header("Door Variables")] [SerializeField]
    private Transform doorTransform;

    [Header("Craft Visual Variables")] [SerializeField]
    private float craftTime;

    [SerializeField] private float itemsDisplayTransitionTime = 0.5f;
    [SerializeField] private float lerpSpeed = 1.0f;
    private readonly Quaternion _doorCloseRot = Quaternion.Euler(0, 0, 0);
    private readonly Quaternion _doorOpenRot = Quaternion.Euler(0, 160, 0);
    private bool _canCraft = true;

    private ItemData _craftedItem;
    private bool _cursorHovered;
    private Vector3 _displayItemsTargetPos;

    private Color _hoverColor;

    private bool _inCraftingMode;

    private void Start()
    {
        _displayItemsTargetPos = offScreenPosition.position;
        craftingItems = new List<ItemData>();
        _hoverColor = hoverOverLight.color;
        hoverOverLight.enabled = false;

        // Loads each item from the game, checks if said item can be crafted and adds it to the recipes list.
        ItemData [] gameItems = Resources.LoadAll<ItemData>("Items/");
        for (int i = 0; i < gameItems.Length; i++)
        {
            if (gameItems[i].canBeCrafted)
            {
                recipes.Add(gameItems[i]);
            }
        }

        EventManager.E_Crafting.updateDisplayedItems += UpdateDisplayItems;
    }

    private void Update()
    {
        if (!_inCraftingMode) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) LeaveCraftingMode();
        if (Input.GetKeyDown(KeyCode.Alpha2)) Craft();
    }

    /*
     * Interact Interface
     */

    public void Interact()
    {
        if (_inCraftingMode) return;
        EnterCraftingMode();
        hoverOverLight.enabled = false;
    }

    public void MouseEnter()
    {
        _cursorHovered = true;
        if (_inCraftingMode) return;
        hoverOverLight.enabled = true;
    }

    public void MouseExit()
    {
        _cursorHovered = false;
        hoverOverLight.enabled = false;
    }

    public void MouseDown()
    {

    }

    public void MouseReleased()
    {

    }

    private void EnterCraftingMode()
    {
        playerCamCm.Priority = 1;
        craftingCamera.Priority = 10;

        player.StartAction();
        _inCraftingMode = true;

        UpdateDisplayItems();

        EventManager.E_Crafting.modeChanged(true);
        StartCoroutine(DisplayItems());
    }

    private void LeaveCraftingMode()
    {
        craftingCamera.Priority = 1;
        playerCamCm.Priority = 10;

        player.EndAction();
        _inCraftingMode = false;

        ClearItems();
        StartCoroutine(RemoveDisplayItems());

        if (_cursorHovered) hoverOverLight.enabled = true;
    }

    private void CreateDisplayItem(ItemData itemData, ItemLocations itemLocations, int locationIndex)
    {
        GameObject displayedItem = Instantiate(itemData.mesh, itemLocations.itemLocations[locationIndex]);
        DisplayedItem dItem = displayedItem.transform.GetChild(0).gameObject.AddComponent<DisplayedItem>();
        dItem.crafting = this;
        dItem.itemData = itemData;
        dItem.hoverMat = hoverMaterial;
    }

    private void UpdateDisplayItems()
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
        if (craftingItems.Count == 3 || !_canCraft) return false;

        foreach (Transform location in insertedItemLocations)
        {
            if (location.childCount == 0)
            {
                GameObject displayedItem = Instantiate(craftingItemData.mesh, location);
                InsertedCraftingItem ici = displayedItem.transform.GetChild(0).gameObject.AddComponent<InsertedCraftingItem>();
                ici.Setup(this, player.inventory, craftingItemData, hoverMaterial);
                break;
            }
        }

        craftingItems.Add(craftingItemData);
        player.inventory.RemoveItem(craftingItemData);
        return true;
    }

    public void Craft()
    {
        if (craftingItems.Count != 3)
        {
            CraftFailed();
            return;
        }

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

    private void CraftSucceeded(ItemData itemCrafted)
    {
        _craftedItem = itemCrafted;
        _canCraft = false;
        StartCoroutine(DisplayCraftSucceeded());
    }

    private void CraftFailed()
    {
        StartCoroutine(DisplayCraftFailed());
    }

    // Removes inserted items from crafter back to players inventory. 
    private void ClearItems()
    {
        // Adds inserted items back to player inventory
        for (int i = 0; i < craftingItems.Count; i++)
        {
            player.inventory.items[craftingItems[i]]++;
        }

        if (craftedItemLocation.childCount != 0)
        {
            player.inventory.items[_craftedItem]++;
            EventManager.E_Item.itemDestroyed.Invoke(craftedItemLocation.GetChild(0).GetChild(0).gameObject);
            Destroy(craftedItemLocation.GetChild(0).gameObject);
        }

        RemoveInsertedItems();
        UpdateDisplayItems();
    }

    private void RemoveInsertedItems()
    {
        craftingItems = new List<ItemData>();
        foreach (Transform location in insertedItemLocations)
        {
            if (location.childCount != 0)
            {
                EventManager.E_Item.itemDestroyed.Invoke(location.GetChild(0).gameObject);
                Destroy(location.GetChild(0).gameObject);
            }
        }
    }

    private bool IsRecipe(Recipe itemRecipe)
    {
        List<ItemData> itemChecklist = new List<ItemData> {itemRecipe.item1, itemRecipe.item2, itemRecipe.item3};

        for (int i = 0; i < craftingItems.Count; i++) // Inserted Items
        {
            for (int j = 0; j < itemChecklist.Count; j++) // Recipe Items
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
    ///     Smoothly moves the crafting items to the players view after the camera is zoomed in.
    /// </summary>
    private IEnumerator DisplayItems()
    {
        yield return new WaitForSeconds(0.75f);

        playerItemsDisplay.SetActive(true);
        _displayItemsTargetPos = onScreenPosition.position;

        float time = 0.0f;
        while (time < itemsDisplayTransitionTime)
        {
            // Lerps player items display to target position
            playerItemsDisplay.transform.position = Vector3.Lerp(playerItemsDisplay.transform.position, _displayItemsTargetPos, time);
            doorTransform.localRotation = Quaternion.Lerp(doorTransform.localRotation, _doorOpenRot, time);
            time += Time.deltaTime * lerpSpeed;
            yield return null;
        }
    }

    /// <summary>
    ///     Smoothly removes the player crafting items from view once the player exits the crafting fabricator.
    /// </summary>
    private IEnumerator RemoveDisplayItems()
    {
        _displayItemsTargetPos = offScreenPosition.position;

        float time = 0.0f;
        while (time < itemsDisplayTransitionTime)
        {
            // Lerps player items display to target position
            playerItemsDisplay.transform.position = Vector3.Lerp(playerItemsDisplay.transform.position, _displayItemsTargetPos, time);
            doorTransform.localRotation = Quaternion.Lerp(doorTransform.localRotation, _doorCloseRot, time);
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

    private IEnumerator DisplayCraftSucceeded()
    {
        float time = 0.0f;
        while (time < itemsDisplayTransitionTime)
        {
            doorTransform.localRotation = Quaternion.Lerp(doorTransform.localRotation, _doorCloseRot, time);
            time += Time.deltaTime * lerpSpeed;
            yield return null;
        }

        yield return new WaitForSeconds(craftTime);

        RemoveInsertedItems();

        GameObject displayedItem = Instantiate(_craftedItem.mesh, craftedItemLocation);
        InsertedCraftingItem ici = displayedItem.transform.GetChild(0).gameObject.AddComponent<InsertedCraftingItem>();
        // displayedItem.transform.GetChild(0).gameObject.AddComponent<InsertedCraftingItem>();
        ici.Setup(this, player.inventory, _craftedItem, hoverMaterial);

        time = 0.0f;
        while (time < itemsDisplayTransitionTime)
        {
            doorTransform.localRotation = Quaternion.Lerp(doorTransform.localRotation, _doorOpenRot, time);
            time += Time.deltaTime * lerpSpeed;
            yield return null;
        }

        _canCraft = true;
    }

    private IEnumerator DisplayCraftFailed()
    {
        hoverOverLight.color = Color.red;
        hoverOverLight.enabled = true;

        yield return new WaitForSeconds(0.5f);

        hoverOverLight.enabled = false;
        hoverOverLight.color = _hoverColor;
    }
}
