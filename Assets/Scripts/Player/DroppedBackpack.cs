using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedBackpack : MonoBehaviour, IInteractable
{
    private Player _player;
    private float baseIntensity;
    
    [SerializeField] private List<Light> lights;
    [SerializeField] private Color hoverColor;
    [SerializeField] private float hoverIntensity = 1.0f;
    
    public Dictionary<ItemData, int> droppedItems;
    
    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        baseIntensity = lights[0].intensity;
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
        foreach (Light light in lights)
        {
            light.color = Color.green;
            light.intensity = hoverIntensity;
        }
    }

    public void MouseExit()
    {
        foreach (Light light in lights)
        {
            light.color = Color.white;
            light.intensity = baseIntensity;
        }
    }

    public void MouseDown()
    {
        
    }

    public void MouseReleased()
    {
        
    }
}
