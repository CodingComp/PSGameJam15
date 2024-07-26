using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    void MouseEnter();
    void MouseExit();
    void MouseDown();
    void MouseReleased();
}

/// <summary>
/// Interact class used for the player.
/// Checks to see if the player interacts with an interactable object, hovers over an interactable object, etc. 
/// </summary>
public class Interact : MonoBehaviour
{
    public LayerMask interactLayer;
    public LayerMask craftItemLayer;
    public ResolutionManager rm;

    private Dictionary<GameObject, IInteractable> interactables;
    private GameObject hoveredInteractable;

    private bool craftingMode = false;
    
    void Start()
    {
        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();

        interactables = new Dictionary<GameObject, IInteractable>();
        // Gets all interactable objects already placed in the world 
        IEnumerable<IInteractable> interactableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>();
        foreach (IInteractable interactable in interactableObjects)
        {
            interactables.Add(((MonoBehaviour)interactable).gameObject, interactable);
        }
        
        // Events
        EventManager.E_Item.itemCreated += (item) =>
        {
            interactables.Add(item, item.GetComponent<IInteractable>());
        };
        EventManager.E_Item.itemDestroyed += (item) =>
        {
            if (hoveredInteractable is not null && hoveredInteractable.gameObject == item) 
                hoveredInteractable = null;
            interactables.Remove(item);
        };
        EventManager.E_Crafting.modeChanged += (mode) => craftingMode = mode;
    }

    void Update()
    {
        // Checks if the cursor is hovering over an interactable object
        Ray ray = rm.mainCamera.ScreenPointToRay(rm.GetMousePosition());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, craftingMode ? craftItemLayer : interactLayer))
        {
            if (hoveredInteractable is null) // Mouse Enter
            {
                hoveredInteractable = hitInfo.transform.gameObject;
                interactables[hoveredInteractable].MouseEnter();
            }

            if (hoveredInteractable != hitInfo.transform.gameObject) // Different item hovered
            {
                interactables[hoveredInteractable].MouseExit();
                hoveredInteractable = hitInfo.transform.gameObject;
            }
            
            // Interact - Mouse Click
            if (Input.GetKeyDown(KeyCode.Mouse0)) interactables[hitInfo.transform.gameObject].Interact();
            // Mouse Held
            else if (Input.GetKey(KeyCode.Mouse0)) interactables[hitInfo.transform.gameObject].MouseDown();
            // Mouse Released
            if (Input.GetKeyUp(KeyCode.Mouse0)) interactables[hitInfo.transform.gameObject].MouseReleased();
        }
        else if (hoveredInteractable is not null) // Mouse Exit
        {
            interactables[hoveredInteractable].MouseExit();
            hoveredInteractable = null;
        }
    }
}
