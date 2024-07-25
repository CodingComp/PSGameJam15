using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    [SerializeField] private LayerMask displayedItemLayer;
    
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
    
    private void Start()
    {
        hoverOverLight.intensity = 0.0f;
        targetPos = offScreenPosition.position;
    }

    private void Update()
    {
        if (inCraftingMode && Input.GetKeyDown(KeyCode.F1)) LeaveCraftingMode();
    }
    
    void EnterCraftingMode()
    {
        playerCamCM.Priority = 1;
        craftingCamera.Priority = 10;

        player.StartAction();
        inCraftingMode = true;

        foreach (ItemLocations itemLocations in craftingItemsLocations)
        {
            int itemCount = player.inventory.items[itemLocations.itemData];
            ItemData itemData = itemLocations.itemData;
            
            // For each amount of current item. Spawn said item
            for (int locationIndex = 0; locationIndex < itemCount; locationIndex++)
            {
                GameObject displayedItem = Instantiate(itemData.mesh, itemLocations.itemLocations[locationIndex]);
                displayedItem.AddComponent<DisplayedItem>();
                //displayedItem.layer = displayedItemLayer;
            }
        }

        EventManager.E_Crafting.modeChanged(true);
        StartCoroutine(displayItems());
    }

    void LeaveCraftingMode()
    {
        craftingCamera.Priority = 1;
        playerCamCM.Priority = 10;
        
        player.EndAction();
        inCraftingMode = false;
        
        StartCoroutine(removeItems());
        
        if (cursorHovered) hoverOverLight.intensity = 5.0f;
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
    private IEnumerator removeItems()
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
        print("Down");
    }

    public void MouseReleased()
    {
        print("Released");
    }
}
