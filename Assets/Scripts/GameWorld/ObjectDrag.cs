using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = true;
    private PlayerInput playerInput;
    private InputAction selectAction;

    private void Awake()
    {
        playerInput = Camera.main.GetComponent<PlayerInput>();
        InputActionMap actionMap = playerInput.actions.FindActionMap("RTS Camera");
        selectAction = actionMap.FindAction("Select");
    }

    void OnEnable()
    {
        selectAction.Enable();

        selectAction.performed += ctx => DragObject();
    }

    void OnDisable()
    {
        selectAction.Disable();

        selectAction.performed -= ctx => DragObject();
    }

    private void DragObject()
    {
        if (isDragging)
        {
            // Stop dragging
            isDragging = false;
        }
        else
        {
            // Start dragging if mouse is over the object
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit) && hit.transform == transform)
            {
                // Calculate offset and enable dragging
                offset = transform.position - BuildingSystem.GetMouseWorldPosition();
                isDragging = true;
            }
        }
    }

    private void Update()
    {
        if (isDragging)
        {
            Vector3 mouseWorldPosition = BuildingSystem.GetMouseWorldPosition();
            Vector3 newPosition = mouseWorldPosition + offset;

            // Snap to grid if needed
            transform.position = BuildingSystem.Instance.SnapCoordinateToGrid(newPosition);
        }
    }

}

