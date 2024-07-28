using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;
    
    [Header("Light Visualizer")]
    [SerializeField] private Transform lightPivot;
    [SerializeField] private List<Light> lights;
    private float rotSpeed = 10.0f;
    private Quaternion lightRotTarget;
    
    [SerializeField] private Color hoverColor;
    [SerializeField] private float hoverIntensity;

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

        lightRotTarget = quaternion.Euler(0,0,0);
        EventManager.E_Player.playerRotated += (newRot) => lightRotTarget = newRot;
    }
    
    void Update()
    {
        lightPivot.transform.rotation = Quaternion.Lerp(lightPivot.transform.rotation, lightRotTarget, Time.deltaTime * rotSpeed);
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
        StartCoroutine(DisplayNotAdded());
    }

    private IEnumerator DisplayNotAdded()
    {
        foreach (Light l in lights)
        {
            l.color = Color.red;
        }

        yield return new WaitForSeconds(0.5f);
        
        UpdateLights();
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
