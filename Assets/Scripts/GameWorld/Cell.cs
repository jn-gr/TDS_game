using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int Position { get; set; }
    public bool IsBuildable { get; set; }
    public GameObject CurrentTower { get; set; }  // If a tower is placed on this cell

    public Cell(Vector3Int position)
    {
        Position = position;
        IsBuildable = true;  // Default to buildable
    }
}
