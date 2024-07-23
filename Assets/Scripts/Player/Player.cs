using System.Collections;
using System.Collections.Generic;
using Cinemachine;
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
    public LayerMask groundMask;
    public Transform orientation;
    public GameObject lookDirectionVisualizer;
    public GameObject cameraPivot;
    
    private Rigidbody rb;
    private ResolutionManager rm;

    private Quaternion targetRot;
    private float camRotSpeed = 10.0f;

    public bool isBusy = false;
    private bool flashlightStateBeforeAction = false;
    
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        rb = GetComponent<Rigidbody>();
        
        targetRot = cameraPivot.transform.rotation;
    }

    void FixedUpdate()
    {
        moveDir = orientation.forward * verticalInput + orientation.right * horizontalInput;
        rb.AddForce(moveDir.normalized * (moveSpeed * 10.0f), ForceMode.Force);
    }
    
    void Update()
    {
        if (isBusy) return;
        
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
        // Look Dir
        Ray ray = rm.mainCamera.ScreenPointToRay(rm.GetMousePosition());
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

    /// <summary>
    /// Used when an action is made by the player where the player shouldn't be able to move / look around, interact, etc.
    /// </summary>
    public void StartAction()
    {
        isBusy = true;
        flashlightStateBeforeAction = flashlight.activeSelf;
        flashlight.SetActive(false);
    }

    /// <summary>
    /// Used when an action the player was doing is over. Allowing the player to move / look around again.
    /// </summary>
    public void EndAction()
    {
        isBusy = false;
        flashlight.SetActive(flashlightStateBeforeAction);
    }

}
