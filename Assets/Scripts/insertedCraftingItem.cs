using UnityEngine;

public class InsertedCraftingItem : MonoBehaviour, IInteractable
{
    private Material _baseMat;
    private Crafting _crafting;
    private Material _hoverMat;
    private ItemData _itemData;
    private PlayerInventory _playerInventory;
    private MeshRenderer _renderer;

    public void Interact()
    {
        _playerInventory.items[_itemData]++;
        _crafting.craftingItems.Remove(_itemData);
        EventManager.E_Item.itemDestroyed.Invoke(gameObject);
        EventManager.E_Crafting.updateDisplayedItems.Invoke();
        Destroy(transform.parent.gameObject);
    }

    public void MouseEnter()
    {
        _renderer.materials = new [] {_baseMat, _hoverMat};
    }

    public void MouseExit()
    {
        if (_renderer) _renderer.materials = new [] {_baseMat};
    }
    public void MouseDown() { }
    public void MouseReleased() { }

    public void Setup(Crafting crafting, PlayerInventory playerInventory, ItemData itemData, Material hoverMat)
    {
        _crafting = crafting;
        _playerInventory = playerInventory;
        _itemData = itemData;
        _hoverMat = hoverMat;
        EventManager.E_Item.itemCreated.Invoke(gameObject);
        _renderer = GetComponent<MeshRenderer>();
        _baseMat = _renderer.material;
    }
}
