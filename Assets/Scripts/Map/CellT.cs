using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellT
{
    public int X { get; set; }
    public int Y { get; set; }

    public bool IsOpenUp { get; set; }
    public bool IsOpenDown { get; set; }
    public bool IsOpenLeft { get; set; }
    public bool IsOpenRight { get; set; }

    public GameObject objectPlacedOnCell;

    public CellT(int x, int y)
    {
        X = x;
        Y = y;
        IsOpenDown = false;
        IsOpenLeft = false;
        IsOpenRight = false;
        IsOpenUp = false;
    }
    public bool IsWalkable
    {
        get
        {
            // cell is considered walkable if it has at least one open direction
            return IsOpenUp || IsOpenDown || IsOpenLeft || IsOpenRight;
        }
    }
}
