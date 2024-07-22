using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Placed on the camera, this class is responsible for making sure the player is always visible on screen.
/// Once a wall goes over the player, a cutout is shown on the player by setting the variables used in the PlayerCutout shader. 
/// </summary>
public class PlayerCutout : MonoBehaviour
{
    [SerializeField] private Transform targetObject;
    [SerializeField] private LayerMask wallMask;
    
    private Camera cam;
    private Dictionary<GameObject, Material> wallMaterials;
    
    void Awake()
    {
        cam = GetComponent<Camera>();

        GameObject[] walls = GameObject.FindGameObjectsWithTag("Wall");
        wallMaterials = new Dictionary<GameObject, Material>();
        
        for (int i = 0; i < walls.Length; i++)
        {
            wallMaterials.Add(walls[i], walls[i].GetComponent<Renderer>().material);
        }
    }

    void Update()
    {
        Vector2 cutoutPos = cam.WorldToViewportPoint(targetObject.position);
        cutoutPos.y /= Screen.width / Screen.height;
        
        Vector3 offset = targetObject.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(transform.position, offset, offset.magnitude, wallMask);

        foreach (KeyValuePair<GameObject, Material> wall in wallMaterials)
        {
            wall.Value.SetVector("_CutoutPos", cutoutPos);
            wall.Value.SetFloat("_CutoutSize", 0.0f);
            wall.Value.SetFloat("_FalloffSize", 0.05f);
        }
        
        foreach (RaycastHit hit in hitObjects)
        {
            Material mat = wallMaterials[hit.transform.gameObject];
            mat.SetFloat("_CutoutSize", 0.2f);
        }
    }
}
