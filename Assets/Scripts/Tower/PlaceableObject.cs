using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaceableObject : MonoBehaviour
{
    public bool Placed {  get; private set; }
    public Vector3Int Size { get; private set; }
    
    private GameManager gameManager;

    

    private void Start()
    {
        //GetColliderVertexPositionsLocal();
        //CalculateSizeInCells();
        gameManager = FindFirstObjectByType<GameManager>();

    }

    public virtual void Place()
    {
        ObjectDrag drag = gameObject.GetComponent<ObjectDrag>();
        Destroy(drag);
        gameObject.GetComponent<Tower>().placed = true;
        gameManager.currency -= 100;
        //Overide here for building timers, or non functional buildings
        //Invoke events for placement
    }

}
