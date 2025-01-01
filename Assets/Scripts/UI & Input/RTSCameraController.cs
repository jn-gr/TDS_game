// This code was adapted from Unity-RTS-Camera by PanMig
// Original source: https://github.com/PanMig/Unity-RTS-Camera/tree/master
// License: MIT

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class RTSCameraController : MonoBehaviour
{
    [Header("Keyboard Only (Only Enable During Development)")]
    [Space]
    public bool keyboardOnly = false;

    [Header("Screen Edge Border Thickness")]
    [Space]
    public float ScreenEdgeBorderThickness = 5.0f;

    [Header("Camera Mode")]
    [Space]
    public bool RTSMode = true;
    public bool FlyCameraMode = false;

    [Header("Movement Speeds")]
    [Space]
    public float minPanSpeed;
    public float maxPanSpeed;
    public float secToMaxSpeed;
    public float zoomSpeed;
    public float dragSpeed = 2.0f;
    public float dragZoomScale = 0.1f;

    [Header("Movement Limits")]
    [Space]
    public bool enableMovementLimits;
    public Vector2 heightLimit;
    public Vector2 lenghtLimit;
    public Vector2 widthLimit;
    public Vector2 zoomLimit = new Vector2(14, 56);

    [Header("Rotation")]
    [Space]
    public bool rotationEnabled;
    public float rotateSpeed;

    // Input System variables
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction zoomAction;
    private InputAction rotateAction;
    private InputAction rotatePressAction;
    private InputAction dragPressAction;
    private InputAction mousePositionAction;

    private float panSpeed;
    private Vector3 initialPos;
    private Vector3 panMovement;
    private Vector3 pos;
    private Quaternion rot;
    private bool rotationActive = false;
    private Vector3 lastMousePosition;
    private Quaternion initialRot;
    private float panIncrease = 0.0f;
    private bool isDragging = false;
    private Camera cam;
    private Vector2 currentMousePosition;
    private Vector2 moveInput;
    private Vector2 zoomInput;

    void Awake()
    {
        // Get references to input actions
        playerInput = GetComponent<PlayerInput>();
        InputActionMap actionMap = playerInput.actions.FindActionMap("RTS Camera");
        
        moveAction = actionMap.FindAction("Move");
        zoomAction = actionMap.FindAction("Zoom");
        rotateAction = actionMap.FindAction("Rotate");
        rotatePressAction = actionMap.FindAction("RotatePress");
        dragPressAction = actionMap.FindAction("DragPress");
        mousePositionAction = actionMap.FindAction("MousePosition");

        // Set up input callbacks
        moveAction.performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        moveAction.canceled += ctx => OnMove(Vector2.zero);
        
        zoomAction.performed += ctx => OnZoom(ctx.ReadValue<Vector2>());
        
        dragPressAction.started += ctx => OnDragStart();
        dragPressAction.canceled += ctx => OnDragEnd();
        
        rotatePressAction.started += ctx => OnRotateStart();
        rotatePressAction.canceled += ctx => OnRotateEnd();
        
        mousePositionAction.performed += ctx => currentMousePosition = ctx.ReadValue<Vector2>();
    }

    void OnEnable()
    {
        moveAction.Enable();
        zoomAction.Enable();
        rotateAction.Enable();
        rotatePressAction.Enable();
        dragPressAction.Enable();
        mousePositionAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        zoomAction.Disable();
        rotateAction.Disable();
        rotatePressAction.Disable();
        dragPressAction.Disable();
        mousePositionAction.Disable();
    }

    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        #region Camera Mode
        if (RTSMode) FlyCameraMode = false;
        if (FlyCameraMode) RTSMode = false;
        #endregion

        #region Movement
        panMovement = Vector3.zero;
        Vector3 orthographicForward = new Vector3(1, 0, 1).normalized;
        Vector3 orthographicLeft = new Vector3(-1, 0, 1).normalized;
        Vector3 orthographicRight = new Vector3(1, 0, -1).normalized;

        if (isDragging)
        {
            Vector3 delta = (Vector3)currentMousePosition - lastMousePosition;
            float zoomSpeedMultiplier = cam.orthographicSize * dragZoomScale;
            Vector3 dragMovement = orthographicForward * -delta.y + orthographicRight * -delta.x;
            transform.Translate(dragMovement * dragSpeed * zoomSpeedMultiplier * Time.deltaTime, Space.World);
        }
        else if (!keyboardOnly)
        {
            // Screen edge scrolling
            if (currentMousePosition.y >= Screen.height - ScreenEdgeBorderThickness)
                panMovement += orthographicForward;
            if (currentMousePosition.y <= ScreenEdgeBorderThickness)
                panMovement -= orthographicForward;
            if (currentMousePosition.x <= ScreenEdgeBorderThickness)
                panMovement += orthographicLeft;
            if (currentMousePosition.x >= Screen.width - ScreenEdgeBorderThickness)
                panMovement += orthographicRight;
        }

        // Add movement from WASD input
        panMovement += new Vector3(moveInput.x, 0, moveInput.y);

        if (panMovement.magnitude > 0)
        {
            panIncrease += Time.deltaTime / secToMaxSpeed;
            panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panIncrease);
        }
        else
        {
            panIncrease = 0;
            panSpeed = minPanSpeed;
        }

        if (RTSMode)
            transform.Translate(panMovement * panSpeed * Time.deltaTime, Space.World);
        else if (FlyCameraMode)
            transform.Translate(panMovement * panSpeed * Time.deltaTime, Space.Self);

        lastMousePosition = currentMousePosition;
        #endregion

        #region boundaries
        if (enableMovementLimits)
        {
            pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, heightLimit.x, heightLimit.y);
            pos.z = Mathf.Clamp(pos.z, lenghtLimit.x, lenghtLimit.y);
            pos.x = Mathf.Clamp(pos.x, widthLimit.x, widthLimit.y);
            transform.position = pos;
        }
        #endregion
    }

    private void OnMove(Vector2 value)
    {
        moveInput = value;
    }

    private void OnZoom(Vector2 value)
    {
        Camera.main.orthographicSize -= value.y * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomLimit.x, zoomLimit.y);
    }

    private void OnDragStart()
    {
        isDragging = true;
        lastMousePosition = currentMousePosition;
    }

    private void OnDragEnd()
    {
        isDragging = false;
    }

    private void OnRotateStart()
    {
        if (rotationEnabled && !isDragging)
        {
            rotationActive = true;
            lastMousePosition = currentMousePosition;
        }
    }

    private void OnRotateEnd()
    {
        if (rotationEnabled)
        {
            rotationActive = false;
            if (RTSMode)
                transform.rotation = Quaternion.Slerp(transform.rotation, initialRot, 0.5f * Time.time);
        }
    }
}
