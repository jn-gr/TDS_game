// This code was adapted from Unity-RTS-Camera by PanMig
// Original source: https://github.com/PanMig/Unity-RTS-Camera/tree/master
// License: MIT

using UnityEngine;

[RequireComponent(typeof(Camera))]

public class RTSCameraController : MonoBehaviour
{
    [Header("Keyboard Only (Only Enable During Development)")]
    [Space]
    public bool keyboardOnly = false;

    [Header("Screen Edge Border Thickness")]
    [Space]
    public float ScreenEdgeBorderThickness = 5.0f; // distance from screen edge. Used for mouse movement

    [Header("Camera Mode")]
    [Space]
    public bool RTSMode = true;
    public bool FlyCameraMode = false;

    [Header("Movement Speeds")]
    [Space]
    public float minPanSpeed;
    public float maxPanSpeed;
    public float secToMaxSpeed; //seconds taken to reach max speed;
    public float zoomSpeed;

    [Header("Movement Limits")]
    [Space]
    public bool enableMovementLimits;
    public Vector2 heightLimit;
    public Vector2 lenghtLimit;
    public Vector2 widthLimit;
    public Vector2 zoomLimit = new Vector2(14,56);

    private float panSpeed;
    private Vector3 initialPos;
    private Vector3 panMovement;
    private Vector3 pos;
    private Quaternion rot;
    private bool rotationActive = false;
    private Vector3 lastMousePosition;
    private Quaternion initialRot;
    private float panIncrease = 0.0f;

    [Header("Rotation")]
    [Space]
    public bool rotationEnabled;
    public float rotateSpeed;





    // Use this for initialization
    void Start()
    {
        initialPos = transform.position;
        initialRot = transform.rotation;
    }


    void Update()
    {

        # region Camera Mode

        //check that ony one mode is choosen
        if (RTSMode == true) FlyCameraMode = false;
        if (FlyCameraMode == true) RTSMode = false;

        #endregion

        #region Movement

        panMovement = Vector3.zero;

        // Need orthographic forward, left, right sinec it's an orthographic camera and Vector3.forward doesn't make sense for it.
        Vector3 orthographicForward = new Vector3(1, 0, 1).normalized;
        Vector3 orthographicLeft = new Vector3(-1, 0, 1).normalized;
        Vector3 orthographicRight = new Vector3(1, 0, -1).normalized;


        if (Input.GetKey(KeyCode.W) || (!keyboardOnly && (Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness)))
        {
            panMovement += orthographicForward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S) || (!keyboardOnly && (Input.mousePosition.y <= ScreenEdgeBorderThickness)))
        {
            panMovement -= orthographicForward * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A) || (!keyboardOnly && (Input.mousePosition.x <= ScreenEdgeBorderThickness)))
        {
            panMovement += orthographicLeft * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D) || (!keyboardOnly && (Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness)))
        {
            panMovement += orthographicRight * panSpeed * Time.deltaTime;
            //pos.x += panSpeed * Time.deltaTime;
        }

        // Removed Q and E since doesnt make sense for orthographic camera 

        //if (Input.GetKey(KeyCode.Q))
        //{
        //    panMovement += Vector3.up * panSpeed * Time.deltaTime;
        //}
        //if (Input.GetKey(KeyCode.E))
        //{
        //    panMovement += Vector3.down * panSpeed * Time.deltaTime;
        //}

        if (RTSMode) transform.Translate(panMovement, Space.World);
        else if (FlyCameraMode) transform.Translate(panMovement, Space.Self);


        //increase pan speed
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)
            || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)
            || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Q)
            || Input.mousePosition.y >= Screen.height - ScreenEdgeBorderThickness
            || Input.mousePosition.y <= ScreenEdgeBorderThickness
            || Input.mousePosition.x <= ScreenEdgeBorderThickness
            || Input.mousePosition.x >= Screen.width - ScreenEdgeBorderThickness)
        {
            panIncrease += Time.deltaTime / secToMaxSpeed;
            panSpeed = Mathf.Lerp(minPanSpeed, maxPanSpeed, panIncrease);
        }
        else
        {
            panIncrease = 0;
            panSpeed = minPanSpeed;
        }

        #endregion

        #region Zoom

        // This got changed to orthographic. Because Orthographic camera. Use fieldOfView if perspective camera.
        Camera.main.orthographicSize -= Input.mouseScrollDelta.y * zoomSpeed;
        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, zoomLimit.x, zoomLimit.y);

        #endregion

        #region mouse rotation

        if (rotationEnabled)
        {
            // Mouse Rotation
            if (Input.GetMouseButton(0))
            {
                rotationActive = true;
                Vector3 mouseDelta;
                if (lastMousePosition.x >= 0 &&
                    lastMousePosition.y >= 0 &&
                    lastMousePosition.x <= Screen.width &&
                    lastMousePosition.y <= Screen.height)
                    mouseDelta = Input.mousePosition - lastMousePosition;
                else
                {
                    mouseDelta = Vector3.zero;
                }
                var rotation = Vector3.up * Time.deltaTime * rotateSpeed * mouseDelta.x;
                rotation += Vector3.left * Time.deltaTime * rotateSpeed * mouseDelta.y;

                transform.Rotate(rotation, Space.World);

                // Make sure z rotation stays locked
                rotation = transform.rotation.eulerAngles;
                rotation.z = 0;
                transform.rotation = Quaternion.Euler(rotation);
            }

            if (Input.GetMouseButtonUp(0))
            {
                rotationActive = false;
                if (RTSMode) transform.rotation = Quaternion.Slerp(transform.rotation, initialRot, 0.5f * Time.time);
            }

            lastMousePosition = Input.mousePosition;

        }


        #endregion


        #region boundaries

        if (enableMovementLimits == true)
        {
            //movement limits
            pos = transform.position;
            pos.y = Mathf.Clamp(pos.y, heightLimit.x, heightLimit.y);
            pos.z = Mathf.Clamp(pos.z, lenghtLimit.x, lenghtLimit.y);
            pos.x = Mathf.Clamp(pos.x, widthLimit.x, widthLimit.y);
            transform.position = pos;
        }



        #endregion

    }

}


