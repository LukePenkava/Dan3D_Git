using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavManager : MonoBehaviour
{
    public int GridWidth = 10;
    public int GridHeight = 10;
    public int GridDepth = 10;
    public Vector3 offset;
    public float VoxelSize = 1f;

    public LayerMask BlockingLayer;

    NavVoxelGrid voxelGrid;

    public bool debugGrid;
    public float debugSize = 0.02f;
    public int debugStep = 10;


    void Awake()
    {
        voxelGrid = new NavVoxelGrid(GridWidth, GridHeight, GridDepth, VoxelSize, BlockingLayer, offset);
    }

    public List<NavVoxel> GetPath(Vector3 pathStart, Vector3 pathTarget)
    {
        List<NavVoxel> path = voxelGrid.FindPath(pathStart, pathTarget);
        return path;
    }

    void OnDrawGizmos()
    {
        if (voxelGrid == null) return;
        if (debugGrid == false) return;



        for (int x = 0; x < voxelGrid.Grid.GetLength(0); x++)
        {
            for (int y = 0; y < voxelGrid.Grid.GetLength(1); y++)
            {
                for (int z = 0; z < voxelGrid.Grid.GetLength(2); z++)
                {
                    if (x % debugStep == 0 && y % debugStep == 0 && z % debugStep == 0)
                    {
                        NavVoxel voxel = voxelGrid.Grid[x, y, z];
                        Gizmos.color = voxel.IsWalkable ? new Color(0, 1, 0) : new Color(1, 0, 0);
                        Gizmos.DrawWireCube(voxel.WorldPosition, new Vector3(debugSize, debugSize, debugSize));
                        //Gizmos.DrawSphere(voxel.WorldPosition, 0.05f);
                        // Gizmos.DrawWireSphere(voxel.WorldPosition, 0.5f);
                    }
                }
            }
        }
    }

}
