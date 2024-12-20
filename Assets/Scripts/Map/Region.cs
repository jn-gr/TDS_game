using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Region
{

    public int Width {  get; set; }
    public int Height { get; set; }

    public CellT[,] Cells;

    public int OriginX { get; private set; }
    public int OriginY { get; private set; }

    

    public Region(int originX, int originY,int width, int height )
    {
        Width = width;
        Height = height;
        OriginX = OriginX;
        OriginY = OriginY;
        Cells = new CellT[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Cells[x, y] = new CellT(x, y); 
                //Debug.Log(x + "," + y);
            }
        }
    }

    public CellT GetCellLocal(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return null;
        return Cells[x, y];
    }

    public (int globalX, int globalY) LocalToGlobal(int x, int y)
    {
        return ((OriginX * Width )+ x, (OriginY * Height)+ y);
    }

    public void GeneratePath()
    {
        PathGenerator.Generate(Cells, Width, Height);
    }
}
