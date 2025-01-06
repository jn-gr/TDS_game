using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

public static class PathGenerator
{
    // Quick rundown of how this works
    // - StartCells, are assigned in each region. This is where the path starts from
    // - EndCells, are chosen directions for the path to end in, such as right, left ..
    // A region can only be unlocked if a neighbour regions path leads to it.
    // The player is not able to click on a region that already exists, or if that region has 4 neighbours around it
    // This is because we plan to put the enemy spawner there.

    // I have also added the ability for paths to split, this adds nice variety and option to implement shortest path algo for enemies. == more marks i hope
    // When 3 or more paths lead to a region. rather than trying to generate a path. I have done it so , the paths all merge into one cell. then that singular path paths off to a random direction
    
    public static void Generate(Region region, Dictionary<(int, int), Region> regionMap, Dictionary<(int, int), CellT> globalMap, int width, int height)
    {
        System.Random rand = new System.Random();
        bool pathGenerated = false;

        // can experiment with this, i wouldnt go above 3 as limited number of possiblities is reduced expontetially, i think 1 is best.
        int initialNoOfSplits = 1;
        
        List<int> validEndSides = GetValidEndSides(region, regionMap);

        if (validEndSides.Count == 0)
        {
            return;
        }

        while (!pathGenerated)
        {
            if (region.startCells.Count > 2) {
                // when more than 3 paths lead into region we merge instead.
                PathMerge(region,regionMap, width, height, validEndSides[rand.Next(validEndSides.Count)]);
                break;
            }
            else 
            {
                int noOfSplits;
                if (region.startCells.Count == 2)
                {
                    noOfSplits = 0;
                }
                else
                {
                    noOfSplits = initialNoOfSplits;
                }

                pathGenerated = true;
                ResetRegion(region);
                List<(CellT headCell, int endSide)> completedPaths = new List<(CellT, int)>();

                foreach (CellT startCell in region.startCells)
                {
                    List<(CellT headCell, int x, int y, int endSide)> activePaths = new List<(CellT, int, int, int)>
                    {
                    (startCell, startCell.X, startCell.Y, validEndSides[rand.Next(validEndSides.Count)])
                    };

                    while (activePaths.Count > 0)
                    {
                        var nextPaths = new List<(CellT headCell, int x, int y, int endSide)>();

                        foreach (var (headCell, x, y, endSide) in activePaths)
                        {
                            var candidates = new List<(int dx, int dy)>();
                            // these are possible directions
                            AddCandidate(region,globalMap, candidates, x, y, 1, 0, width, height); // Right
                            AddCandidate(region, globalMap, candidates, x, y, -1, 0, width, height); // Left
                            AddCandidate(region, globalMap, candidates, x, y, 0, -1, width, height); // Down
                            AddCandidate(region, globalMap, candidates, x, y, 0, 1, width, height); // Up

                            if (IsEndReached(headCell, endSide, width, height))
                            {
                                // this path successfully reached its end
                                completedPaths.Add((headCell, endSide));
                                continue;
                            }

                            if (candidates.Count == 0)
                            {
                                // No candidates left and the end was not reached
                                pathGenerated = false;
                                break;
                            }

                            // out of the possible directions, we choose one randomly
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

                                    int splitEndSide = validEndSides[rand.Next(validEndSides.Count)];

                                    nextPaths.Add((splitCell, splitX, splitY, splitEndSide));
                                }
                                noOfSplits--;
                            }
                        }

                        activePaths = nextPaths; // Update active paths for the next iteration

                    }
                    if (!pathGenerated)
                    {
                        break;
                    }

                    // checks if all completed paths 
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
        }    
    }
     public static void GenerateCenterRegion(Region region, Dictionary<(int, int), Region> regionMap, Dictionary<(int, int), CellT> globalMap, int width, int height)
     {
        (int x, int y) = (region.Width/2, region.Height / 2);
        
        CellT centerCell = region.GetCellLocal(x, y);
        PathFromMergePointToSide(centerCell, 0, region, width, height);
        PathFromMergePointToSide(centerCell, 1, region, width, height);
        PathFromMergePointToSide(centerCell, 2, region, width, height);
        PathFromMergePointToSide(centerCell, 3, region, width, height);
     }

    // Rather than generating paths randomly, we merge all paths into one, then direct one path to an end side
    private static void PathMerge(Region region, Dictionary<(int, int), Region> regionMap, int width, int height, int endSide)
    {
        System.Random rand = new System.Random();
        int mergePointX = width / 2;
        int mergePointY = height / 2;
        CellT mergeCell = region.Cells[mergePointX, mergePointY];
        bool pathgenerated = false;

        while (!pathgenerated)
        {
            pathgenerated = true;
            ResetRegion(region);
            foreach (CellT startCell in region.startCells)
            {
                int currentX = startCell.X;
                int currentY = startCell.Y;

                while (currentX != mergePointX || currentY != mergePointY)
                {
                    int newX = currentX;
                    int newY = currentY;

                    // Move towards the merge point without diagonal movement
                    if (currentX != mergePointX)
                    {
                        if (currentX < mergePointX) newX++; // Move right
                        else newX--; // Move left
                    }
                    else if (currentY != mergePointY)
                    {
                        if (currentY < mergePointY) newY++; // Move up
                        else newY--; // Move down
                    }

                    // Carve a passage between the current cell and the next cell
                    CellT nextCell = region.Cells[newX, newY];
                    CarvePassage(region.Cells[currentX, currentY], nextCell);

                    // Update current position
                    currentX = newX;
                    currentY = newY;
                }
            }
            pathgenerated = PathFromMergePointToSide(mergeCell, endSide, region, width, height);
        }

    }

