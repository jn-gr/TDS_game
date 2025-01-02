using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class BuildingSystem : MonoBehaviour
{
    public static BuildingSystem current;

    //public GridLayout gridLayout;
    //private Grid grid;
    //[SerializeField] private Tilemap MainTilemap;
    //[SerializeField] private TileBase whiteTile;

    //public LayerMask layersToHit;
    ////Place Prefabs Here
    //public GameObject NothingObject;
    //public GameObject House_01;
    //public GameObject Castle;
    private MapManager mapManager;
    public GameManager gameManager;

    public PlaceableObject objectToPlace;

    private Tilemap grid;

    public ToastPanel toastPanel;

    #region Unity methods

    private void Awake()
    {
        mapManager = GetComponent<MapManager>();
        gameManager = FindFirstObjectByType<GameManager>();
        current = this;
        grid = mapManager.tilemap;
    }

    private void Update() //Selection of buildings (currently with keyboard buttons
    {

        if (!objectToPlace)
        {
            return;
        }
        if (objectToPlace != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane tilemapPlane = new Plane(Vector3.up, Vector3.zero);
                if (tilemapPlane.Raycast(ray, out float enter))
                {
                    Vector3 worldPosition = ray.GetPoint(enter);
                    Vector3Int cellPosition = grid.WorldToCell(worldPosition);
                   
                    if (mapManager.globalMap.TryGetValue((cellPosition.x, cellPosition.y), out CellT clickedCell))
                    {
                        PlaceObject(clickedCell);
                    }
                    else
                    {
                        Debug.Log("cant place here");
                        Destroy(objectToPlace.gameObject);
                        objectToPlace = null;
                    }


                }
               
                
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Destroy(objectToPlace.gameObject);
            }
        }
    }

    // Hackyish solution. Just copied the InputKeyDownAlpha1 and put it here. Then connected the button to this method.
    // Currentyl no way to tell UI that theres insufficient gold outside the console.
    public void PlaceObject( GameObject tower) 
    {
        
        if (gameManager.currency >= 100)
        {
            Debug.Log("hello");
            InitializeWithObject(tower);
            
        }
        else
        {
            Debug.Log("insufficient gold");
            toastPanel.ShowMessage("Insufficient Gold");
        }
    }

    #endregion

    #region Utils

    public static Vector3 GetMouseWorldPosition()  //Cast a ray from the camera to align building to mouse pointer
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //if (Physics.Raycast(ray, out RaycastHit raycastHit, 1000, BuildingSystem.current.layersToHit))
        //{
        //    //Debug.Log("Ray hit: " + raycastHit.collider.name);
        //    return raycastHit.point;
        //}
        //else
        //{
        //    return Vector3.zero;
        //}
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane tilemapPlane = new Plane(Vector3.up, Vector3.zero);
        if (tilemapPlane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }
        else
        {
            return Vector3.zero;
        }
    }

    public Vector3 SnapCoordinateToGrid(Vector3 position)  //Snap the cast ray to the grid
    {
        Vector3Int cellPos = grid.WorldToCell(position);
        position = grid.GetCellCenterLocal(cellPos);
        return position;
    }

    //private static TileBase[] GetTilesBlock(BoundsInt area, Tilemap tilemap)
    //{
    //    TileBase[] array = new TileBase[area.size.x * area.size.y * area.size.z];
    //    int counter = 0;

    //    foreach (var v in area.allPositionsWithin)
    //    {
    //        Vector3Int pos = new Vector3Int(v.x, v.y, 0);
    //        array[counter] = tilemap.GetTile(pos);
    //        counter++;
    //    }
    //    return array;
    //}

    #endregion

    #region Building Placement

    public void InitializeWithObject(GameObject prefab) //Show tower when selected
    {
        //if (objectToPlace != null) // this will destory the building that hasnt been locked in a place, so that the player doesnt end up having many unplaced towers in the scene
        //{
          
        //    Destroy(objectToPlace.gameObject);
            
        //}
        
        Vector3 positon = SnapCoordinateToGrid(GetMouseWorldPosition());

        GameObject obj = Instantiate(prefab, positon, Quaternion.identity);
        objectToPlace = obj.GetComponent<PlaceableObject>();
        obj.AddComponent<ObjectDrag>();
    }

    private void PlaceObject(CellT cell)
    {
        if(cell.objectPlacedOnCell == null && !cell.IsWalkable)
        {
            objectToPlace.Place();
            cell.objectPlacedOnCell = objectToPlace.gameObject;
            objectToPlace.GetComponent<Tower>().cellPlacedOn = cell;
            objectToPlace = null;
            
        }
        else
        {
            
            Destroy(objectToPlace.gameObject);
            objectToPlace = null;
            toastPanel.ShowMessage("Can't Place Tower Here");
        }

        Debug.Log($"{cell.objectPlacedOnCell} has been placed on {cell.X},{cell.Y}");
       
        //BoundsInt area = new BoundsInt();
        //area.position = gridLayout.WorldToCell(objectToPlace.GetStartPosition());
        //area.size = placeableObject.Size;

        //TileBase[] baseArray = GetTilesBlock(area, MainTilemap);

        //foreach (var b in baseArray){
        //    if (b == whiteTile)
        //    {
        //        return false;
        //    }
        //}
        //return true;
    }

    

    #endregion

}
