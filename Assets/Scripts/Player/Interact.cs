using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IInteractable
{
    void Interact();
    void MouseEnter();
    void MouseExit();
}

/// <summary>
/// Interact class used for the player.
/// Checks to see if the player interacts with an interactable object, hovers over an interactable object, etc. 
/// </summary>
public class Interact : MonoBehaviour
{
    public LayerMask interactLayer;
    public ResolutionManager rm;

    private Dictionary<GameObject, IInteractable> interactables;
    private GameObject hoveredInteractable; 
    
    void Start()
    {
        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();

        interactables = new Dictionary<GameObject, IInteractable>();
        
        IEnumerable<IInteractable> interactableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>();
        foreach (IInteractable interactable in interactableObjects)
        {
            interactables.Add(((MonoBehaviour)interactable).gameObject, interactable);
        }
    }

    void Update()
    {
        Ray ray = rm.mainCamera.ScreenPointToRay(rm.GetMousePosition());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, interactLayer))
        {
            
            if (hoveredInteractable is null) // Mouse Enter
            {
                hoveredInteractable = hitInfo.transform.gameObject;
                interactables[hoveredInteractable].MouseEnter();
            }

            // Interact - Mouse Click
            if (Input.GetKeyDown(KeyCode.Mouse0)) interactables[hitInfo.transform.gameObject].Interact();
        }
        else if (hoveredInteractable is not null) // Mouse Exit
        {
            interactables[hoveredInteractable].MouseExit();
            hoveredInteractable = null;
        }
    }
}
