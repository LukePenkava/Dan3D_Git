using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NavVoxelGrid
{
    public NavVoxel[,,] Grid;
    float VoxelSize;
    LayerMask ObstacleLayer;
    Vector3 GridOffset;

    public NavVoxelGrid(int width, int height, int depth, float voxelSize, LayerMask obstacleLayer, Vector3 offset)
    {
        VoxelSize = voxelSize;
        ObstacleLayer = obstacleLayer;
        GridOffset = offset;
        Grid = new NavVoxel[width, height, depth];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < depth; z++)
                {
                    Vector3 worldPos = new Vector3(x, y, z) * VoxelSize + GridOffset;
                    bool isBlocked = Physics.CheckBox(worldPos, new Vector3(VoxelSize / 2, VoxelSize / 2, VoxelSize / 2), Quaternion.identity, ObstacleLayer);
                    bool isWalkable = !isBlocked;
                    Grid[x, y, z] = new NavVoxel(worldPos, isBlocked, isWalkable);
                }
            }
        }
    }

    public NavVoxel GetVoxelFromWorldPosition(Vector3 worldPos)
    {
        // Adjust position by subtracting the offset before converting to grid coordinates
        Vector3 adjustedPos = worldPos - GridOffset;

        int x = Mathf.FloorToInt(adjustedPos.x / VoxelSize);
        int y = Mathf.FloorToInt(adjustedPos.y / VoxelSize);
        int z = Mathf.FloorToInt(adjustedPos.z / VoxelSize);

        if (x >= 0 && y >= 0 && z >= 0 && x < Grid.GetLength(0) && y < Grid.GetLength(1) && z < Grid.GetLength(2))
        {
            return Grid[x, y, z];
        }
        else
        {
            Debug.Log("Requested position out of bounds");
        }
        return null; // Return null if the position is out of the grid bounds
    }

    public List<NavVoxel> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        // Convert world positions to corresponding voxels
        NavVoxel startVoxel = GetVoxelFromWorldPosition(startPos);
        NavVoxel targetVoxel = GetVoxelFromWorldPosition(targetPos);

        // Check if start or end voxels are valid and not blocked
        if (startVoxel == null || targetVoxel == null)
        {
            Debug.Log("Start or Target path pos is null");
            return null; // Return null if pathfinding is not possible
        }

        if (startVoxel.IsBlocked)
        {
            Debug.Log("Start Pos is Blocked, looking for neighbors");
            startVoxel = FindNearestWalkableNode(startPos);
        }

        if (targetVoxel.IsBlocked)
        {
            Debug.Log("End Pos is Blocked, looking for neighbors");
            targetVoxel = FindNearestWalkableNode(targetPos);
        }

        // Open set stores nodes to be evaluated, closed set stores nodes already evaluated
        List<NavNode> openSet = new List<NavNode>();
        HashSet<NavNode> closedSet = new HashSet<NavNode>();

        // Create a start node and add it to the open set
        NavNode startNode = new NavNode(startVoxel);
        openSet.Add(startNode);

        // Loop until there are no more nodes to evaluate
        while (openSet.Count > 0)
        {
            // Start with the first node in the open set
            NavNode currentNode = openSet[0];

            // Find the node with the lowest F cost in the open set
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].FCost < currentNode.FCost || (openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost))
                {
                    currentNode = openSet[i];
                }
            }

            // Move the current node from the open set to the closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Check if the current node is the target node (path has been found)
            if (currentNode.Voxel == targetVoxel)
            {
                // Retrace the path from the end node to the start node
                return RetracePath(startNode, currentNode);
            }

            // Iterate through each neighbor of the current node
            foreach (NavVoxel neighbor in GetNeighbors(currentNode.Voxel))
            {
                // Skip this neighbor if it is blocked or already in the closed set
                if (neighbor.IsBlocked || closedSet.Any(n => n.Voxel == neighbor))
                {
                    continue;
                }

                // Calculate the new movement cost to the neighbor
                int newCostToNeighbor = currentNode.GCost + GetDistance(currentNode.Voxel, neighbor);
                NavNode neighborNode = openSet.FirstOrDefault(n => n.Voxel == neighbor);

                // If the neighbor is not in the open set, add it
                if (neighborNode == null)
                {
                    neighborNode = new NavNode(neighbor);
                    neighborNode.HCost = GetDistance(neighbor, targetVoxel);
                    neighborNode.GCost = newCostToNeighbor;
                    neighborNode.Parent = currentNode;
                    openSet.Add(neighborNode);
                }
                else if (newCostToNeighbor < neighborNode.GCost)
                {
                    // If the new path to the neighbor is shorter, update the node's cost and parent
                    neighborNode.GCost = newCostToNeighbor;
                    neighborNode.Parent = currentNode;
                }
            }
        }

        Debug.Log("No Path Found");
        return null; // No path found
    }

    // Method to retrace the path from the end node to the start node
    private List<NavVoxel> RetracePath(NavNode startNode, NavNode endNode)
    {
        List<NavVoxel> path = new List<NavVoxel>();
        NavNode currentNode = endNode;

        // Work back from the end node to the start node
        while (currentNode != startNode)
        {
            path.Add(currentNode.Voxel);
            currentNode = currentNode.Parent;
        }

        // Reverse the path to get the correct order from start to end
        path.Reverse();
        return path;
    }

    private int GetDistance(NavVoxel a, NavVoxel b)
    {
        Vector3 adjusted_a = a.WorldPosition - GridOffset;
        Vector3 adjusted_b = b.WorldPosition - GridOffset;

        int distX = Mathf.Abs(Mathf.FloorToInt(adjusted_a.x / VoxelSize) - Mathf.FloorToInt(adjusted_b.x / VoxelSize));
        int distY = Mathf.Abs(Mathf.FloorToInt(adjusted_a.y / VoxelSize) - Mathf.FloorToInt(adjusted_b.y / VoxelSize));
        int distZ = Mathf.Abs(Mathf.FloorToInt(adjusted_a.z / VoxelSize) - Mathf.FloorToInt(adjusted_b.z / VoxelSize));

        // Manhattan distance for a 3D grid
        return distX + distY + distZ;
    }

    private IEnumerable<NavVoxel> GetNeighbors(NavVoxel voxel)
    {
        // List to store all valid neighbors
        List<NavVoxel> neighbors = new List<NavVoxel>();

        Vector3 adjustedWorldPos = voxel.WorldPosition - GridOffset;

        // Convert the voxel's world position to grid coordinates
        int voxelX = Mathf.FloorToInt(adjustedWorldPos.x / VoxelSize);
        int voxelY = Mathf.FloorToInt(adjustedWorldPos.y / VoxelSize);
        int voxelZ = Mathf.FloorToInt(adjustedWorldPos.z / VoxelSize);

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    // Skip the center voxel (the voxel itself is not its neighbor)
                    if (x == 0 && y == 0 && z == 0)
                        continue;

                    // Calculate the position of the neighbor
                    int checkX = voxelX + x;
                    int checkY = voxelY + y;
                    int checkZ = voxelZ + z;

                    // Check if the neighbor coordinates are within the grid bounds
                    if (checkX >= 0 && checkX < Grid.GetLength(0) &&
                        checkY >= 0 && checkY < Grid.GetLength(1) &&
                        checkZ >= 0 && checkZ < Grid.GetLength(2))
                    {
                        // Add the neighbor to the list
                        neighbors.Add(Grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbors;
    }

    public NavVoxel FindNearestWalkableNode(Vector3 currentPos)
    {
        // Convert the current position to grid coordinates
        NavVoxel currentVoxel = GetVoxelFromWorldPosition(currentPos);

        // If the current voxel is walkable, return it
        if (currentVoxel != null && currentVoxel.IsWalkable)
        {
            return currentVoxel;
        }

        // Search for the nearest walkable voxel
        const int searchRadius = 10; // You can adjust this value
        for (int r = 1; r <= searchRadius; r++)
        {
            foreach (NavVoxel neighbor in GetNeighborsInRange(currentVoxel, r))
            {
                if (neighbor.IsWalkable)
                {
                    return neighbor;
                }
            }
        }

        // No walkable node found within the search radius
        return null;
    }

    private IEnumerable<NavVoxel> GetNeighborsInRange(NavVoxel voxel, int range)
    {
        List<NavVoxel> neighbors = new List<NavVoxel>();

        // Convert the voxel's world position to grid coordinates
        Vector3 adjustedWorldPos = voxel.WorldPosition - GridOffset;
        int voxelX = Mathf.FloorToInt(adjustedWorldPos.x / VoxelSize);
        int voxelY = Mathf.FloorToInt(adjustedWorldPos.y / VoxelSize);
        int voxelZ = Mathf.FloorToInt(adjustedWorldPos.z / VoxelSize);

        for (int x = -range; x <= range; x++)
        {
            for (int y = -range; y <= range; y++)
            {
                for (int z = -range; z <= range; z++)
                {
                    int checkX = voxelX + x;
                    int checkY = voxelY + y;
                    int checkZ = voxelZ + z;

                    if (checkX >= 0 && checkX < Grid.GetLength(0) &&
                        checkY >= 0 && checkY < Grid.GetLength(1) &&
                        checkZ >= 0 && checkZ < Grid.GetLength(2))
                    {
                        neighbors.Add(Grid[checkX, checkY, checkZ]);
                    }
                }
            }
        }

        return neighbors;
    }
}