    private static bool PathFromMergePointToSide(CellT mergeCell, int endSide, Region region, int width, int height)
    {
        System.Random rand = new System.Random();
        int currentX = mergeCell.X;
        int currentY = mergeCell.Y;

        while (true)
        {
            // Check if the path has reached the end side
            if (IsEndReached(region.Cells[currentX, currentY], endSide, width, height))
            {
                if (!region.endCells.Contains(region.Cells[currentX, currentY]))
                {
                    region.endCells.Add(region.Cells[currentX, currentY]);
                }
                return true;
            }

            // Move deterministically toward the end side
            if (endSide == 0 && currentX > 0) // Left
            {
                currentX--;
            }
            else if (endSide == 1 && currentX < width - 1) // Right
            {
                currentX++;
            }
            else if (endSide == 2 && currentY > 0) // Bottom
            {
                currentY--;
            }
            else if (endSide == 3 && currentY < height - 1) // Top
            {
                currentY++;
            }
            else
            {
                // Path can't move further; this should not happen with deterministic movement
                return false;
            }
            // Carve the path
            CellT nextCell = region.Cells[currentX, currentY];
            CarvePassage(region.Cells[mergeCell.X, mergeCell.Y], nextCell);

            // Update the merge cell to track progress
            mergeCell = nextCell;
        }
    }

    private static List<int> GetValidEndSides(Region region, Dictionary<(int, int), Region> globalRegionMap)
    {
        List<int> validEndSides = new List<int>();

        // Check each side for neighboring regions
        if (!globalRegionMap.ContainsKey((region.RegionX - 1, region.RegionY))) // Left neighbor
        {
            validEndSides.Add(0); // 0 = Left
        }
        if (!globalRegionMap.ContainsKey((region.RegionX + 1, region.RegionY))) // Right neighbor
        {
            validEndSides.Add(1); // 1 = Right
        }
        if (!globalRegionMap.ContainsKey((region.RegionX, region.RegionY - 1))) // Bottom neighbor
        {
            validEndSides.Add(2); // 2 = Bottom
        }
        if (!globalRegionMap.ContainsKey((region.RegionX, region.RegionY + 1))) // Top neighbor
        {
            validEndSides.Add(3); // 3 = Top
        }

        return validEndSides;
    }

    /// <summary>
    /// Check if any path reaches the end, for visual clarity, we also dont allow paths to finish on corners of the region
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="endSide"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    private static bool IsEndReached(CellT cell, int endSide, int width, int height)
    {

        bool isCorner =
        (cell.X == 0 && cell.Y == 0) || // Bottom-left corner
        (cell.X == 0 && cell.Y == height - 1) || // Top-left corner
        (cell.X == width - 1 && cell.Y == 0) || // Bottom-right corner
        (cell.X == width - 1 && cell.Y == height - 1); // Top-right corner

        if (isCorner)
        {
            return false; // Corners are not valid end positions
        }
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

    private static void AddCandidate(Region region, Dictionary<(int, int), CellT> globalMap, List<(int dx, int dy)> candidates, int x, int y, int dx, int dy, int width, int height)
    {
        int newX = x + dx;
        int newY = y + dy;

        if (newX >= 0 && newX < width && newY >= 0 && newY < height)
        {
            //UnityEngine.Debug.Log((newX, ",",  newY, "is a candidate"));
            CellT target = region.Cells[newX, newY];
            // Check that the target cell is not already walkable
            if (!target.IsWalkable)
            {
                // Ensure the target cell has unvisited neighbors to avoid dead ends and loops
                if (HasUnvisitedNeighbor(region,globalMap, newX, newY, width, height))
                {
                    candidates.Add((dx, dy));
                }
            }
        }
    }
    private static bool HasUnvisitedNeighbor(Region region, Dictionary<(int, int), CellT> globalMap, int x, int y, int width, int height)
    {
        int[] dx = { 1, -1, 0, 0, 1, 1, -1, -1 };
        int[] dy = { 0, 0, 1, -1, 1, -1, 1, -1 };
        int numberOfWalkableNeighbour = 0;

        // Get global coordinates for the current cell
        (int globalX, int globalY) = region.LocalToGlobal(x, y);

        for (int i = 0; i < dx.Length; i++)
        {
            int nx = x + dx[i];
            int ny = y + dy[i];
            int globalnx = globalX + dx[i];
            int globalny = globalY + dy[i];


            //Ensure the local coordinates are within bounds
            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
            {
                if (region.Cells[nx, ny].IsWalkable) numberOfWalkableNeighbour++;

                // Check if the neighbor exists in the global map and is walkable
            }
            else
            {
                if (globalMap.TryGetValue((globalnx, globalny), out CellT neighborCell) && neighborCell.IsWalkable)
                {
                    numberOfWalkableNeighbour++;
                }
            }
        }
        // If the number of neighbors is greater than 2, exclude as a candidate
        return numberOfWalkableNeighbour <= 2;
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
            from.IsOpenUp = true;
            to.IsOpenDown = true;
        }
        else if (dx == 0 && dy == -1) // Down
        {
            from.IsOpenDown = true;
            to.IsOpenUp = true;
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
        region.endCells.Clear();
    }
}

