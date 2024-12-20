using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class PathGenerator
{
    //{
    //    // Generates a single random path from the left edge to the right edge of the region.
    //    // Moves allowed: right, up, down (no diagonals).
    //    public static void Generate(CellT[,] cells, int width, int height)
    //    {
    //        System.Random rand = new System.Random();

    //        // Pick a random start cell on the left edge
    //        int startY = rand.Next(height);
    //        CellT current = cells[0, startY];

    //        int x = 0;
    //        int y = startY;

    //        // Continue until we reach the rightmost column
    //        while (x < width - 1)
    //        {
    //            var candidates = new List<(int dx, int dy)>();

    //            // Always try to go right if not at the last column
    //            if (x < width - 1)
    //            {
    //                if (!cells[x + 1, y].IsWalkable) // do not allow to carve into an already walkable path, this stops loops forming
    //                {
    //                    candidates.Add((1, 0));
    //                }
    //            }

    //            // can also go left
    //            if (x > 0)
    //            {
    //                if (!cells[x - 1, y].IsWalkable) // dont go left if path is already carved
    //                {

    //                    candidates.Add((-1, 0));
    //                }
    //            }

    //            // If not at the bottom row, can go down
    //            if (y > 0)
    //            {
    //                if (!cells[x, y - 1].IsWalkable) // do not go down if path below is carved
    //                {
    //                    candidates.Add((0, -1));
    //                }
    //            }

    //            // If not at the top row, can go up

    //            if (y < height - 1)
    //            {
    //                if (!cells[x, y + 1].IsWalkable) // dont go up if path above is carved
    //                {
    //                    candidates.Add((0, 1));
    //                }
    //            }

    //            UnityEngine.Debug.Log(candidates);
    //            // Choose a random move
    //            var chosen = candidates[rand.Next(candidates.Count)];
    //            int newX = x + chosen.dx;
    //            int newY = y + chosen.dy;

    //            CellT nextCell = cells[newX, newY];
    //            CarvePassage(current, nextCell);

    //            current = nextCell;
    //            x = newX;
    //            y = newY;
    //        }
    //    }

    //    private static void CarvePassage(CellT from, CellT to)
    //    {
    //        int dx = to.X - from.X;
    //        int dy = to.Y - from.Y;

    //        // Horizontal right move
    //        if (dx == 1 && dy == 0)
    //        {
    //            from.IsOpenRight = true;
    //            to.IsOpenLeft = true;
    //        }
    //        // Move up (0, -1)
    //        else if (dx == 0 && dy == -1)
    //        {
    //            from.IsOpenUp = true;
    //            to.IsOpenDown = true;
    //        }
    //        // Move down (0, 1)
    //        else if (dx == 0 && dy == 1)
    //        {
    //            from.IsOpenDown = true;
    //            to.IsOpenUp = true;
    //        }
    //    }
    //}

    public static void Generate(CellT[,] cells, int width, int height)
    {
        
        System.Random rand = new System.Random();
        bool pathGenerated = false;

        while (!pathGenerated)
        {
            ResetCells(cells, width, height);
            // Pick a random start cell on the left edge
            int startY = rand.Next(height);
            CellT current = cells[0, startY];

            int x = 0;
            int y = startY;


            pathGenerated = true;
            //UnityEngine.Debug.Log((x, ",", y, "is the start"));
            // Continue until we reach the rightmost column
            while (x < width - 1)
            {
                var candidates = new List<(int dx, int dy)>();

                // Add valid candidates
                AddCandidate(cells, candidates, x, y, 1, 0, width, height); // Right
                AddCandidate(cells, candidates, x, y, -1, 0, width, height); // Left
                AddCandidate(cells, candidates, x, y, 0, -1, width, height); // Down
                AddCandidate(cells, candidates, x, y, 0, 1, width, height); // Up

                if (candidates.Count == 0)
                {
                    UnityEngine.Debug.LogWarning("Path generation failed. Trying again");
                    pathGenerated = false;
                    break;
                    
                }
                

                    // Choose a random move
                    var chosen = candidates[rand.Next(candidates.Count)];
                    int newX = x + chosen.dx;
                    int newY = y + chosen.dy;

                    CellT nextCell = cells[newX, newY];
                    CarvePassage(current, nextCell);

                    current = nextCell;
                    x = newX;
                    y = newY;
                
            }
            
        }
    }

    private static void AddCandidate(CellT[,] cells, List<(int dx, int dy)> candidates, int x, int y, int dx, int dy, int width, int height)
    {
        int newX = x + dx;
        int newY = y + dy;

        if (newX >= 0 && newX < width && newY >= 0 && newY < height)
        {
            //UnityEngine.Debug.Log((newX, ",",  newY, "is a candidate"));
            CellT target = cells[newX, newY];
            // Check that the target cell is not already walkable
            if (!target.IsWalkable)
            {
                // Ensure the target cell has unvisited neighbors to avoid dead ends
                if (HasUnvisitedNeighbor(cells, newX, newY, width, height))
                {
                    candidates.Add((dx, dy));
                }
            }
        }
    }

    private static bool HasUnvisitedNeighbor(CellT[,] cells, int x, int y, int width, int height)
    {
        int[] dx = { 1, -1, 0, 0 };
        int[] dy = { 0, 0, 1, -1 };
        int numberOfWalkableNeighbour = 0; 

        //UnityEngine.Debug.Log((x, ",", y, "We are checking if this cell now has neighbours"));

        for (int i = 0; i < dx.Length; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
            if (nx >= 0 && nx < width && ny >= 0 && ny < height && cells[nx, ny].IsWalkable)
            {
                //return true;
                //UnityEngine.Debug.Log((nx, ",", ny, "is a neighbour "));
                numberOfWalkableNeighbour++;
            }
        }
        // if the number of neighbours is greater than 1, we dont count it as a candidate
        // the reason we do 1 is becuase, one of the neighbour will obviously be one that came from the previous cell so we dont regard that as a neighbour
        if (numberOfWalkableNeighbour > 1) 
        {
            return false;
        }
        return true;
    }

    private static void CarvePassage(CellT from, CellT to)
    {
        int dx = to.X - from.X;
        int dy = to.Y - from.Y;

        if (dx == 1 && dy == 0) // Right
        {
            from.IsOpenRight = true;
            to.IsOpenLeft = true;
        }
        else if (dx == -1 && dy == 0) // Left
        {
            from.IsOpenLeft = true;
            to.IsOpenRight = true;
        }
        else if (dx == 0 && dy == 1) // Down
        {
            from.IsOpenDown = true;
            to.IsOpenUp = true;
        }
        else if (dx == 0 && dy == -1) // Up
        {
            from.IsOpenUp = true;
            to.IsOpenDown = true;
        }
    }

    private static void ResetCells(CellT[,] cells, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                cells[x, y].IsOpenLeft = false;
                cells[x, y].IsOpenRight = false;
                cells[x, y].IsOpenUp = false;
                cells[x, y].IsOpenDown = false;
            }
        }
    }

}