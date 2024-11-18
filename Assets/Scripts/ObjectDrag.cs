using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDrag : MonoBehaviour
{
    private Vector3 offset;
    private bool isDragging = true;

    private void Update()
    {
        // Check for left mouse click to toggle dragging
        if (Input.GetMouseButtonDown(0))
        {
            if (isDragging)
            {
                // Stop dragging
                isDragging = false;
            }
            else
            {
                // Start dragging if mouse is over the object
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit) && hit.transform == transform)
                {
                    // Calculate offset and enable dragging
                    offset = transform.position - BuildingSystem.GetMouseWorldPosition();
                    isDragging = true;
                }
            }
        }

        // Continue dragging if in drag mode
        if (isDragging)
        {
            Vector3 mouseWorldPosition = BuildingSystem.GetMouseWorldPosition();
            Vector3 newPosition = mouseWorldPosition + offset;

            // Snap to grid if needed
            transform.position = BuildingSystem.current.SnapCoordinateToGrid(newPosition);
        }
    }

}

