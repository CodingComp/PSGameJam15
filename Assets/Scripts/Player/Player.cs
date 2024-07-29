using UnityEngine;

/// <summary>
///     Main Player Class. Used to control movement, input actions from user, etc.
/// </summary>
public class Player : MonoBehaviour
{
    private const float CamRotSpeed = 10.0f;
    public GameObject playerModel;
    public PlayerInventory inventory;
    public PlayerHealth playerHealth;
    public EquipmentManager eqm;

    [Header("Movement")] 
    public float moveSpeed = 10.0f;

    [Header("Look Direction")] 
    public LayerMask groundMask;

    public Transform orientation;
    public GameObject lookDirectionVisualizer;
    public GameObject cameraPivot;

    public bool isBusy;
    private bool _flashlightStateBeforeAction;
    private float _horizontalInput;
    private Vector3 _moveDir;

    private Rigidbody _rb;
    private ResolutionManager _rm;

    private Quaternion _targetRot;
    private float _verticalInput;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        _rm = GameObject.Find("GameManager").GetComponent<ResolutionManager>();
        _rb = GetComponent<Rigidbody>();
        eqm = GetComponent<EquipmentManager>();
        inventory = GetComponent<PlayerInventory>();
        playerHealth = GetComponent<PlayerHealth>();

        _targetRot = cameraPivot.transform.rotation;
    }

    private void Update()
    {
        if (isBusy) return;

        // Rotate Camera
        if (Input.GetKeyDown(KeyCode.E)) RotateCamera(Quaternion.Euler(0, 90f, 0)); // Right
        if (Input.GetKeyDown(KeyCode.Q)) RotateCamera(Quaternion.Euler(0, -90f, 0)); // Left

        Movement();

        cameraPivot.transform.rotation = Quaternion.Lerp(cameraPivot.transform.rotation, _targetRot, Time.deltaTime * CamRotSpeed);
    }

    private void FixedUpdate()
    {
        if (isBusy) return;

        _moveDir = orientation.forward * _verticalInput + orientation.right * _horizontalInput;
        _rb.AddForce(_moveDir.normalized * (moveSpeed * 10.0f), ForceMode.Force);
    }

    private void RotateCamera(Quaternion rot)
    {
        _targetRot *= rot;
        EventManager.E_Player.playerRotated.Invoke(_targetRot);
    }

    private void Movement()
    {
        // Look Dir
        Ray ray = _rm.mainCamera.ScreenPointToRay(_rm.GetMousePosition());
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            Vector3 dir = hitInfo.point - transform.position;
            dir.y = 0;

            lookDirectionVisualizer.transform.position = hitInfo.point;
            playerModel.transform.forward = dir;
        }

        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");
    }

    /// <summary>
    ///     Used when an action is made by the player where the player shouldn't be able to move / look around, interact, etc.
    /// </summary>
    public void StartAction()
    {
        isBusy = true;
        _flashlightStateBeforeAction = eqm.flashlight.activeSelf;
        eqm.flashlight.SetActive(false);
    }

    /// <summary>
    ///     Used when an action the player was doing is over. Allowing the player to move / look around again.
    /// </summary>
    public void EndAction()
    {
        isBusy = false;
        eqm.flashlight.SetActive(_flashlightStateBeforeAction);
    }
}
