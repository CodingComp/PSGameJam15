using System.Collections.Generic;
using UnityEngine;

public class DroppedBackpack : MonoBehaviour, IInteractable
{
    [SerializeField] private List<Light> lights;
    [SerializeField] private Color hoverColor;
    [SerializeField] private float hoverIntensity = 1.0f;
    private float _baseIntensity;
    private Player _player;

    public Dictionary<ItemData, int> droppedItems;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        _baseIntensity = lights[0].intensity;
        EventManager.E_Item.itemCreated.Invoke(gameObject);
    }

    public void Interact()
    {
        _player.inventory.items = droppedItems;
        EventManager.E_Item.itemDestroyed.Invoke(gameObject);
        Destroy(gameObject);
    }

    public void MouseEnter()
    {
        foreach (Light l in lights)
        {
            l.color = Color.green;
            l.intensity = hoverIntensity;
        }
    }

    public void MouseExit()
    {
        foreach (Light l in lights)
        {
            l.color = Color.white;
            l.intensity = _baseIntensity;
        }
    }

    public void MouseDown()
    {

    }

    public void MouseReleased()
    {

    }
}
