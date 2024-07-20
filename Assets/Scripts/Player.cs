using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject mouseTestObj;
    public Vector3 mousePos;
    
    public GameObject playerModel;
    public Transform orientation;

    public LayerMask groundMask;
    public LayerMask enemyMask;
    private Vector3 moveDir;
    public float moveSpeed = 10.0f;
    
    private int resX = 640;
    private int resY = 360;
    private float resScaleX;
    private float resScaleY;
    
    private Camera camera;
    private Rigidbody rb;
    
    private float horizontalInput;
    private float verticalInput;

    public GameObject flashlight;

    private Vector3 camWorldBoundry;
    private float pixelSize;
    
    private Dictionary<GameObject, LightTest> visibleEnemies;

    private int rayPaddingX = 10;
    private int rayPaddingY = 10;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        camera = Camera.main;
        rb = GetComponent<Rigidbody>();
        
        // Sets orientation to the camera view. Camera view Up is forward.
        orientation.transform.Rotate(Vector3.up, -45.0f);
        
        // RENDER TEXTURE RESOLUTION: 640 x 360
        resScaleX = (float)resX / Screen.width;
        resScaleY = (float)resY / Screen.height;

        // Pixel Testing
        camWorldBoundry = new Vector2(camera.orthographicSize * 2 * Screen.width / Screen.height, camera.orthographicSize * 2);
        pixelSize = camWorldBoundry.x / resX;

        visibleEnemies = new Dictionary<GameObject, LightTest>();
    }

    void FixedUpdate()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDir.normalized * moveSpeed * 10.0f, ForceMode.Force);
    }

    private bool done = false;
    void Update()
    {
        // Temp flashlight testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.SetActive(!flashlight.activeSelf);
        }
        
        Movement();
        
        Texture2D cameraRender = new Texture2D(resX, resY, TextureFormat.RGB24, false);
        RenderTexture.active = camera.activeTexture;
        cameraRender.ReadPixels(new Rect(0, 0, resX, resY), 0, 0);
        cameraRender.Apply();
        
        foreach (KeyValuePair<GameObject, LightTest> enemy in visibleEnemies)
        {
            GameObject e = enemy.Key;
                
            Vector3 originScreenPos = camera.WorldToScreenPoint(e.transform.position);
            originScreenPos.z = 0;

            bool inLight = false;
                
            // Break if any pixel is in light?
            for (int i = (int)originScreenPos.y - rayPaddingY; i < (int)originScreenPos.y + rayPaddingY; i++)
            {
                for (int j = (int)originScreenPos.x - rayPaddingX; j < (int)originScreenPos.x + rayPaddingX; j++)
                {
                        
                    Vector3 rPos = camera.ScreenToWorldPoint(new Vector3(j, i, 0));
                    rPos += camera.transform.right * pixelSize / 2;
                    rPos += camera.transform.up * pixelSize / 2;
                        
                    // Debug.DrawLine(rPos, rPos + camera.transform.forward * 20);
                        
                    if (Physics.Raycast(rPos, camera.transform.forward, out var hitInfo, 100.0f,
                            enemyMask))
                    {
                        if ((0.2126 * cameraRender.GetPixel(j, i).r + 0.7152 * cameraRender.GetPixel(j, i).g +
                             0.0722 * cameraRender.GetPixel(j, i).b) > 0.01f) inLight = true;
                    }
                }
            }

            enemy.Value.inLight = inLight;
        }
    }

    void Movement()
    {
        // Mouse Pos * Resolution Scale to get accurate position
        mousePos = Input.mousePosition;
        mousePos.x *= resScaleX;
        mousePos.y *= resScaleY;
        
        // Look Dir
        Ray ray = camera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Vector3 dir = hitInfo.point - transform.position;
            dir.y = 0;
            
            mouseTestObj.transform.position = hitInfo.point;
            playerModel.transform.forward = dir;
        }

        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    public void EnemyVisible(GameObject enemyObj, LightTest lt)
    {
        visibleEnemies.Add(enemyObj, lt);
    }

    public void EnemyLeftVisibility(GameObject enemyObj)
    {
        visibleEnemies.Remove(enemyObj);
    }
}
