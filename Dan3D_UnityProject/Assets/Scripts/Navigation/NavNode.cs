using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavNode
{
    public NavVoxel Voxel;
    public NavNode Parent;
    public int GCost; // Cost from start to this node
    public int HCost; // Heuristic cost from this node to end
    public int FCost { get { return GCost + HCost; } } // Total cost

    public NavNode(NavVoxel voxel)
    {
        Voxel = voxel;
    }
}
