using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData;

    [Header("Light Visualizer")] [SerializeField]
    private Transform lightPivot;

    [SerializeField] private List<Light> lights;

    [SerializeField] private Color hoverColor;
    [SerializeField] private float hoverIntensity;
    private const float RotSpeed = 10.0f;

    private Color _baseColor;
    private float _baseIntensity;
    private float _currentIntensity;

    private Color _currentLightColor;
    private Quaternion _lightRotTarget;

    private void Start()
    {
        GameObject item = Instantiate(itemData.mesh, transform);
        item.transform.localPosition = new Vector3();

        item.AddComponent<BoxCollider>();

        _baseColor = lights[0].color;
        _baseIntensity = lights[0].intensity;
        _currentLightColor = _baseColor;

        _lightRotTarget = quaternion.Euler(0, 0, 0);
        EventManager.E_Player.playerRotated += newRot => _lightRotTarget = newRot;
    }

    private void Update()
    {
        lightPivot.transform.rotation = Quaternion.Lerp(lightPivot.transform.rotation, _lightRotTarget, Time.deltaTime * RotSpeed);
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
        _currentLightColor = hoverColor;
        _currentIntensity = hoverIntensity;

        UpdateLights();
    }

    public void MouseExit()
    {
        _currentLightColor = _baseColor;
        _currentIntensity = _baseIntensity;

        UpdateLights();
    }

    public void MouseDown()
    {

    }

    public void MouseReleased()
    {

    }

    /// <summary>
    ///     Callback from the player inventory, called when the item was successfully added to the player's inventory.
    /// </summary>
    public void ItemAdded()
    {
        print("a");
        EventManager.E_Item.itemDestroyed.Invoke(gameObject);
        lights = new List<Light>();
        Destroy(gameObject);
    }

    /// <summary>
    ///     Callback from the player inventory, called when the item couldn't be added to the player's inventory.
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
            l.color = _currentLightColor;
            l.intensity = _currentIntensity;
        }
    }
}
