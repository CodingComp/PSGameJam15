using System.Collections;
using UnityEngine;

public class CraftingLever : MonoBehaviour, IInteractable
{
    public Transform leverTransform;
    public Crafting crafting;

    private const float MaxRot = 135.0f;
    private const float MinRot = 0.0f;
    private bool _canUseLever = true;
    private ResolutionManager _rm;

    private Vector3 _startMousePos;
    private Quaternion _startRotation;

    private void Start()
    {
        _rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        _startRotation = leverTransform.rotation;
    }

    public void Interact()
    {
        _startMousePos = _rm.GetMousePosition();
    }

    public void MouseEnter()
    {

    }

    public void MouseExit()
    {

    }

    public void MouseDown()
    {
        Vector3 movementDiff = _startMousePos - _rm.GetMousePosition();

        if (!_canUseLever) return;

        // Checks if lever is in down position
        if (movementDiff.y >= MaxRot)
        {
            crafting.Craft();
            _canUseLever = false;
            MouseReleased();
            return;
        }
        if (movementDiff.y > MaxRot || movementDiff.y < MinRot) return;

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
            leverTransform.rotation = Quaternion.Lerp(leverTransform.rotation, _startRotation, time);
            time += Time.deltaTime;
            yield return null;
        }
        _canUseLever = true;
    }
}
