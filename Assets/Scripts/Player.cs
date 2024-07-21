using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Main Player Class. Used to control movement, input actions from user, etc.
/// </summary>
public class Player : MonoBehaviour
{
    public GameObject playerModel;
    public GameObject flashlight;

    [Header("Movement")]
    public float moveSpeed = 10.0f;
    private Vector3 moveDir;
    private float horizontalInput;
    private float verticalInput;

    [Header("Look Direction")]
    public Vector3 mousePos;
    public LayerMask groundMask;
    public Transform orientation;
    public GameObject lookDirectionVisualizer;
    public GameObject cameraPivot;
    
    private Rigidbody rb;
    private ResolutionManager rm;

    private Quaternion targetRot;
    private float camRotSpeed = 10.0f;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        rb = GetComponent<Rigidbody>();
        
        // Sets orientation to the camera view. Camera view Up is forward.
        orientation.transform.Rotate(Vector3.up, -45.0f);

        targetRot = cameraPivot.transform.rotation;
    }

    void FixedUpdate()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDir.normalized * (moveSpeed * 10.0f), ForceMode.Force);
    }
    
    void Update()
    {
        // Temp flashlight testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            flashlight.SetActive(!flashlight.activeSelf);
        }

        // Rotate Camera
        if (Input.GetKeyDown(KeyCode.E)) RotateCamera(Quaternion.Euler(0,  90f, 0)); // Right
        if (Input.GetKeyDown(KeyCode.Q)) RotateCamera(Quaternion.Euler(0, -90f, 0)); // Left
        
        Movement();
        
        cameraPivot.transform.rotation = Quaternion.Lerp(cameraPivot.transform.rotation, targetRot, Time.deltaTime * camRotSpeed);
    }

    void RotateCamera(Quaternion rot)
    {
        targetRot *= rot;
    }

    void Movement()
    {
        // Mouse Pos * Resolution Scale to get accurate position
        mousePos = Input.mousePosition;
        mousePos.x *= rm.resScaleX;
        mousePos.y *= rm.resScaleY;
        
        // Look Dir
        Ray ray = rm.mainCamera.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Vector3 dir = hitInfo.point - transform.position;
            dir.y = 0;
            
            lookDirectionVisualizer.transform.position = hitInfo.point;
            playerModel.transform.forward = dir;
        }
        
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
}
