using System.Collections.Generic;
using UnityEngine;

/// <summary>
///     Placed on the camera, this class is responsible for making sure the player is always visible on screen.
///     Once a wall goes over the player, a cutout is shown on the player by setting the variables used in the PlayerCutout
///     shader.
/// </summary>
public class PlayerCutout : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Transform rayStartTransform;
    
    private Camera _cam;
    private Dictionary<GameObject, Material> _wallMaterials;
    
    private void Awake()
    {
        _cam = GetComponent<Camera>();

        GameObject [] walls = GameObject.FindGameObjectsWithTag("Wall");
        _wallMaterials = new Dictionary<GameObject, Material>();

        for (int i = 0; i < walls.Length; i++)
        {
            _wallMaterials.Add(walls[i], walls[i].GetComponent<Renderer>().material);
        }
    }

    private void Update()
    {
        Vector2 cutoutPos = _cam.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= Screen.width / Screen.height;

        Vector3 offset = targetObject.position - rayStartTransform.position;
        Debug.DrawRay(rayStartTransform.position, offset);
        RaycastHit [] hitObjects = Physics.RaycastAll(rayStartTransform.position, offset, offset.magnitude, wallMask);

        foreach (KeyValuePair<GameObject, Material> wall in _wallMaterials)
        {
            wall.Value.SetVector("_CutoutPos", cutoutPos);
            wall.Value.SetFloat("_CutoutSize", 0.0f);
            wall.Value.SetFloat("_FalloffSize", 0.05f);
        }

        foreach (RaycastHit hit in hitObjects)
        {
            Material mat = _wallMaterials[hit.transform.gameObject];
            mat.SetFloat("_CutoutSize", 0.35f);
        }
    }
}
