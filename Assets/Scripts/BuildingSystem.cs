using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    public GridLayout gridLayout;
    private Grid grid;
    [SerializeField] private Tilemap MainTilemap;
    [SerializeField] private TileBase whiteTile;

    public LayerMask layersToHit;
    //Place Prefabs Here
    public GameObject House_01;
    public GameObject Castle;
    public GameManager gameManager;

    public PlaceableObject objectToPlace;

    #region Unity methods

    private void Awake()
    {
        gameManager = FindFirstObjectByType<GameManager>();
        current = this;
        grid = gridLayout.gameObject.GetComponent<Grid>();
    }

    private void Update() //Selection of buildings (currently with keyboard buttons
    {

        //if (!objectToPlace)
        //{
        //    return;
        //}
        if (objectToPlace != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (CanBePlaced(objectToPlace))
                {

                    objectToPlace.Place();
                    Vector3Int start = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
                    TakeArea(start, objectToPlace.Size);
                    Turret turret = objectToPlace.GetComponent<Turret>();
                    turret.enabled = true;
                    objectToPlace = null; // we change this to null because once the object has been placed and locked we wont be able to move it
                }
                else
                {
                    Destroy(objectToPlace.gameObject);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(objectToPlace.gameObject);
            }
        }
    }

    // Hackyish solution. Just copied the InputKeyDownAlpha1 and put it here. Then connected the button to this method.
    // Currentyl no way to tell UI that theres insufficient gold outside the console.
    public void PlaceTower() 
    {
        if (gameManager.currency >= 100)
        {
            InitializeWithObject(House_01);
            gameManager.currency -= 100;
        }
        else
        {
            Debug.Log("insufficient gold");
        }
    }

    #endregion

    #region Utils

    public static Vector3 GetMouseWorldPosition()  //Cast a ray from the camera to align building to mouse pointer
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 1f);
        if (Physics.Raycast(ray, out RaycastHit raycastHit,1000,BuildingSystem.current.layersToHit))
        {

            Debug.DrawRay(raycastHit.point, Vector3.up * 1f, Color.green, 1f);
            //Debug.Log("Ray hit: " + raycastHit.collider.name);
            return raycastHit.point;
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)  //Snap the cast ray to the grid
    {
        Vector3Int cellPos = gridLayout.WorldToCell(position);
        position = grid.GetCellCenterLocal(cellPos);
        return position;
    }

    private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    {
        TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
        int counter = 0;

        foreach (var v in area.allPositionsWithin)
        {
            Vector3Int pos = new Vector3Int(v.x, v.y, 0);
            array[counter] = tilemap.GetTile(pos);
            counter++;
        }
        return array;
    }

    #endregion

    #region Building Placement

    public void InitializeWithObject(GameObject prefab) //Show tower when selected
    {
        if (objectToPlace != null) // this will destory the building that hasnt been locked in a place, so that the player doesnt end up having many unplaced towers in the scene
        {
          
            Destroy(objectToPlace.gameObject);
            
        }
        
        Vector3 positon = SnapCoordinateToGrid(Vector3.zero);

        GameObject obj = Instantiate(prefab, positon, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        Turret turret = obj.GetComponent<Turret>();
        turret.enabled = false;
        obj.AddComponent<ObjectDrag>();
    }

    private bool CanBePlaced(PlaceableObject placeableObject)
    {
        BoundsInt area = new BoundsInt();
        area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        area.size = placeableObject.Size;

        TileBase[] baseArray = GetTilesBlock(area, MainTilemap);

        foreach (var b in baseArray){
            if (b == whiteTile)
            {
                return false;
            }
        }
        return true;
    }

    public void TakeArea(Vector3Int start, Vector3Int size) //Take area around building to stop overlapping buildings
    {
        MainTilemap.BoxFill(start, whiteTile, start.x, start.y, start.x +size.x, start.y + size.y);
    }

    #endregion
}
