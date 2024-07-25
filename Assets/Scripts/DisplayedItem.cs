using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayedItem : MonoBehaviour, IInteractable
{
    private ResolutionManager rm;

    private void Start()
    {
        rm = GetComponent<ResolutionManager>();
    }

    /*
     * Interact Interface
     */
    
    public void Interact()
    {
        print("Clicked");
    }

    public void MouseEnter()
    {
        print("Enter");
    }

    public void MouseExit()
    {
        print("Exit");
    }

    public void MouseDown()
    {
        print("Down");
        
    }

    public void MouseReleased()
    {
        print("Released");
    }
}
