using UnityEngine;

public class DisplayedItem : MonoBehaviour, IInteractable
{
    public Crafting crafting;
    public ItemData itemData;
    public Material hoverMat;
    private Material _baseMat;
    private Renderer _renderer;
    private ResolutionManager _rm;
    private float _zDist;

    private void Start()
    {
        _rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        EventManager.E_Item.itemCreated.Invoke(gameObject);
        _renderer = transform.GetChild(0).GetComponent<Renderer>();
        _baseMat = _renderer.material;
    }

    /*
     * Interact Interface
     */

    public void Interact()
    {
        _zDist = _rm.mainCamera.WorldToScreenPoint(transform.position).z;
    }

    public void MouseEnter()
    {
        _renderer.materials = new [] {_baseMat, hoverMat};
    }

    public void MouseExit()
    {
        _renderer.materials = new [] {_baseMat};
    }

    public void MouseDown()
    {
        transform.position = _rm.mainCamera.ScreenToWorldPoint(_rm.GetMousePosition(_zDist) + new Vector3(0, -20, 0));
    }

    public void MouseReleased()
    {
        Ray ray = _rm.mainCamera.ScreenPointToRay(_rm.GetMousePosition());
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
