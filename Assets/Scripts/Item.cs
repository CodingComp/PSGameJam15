using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;

    [SerializeField] private List<Light> lights;
    
    [SerializeField] private Color hoverColor;
    [SerializeField] private float hoverIntensity = 0.5f;

    private Color currentLightColor;
    private float currentIntensity;
    
    private Color baseColor;
    private float baseIntensity;
    
    void Start()
    {
        GameObject item = Instantiate(itemData.mesh, transform);
        item.transform.localPosition = new Vector3();

        item.AddComponent<BoxCollider>();
        
        baseColor = lights[0].color;
        baseIntensity = lights[0].intensity;
        currentLightColor = baseColor;
    }
    
    void Update()
    {
        
    }

    /// <summary>
    /// Callback from the player inventory, called when the item was successfully added to the player's inventory.
    /// </summary>
    public void ItemAdded()
    {
        EventManager.E_Item.itemDestroyed.Invoke(gameObject);
        lights = new List<Light>();
        Destroy(gameObject);
    }

    /// <summary>
    /// Callback from the player inventory, called when the item couldn't be added to the player's inventory.
    /// </summary>
    public void ItemNotAdded()
    {
        foreach (Light l in lights)
        {
            l.color = Color.red;
        }
        Invoke("UpdateLights", 0.5f);
    }
    
    private void UpdateLights()
    {
        foreach (Light l in lights)
        {
            l.color = currentLightColor;
            l.intensity = currentIntensity;
        }
    }

    /*
     * Interact Interface
     */
    
    public void Interact()
    {
        EventManager.E_Player.itemPickedUp.Invoke(this, itemData);
    }

    public void MouseEnter()
    {
        currentLightColor = hoverColor;
        currentIntensity = hoverIntensity;
        
        UpdateLights();
    }

    public void MouseExit()
    {
        currentLightColor = baseColor;
        currentIntensity = baseIntensity;
        
        UpdateLights();
    }

    public void MouseDown()
    {
        
    }

    public void MouseReleased()
    {
        
    }
}
