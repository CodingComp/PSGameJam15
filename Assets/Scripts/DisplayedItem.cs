using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayedItem : MonoBehaviour, IInteractable
{
    public Crafting crafting;
    public ItemData itemData;
    private ResolutionManager rm;
    private float zDist;
    
    private void Start()
    {
        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        EventManager.E_Item.itemCreated.Invoke(gameObject);
    }

    /*
     * Interact Interface
     */
    
    public void Interact()
    {
        zDist = rm.mainCamera.WorldToScreenPoint(transform.position).z;

    }

    public void MouseEnter()
    {
        
    }

    public void MouseExit()
    {
        
    }

    public void MouseDown()
    {
        transform.position = rm.mainCamera.ScreenToWorldPoint(rm.GetMousePosition(zDist));
    }

    public void MouseReleased()
    {
        Ray ray = rm.mainCamera.ScreenPointToRay(rm.GetMousePosition());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, crafting.craftingItemInsertLayer) &&
            crafting.AddCraftingItem(itemData))
        {
            EventManager.E_Item.itemDestroyed.Invoke(gameObject);
            Destroy(gameObject);
            return;
        }
        
        transform.localPosition = Vector3.zero; 
    }
}
