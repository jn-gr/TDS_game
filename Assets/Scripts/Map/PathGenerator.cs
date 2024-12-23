using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class PathGenerator
{
    public static void Generate(Region region, int width, int height)
    {


        // what needs to be done
        // it has to select the startcells, and because it is a list, we have ti implement it so that it starts a path from all of them and garantees an exit
        // whats also very important, is we need to add a check for the end cell, we cannot have the end cell be on the same side as an unlocked region
        // meaning, end side has to be on a unlocked region

        // because of the splitting thing we have going on, we have to add some sort of count or something, i think its best to limit the split to 2 per region
        // matching end cells and startcells is the key to all this, and ensuring end cells , acctually point to unlocked regions.
        // that should be it i think

        System.Random rand = new System.Random();
        bool pathGenerated = false;

        int initialNoOfSplits = 0;

        // Determine starting side
        int startSide = rand.Next(4); // 0 = Left, 1 = Right, 2 = Bottom, 3 = Top
        UnityEngine.Debug.Log(("This is startside", startSide));

        CellT startCell = GetStartCell(region.Cells, startSide, width, height, rand);
        region.startCells.Add(startCell);

        while (!pathGenerated)
        {
            ResetRegion(region);
            int noOfSplits = initialNoOfSplits;

            // Initialize active paths
            List<(CellT headCell, int x, int y, int endSide)> activePaths = new List<(CellT, int, int, int)>
            {
            (startCell, startCell.X, startCell.Y, GetRandomEndSide(startSide))
            };

            List<(CellT headCell, int endSide)> completedPaths = new List<(CellT, int)>();
            pathGenerated = true;

            while (activePaths.Count > 0)
            {
                var nextPaths = new List<(CellT headCell, int x, int y, int endSide)>();

                foreach (var (headCell, x, y, endSide) in activePaths)
                {
                    var candidates = new List<(int dx, int dy)>();

                    // Add valid candidates
                    AddCandidate(region.Cells, candidates, x, y, 1, 0, width, height); // Right
                    AddCandidate(region.Cells, candidates, x, y, -1, 0, width, height); // Left
                    AddCandidate(region.Cells, candidates, x, y, 0, -1, width, height); // Down
                    AddCandidate(region.Cells, candidates, x, y, 0, 1, width, height); // Up

                    if (candidates.Count == 0)
                    {
                        if (IsEndReached(headCell, endSide, width, height))
                        {
                            
                            completedPaths.Add((headCell, endSide));
                            
                        }
                        else
                        {
                            pathGenerated = false;
                        }
                       continue;
                    }

                    // Pick a random move
                    var chosen = candidates[rand.Next(candidates.Count)];
                    int newX = x + chosen.dx;
                    int newY = y + chosen.dy;

                    CellT nextCell = region.Cells[newX, newY];
                    CarvePassage(headCell, nextCell);

                    nextPaths.Add((nextCell, newX, newY, endSide));

                    // Random chance to split
                    if (noOfSplits > 0 && rand.NextDouble() < 0.3) // 30% chance to split
                    {
                        var splitCandidates = candidates.Where(c => c != chosen).ToList();
                        if (splitCandidates.Count > 0)
                        {
                            var splitChosen = splitCandidates[rand.Next(splitCandidates.Count)];
                            int splitX = x + splitChosen.dx;
                            int splitY = y + splitChosen.dy;

                            CellT splitCell = region.Cells[splitX, splitY];
                            CarvePassage(headCell, splitCell);

                            int splitEndSide = GetEndSideForSplit(splitChosen.dx, splitChosen.dy, startSide);

                            nextPaths.Add((splitCell, splitX, splitY, splitEndSide));
                        }
                        noOfSplits--;
                    }
                }

                activePaths = nextPaths; // Update active paths for the next iteration
                
            }
            //UnityEngine.Debug.Log($"Complete paths count: {completedPaths.Count}");
            foreach (var (headCell, endSide) in completedPaths)
            {
                if (!IsEndReached(headCell, endSide, width, height))
                {
                    UnityEngine.Debug.LogError($"Path failed to reach end side: {endSide}");
                    pathGenerated = false;
                    break; // Retry immediately if any path is invalid
                }

                if (!region.endCells.Contains(headCell))
                {
                    region.endCells.Add(headCell);
                }
            }
        }
        
    }


    private static int GetRandomEndSide(int excludeSide)
    {
        System.Random rand = new System.Random();
        int endSide;
        do
        {
            endSide = rand.Next(4);
        } while (endSide == excludeSide); // Ensure end side is different from the starting side
        return endSide;
    }
   
    private static int GetEndSideForSplit(int dx, int dy,int startSide)
    {
        if (dx < 0) return 0; // Left
        if (dx > 0) return 1; // Right   
        if (dy < 0) return 2; // Down
        if (dy > 0) return 3; // Up

        return GetRandomEndSide(startSide);
        
    }
    private static CellT GetStartCell(CellT[,] cells, int startSide, int width, int height, System.Random rand)
    {
        switch (startSide)
        {
            case 0: // Left
                return cells[0, rand.Next(height)];
            case 1: // Right
                return cells[width - 1, rand.Next(height)];
            case 2: // Bottom
                return cells[rand.Next(width), 0];
            case 3: // Top
                return cells[rand.Next(width), height-1];
            default:
                throw new ArgumentException("Invalid start side.");
        }
    }

    private static bool IsEndReached(CellT cell, int endSide, int width, int height)
    {
        switch (endSide)
        {
            case 0: // Left
                return cell.X == 0;
            case 1: // Right
                return cell.X == width - 1;
            case 2: // Bottom
                return cell.Y == 0;
            case 3: // Top
                return cell.Y == height - 1;
            default:
                throw new ArgumentException("Invalid end side.");
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
                // Ensure the target cell has unvisited neighbors to avoid dead ends and loops
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
        else if (dx == 0 && dy == 1) // Up
        {
            from.IsOpenDown = true;
            to.IsOpenUp = true;
        }
        else if (dx == 0 && dy == -1) // Down
        {
            from.IsOpenUp = true;
            to.IsOpenDown = true;
        }
    }

    private static void ResetRegion(Region region)
    {
        for (int x = 0; x < region.Width; x++)
        {
            for (int y = 0; y < region.Height; y++)
            {
                region.Cells[x, y].IsOpenLeft = false;
                region.Cells[x, y].IsOpenRight = false;
                region.Cells[x, y].IsOpenUp = false;
                region.Cells[x, y].IsOpenDown = false;
            }
        }
    }

}

//// if the path doesnt reach the end, we try again, it will come up with a path eventually
//while (!pathGenerated)
//{
//    ResetCells(region.Cells, width, height);

//    //int startY = rand.Next(height);
//    int startY = region.startCell.Y;
//    CellT current = region.Cells[0, startY];

//    int x = 0;
//    int y = startY;


//    pathGenerated = true;
//    //UnityEngine.Debug.Log((x, ",", y, "is the start"));
//    // continue until we reach the rightmost column
//    while (x < width - 1)
//    {
//        var candidates = new List<(int dx, int dy)>();

//        // Add valid candidates
//        AddCandidate(region.Cells, candidates, x, y, 1, 0, width, height); // Right
//        AddCandidate(region.Cells, candidates, x, y, -1, 0, width, height); // Left
//        AddCandidate(region.Cells, candidates, x, y, 0, -1, width, height); // Down
//        AddCandidate(region.Cells, candidates, x, y, 0, 1, width, height); // Up

//        if (candidates.Count == 0)
//        {
//            UnityEngine.Debug.LogWarning("Path generation failed. Trying again");
//            pathGenerated = false;
//            break;

//        }


//            // pikc a random move
//            var chosen = candidates[rand.Next(candidates.Count)];
//            int newX = x + chosen.dx;
//            int newY = y + chosen.dy;

//            CellT nextCell = region.Cells[newX, newY];
//            CarvePassage(current, nextCell);

//            current = nextCell;
//            x = newX;
//            y = newY;

//        if (x == width - 1) 
//        { 
//            region.endCell = current;
//        }

//    }

//}