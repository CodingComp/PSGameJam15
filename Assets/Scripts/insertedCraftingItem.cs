using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsertedCraftingItem : MonoBehaviour, IInteractable
{
    private Crafting _crafting;
    private Material _hoverMat;
    private Material _baseMat;
    private MeshRenderer _renderer;
    private PlayerInventory _playerInventory;
    private ItemData _itemData;
    
    public void Setup(Crafting crafting, PlayerInventory playerInventory, ItemData itemData, Material hoverMat)
    {
        _crafting = crafting;
        _playerInventory = playerInventory;
        _itemData = itemData;
        _hoverMat = hoverMat;
        EventManager.E_Item.itemCreated.Invoke(gameObject);
        _renderer = transform.GetChild(0).GetComponent<MeshRenderer>();
        _baseMat = _renderer.material;
    }

    public void Interact()
    {
        _playerInventory.items[_itemData]++;
        _crafting.craftingItems.Remove(_itemData);
        EventManager.E_Item.itemDestroyed.Invoke(gameObject);
        EventManager.E_Crafting.updateDisplayedItems.Invoke();
        Destroy(gameObject);
    }

    public void MouseEnter()
    {
        _renderer.materials = new [] {_baseMat, _hoverMat};
    }

    public void MouseExit()
    {
        _renderer.materials = new [] {_baseMat};
    }
    public void MouseDown() { }
    public void MouseReleased() { }
}