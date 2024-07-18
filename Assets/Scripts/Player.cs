using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject mouseTestObj;
    public Vector3 mousePos;
    
    public GameObject playerModel;
    public Transform orientation;

    public LayerMask groundMask;
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
    }
    
    void FixedUpdate()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDir.normalized * moveSpeed * 10.0f, ForceMode.Force);
    }
    
    void Update()
    {
        // Temp flashlight testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.SetActive(!flashlight.activeSelf);
        }

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
}
