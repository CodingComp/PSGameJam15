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
///     Interact class used for the player.
///     Checks to see if the player interacts with an interactable object, hovers over an interactable object, etc.
/// </summary>
public class Interact : MonoBehaviour
{
    public LayerMask interactLayer;
    public LayerMask craftItemLayer;
    public ResolutionManager rm;
    private bool _craftingMode;

    private IInteractable _currentInteractable;
    private GameObject _hoveredInteractable;

    private Dictionary<GameObject, IInteractable> _interactables;

    private void Awake()
    {
        _interactables = new Dictionary<GameObject, IInteractable>();
        // Events
        EventManager.E_Item.itemCreated += item =>
        {
            _interactables.Add(item, item.GetComponent<IInteractable>());
        };
        EventManager.E_Item.itemDestroyed += item =>
        {
            if (_hoveredInteractable is not null && _hoveredInteractable.gameObject == item)
                _hoveredInteractable = null;
            _interactables.Remove(item);
        };
        EventManager.E_Crafting.modeChanged += mode => _craftingMode = mode;
        EventManager.E_Crafting.resetInteractable += () => _currentInteractable = null;
    }

    private void Start()
    {
        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();

        // Gets all interactable objects already placed in the world 
        IEnumerable<IInteractable> interactableObjects = FindObjectsOfType<MonoBehaviour>().OfType<IInteractable>();
        foreach (IInteractable interactable in interactableObjects)
        {
            _interactables.Add(((MonoBehaviour)interactable).gameObject, interactable);
        }
    }

    private void Update()
    {
        if (_currentInteractable is not null)
        {
            // Mouse Held
            if (Input.GetKey(KeyCode.Mouse0)) _currentInteractable.MouseDown();
            // Mouse Released
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                _currentInteractable.MouseReleased();
                _currentInteractable = null;
            }

            return;
        }

        // Checks if the cursor is hovering over an interactable object
        Ray ray = rm.mainCamera.ScreenPointToRay(rm.GetMousePosition());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _craftingMode ? craftItemLayer : interactLayer))
        {
            if (_hoveredInteractable is null) // Mouse Enter
            {
                _hoveredInteractable = hitInfo.transform.gameObject;
                _interactables[_hoveredInteractable].MouseEnter();
            }

            if (_hoveredInteractable != hitInfo.transform.gameObject) // Different item hovered
            {
                _interactables[_hoveredInteractable].MouseExit();
                _hoveredInteractable = hitInfo.transform.gameObject;
                _interactables[_hoveredInteractable].MouseEnter();
            }

            // Interact - Mouse Click
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                _currentInteractable = _interactables[hitInfo.transform.gameObject];
                _currentInteractable.Interact();
            }
        }
        else if (_hoveredInteractable is not null) // Mouse Exit
        {
            _interactables[_hoveredInteractable].MouseExit();
            _hoveredInteractable = null;
        }
    }
}
