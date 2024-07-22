using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class Crafting : MonoBehaviour, IInteractable
{
    [SerializeField] private Player player;

    [SerializeField] private Light hoverOverLight;
    
    [Header("Camera Properties")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private CinemachineVirtualCamera playerCamCM;
    [SerializeField] private CinemachineVirtualCamera craftingCamera;

    private bool inCraftingMode;
    private bool cursorHovered;
    
    private void Start()
    {
        hoverOverLight.intensity = 0.0f;
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
    }

    void LeaveCraftingMode()
    {
        craftingCamera.Priority = 1;
        playerCamCM.Priority = 10;
        
        player.EndAction();
        inCraftingMode = false;

        if (cursorHovered) hoverOverLight.intensity = 5.0f;
    }
    
    /*
     * Interact Interface
     */
    public void Interact()
    {
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
}
