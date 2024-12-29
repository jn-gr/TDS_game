using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region
{

    public int Width {  get; set; }
    public int Height { get; set; }

    public CellT[,] Cells;

    public int RegionX { get; private set; }
    public int RegionY { get; private set; }


    // local coords within region
    public List<CellT> startCells { get;  set; } = new List<CellT>();

    // local coords within region
    public List<CellT> endCells { get; set; } = new List<CellT>();  
    

    public Region(int regionX, int regionY,int width, int height )
    {
        Width = width;
        Height = height;
        RegionX = regionX;
        RegionY = regionY;
        Cells = new CellT[width, height];
        //startCell = new CellT(0, height/2);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cells[x, y] = new CellT(x, y); 
                //Debug.Log(x + "," + y);
            }
        }
    }
    // this gets the 
    public CellT GetCellLocal(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return null;
        return Cells[x, y];
    }

    // this gets the local coordinate of cell of a region and turns into global coord of that cell
    // example if the region is in (1,2) and width and hieght is 10, and they want the (4,5) of that cell
    // the global coords are (10,20) + (4+5) = (14,25)
    public (int globalX, int globalY) LocalToGlobal(int x, int y)
    {
        return ((RegionX * Width )+ x, (RegionY * Height)+ y);
    }

    public void GeneratePath(Dictionary<(int, int), Region> regionMap, Dictionary<(int, int), CellT> globalMap)
    {
        PathGenerator.Generate(this,regionMap, globalMap, Width, Height);
    }

    
}
