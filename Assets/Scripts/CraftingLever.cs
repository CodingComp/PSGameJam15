using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingLever : MonoBehaviour, IInteractable
{
    public Transform leverTransform;
    public Crafting crafting;
    private ResolutionManager rm;
    
    private float maxRot = 135.0f;
    private float minRot = 0.0f;

    private Vector3 startMousePos;
    private bool canUseLever = true;

    private Quaternion startRotation;
    
    private void Start()
    {
        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        startRotation = leverTransform.rotation;
    }

    public void Interact()
    {
        startMousePos = rm.GetMousePosition();// - new Vector3(0,minRot,0);
    }

    public void MouseEnter()
    {
        
    }

    public void MouseExit()
    {
        
    }

    public void MouseDown()
    {
        Vector3 movementDiff = startMousePos - rm.GetMousePosition();
        
        if (!canUseLever) return;

        // Checks if lever is in down position
        if (movementDiff.y >= maxRot)
        {
            crafting.Craft();
            canUseLever = false;
            MouseReleased();
            return;
        }
        if (movementDiff.y > maxRot || movementDiff.y < minRot) return;
        
        Quaternion rotation = Quaternion.Euler(movementDiff.y, 180.0f, 0);
        leverTransform.rotation = rotation;
    }

    public void MouseReleased()
    {
        StartCoroutine(ReturnToStartPos());
        EventManager.E_Crafting.resetInteractable.Invoke();
    }

    public IEnumerator ReturnToStartPos()
    {
        yield return new WaitForSeconds(0.2f);
        
        float time = 0.0f;
        while (time < 0.25f)
        {
            // Lerps player items display to target position
            leverTransform.rotation = Quaternion.Lerp(leverTransform.rotation, startRotation, time);
            time += Time.deltaTime;
            yield return null;
        }
        canUseLever = true;
    }
}
