using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavVoxel 
{
    public Vector3 WorldPosition;
    public bool IsBlocked;
    public bool IsWalkable;

    public NavVoxel(Vector3 worldPosition, bool isBlocked, bool isWalkable)
    {
        WorldPosition = worldPosition;
        IsBlocked = isBlocked;
        IsWalkable = isWalkable;
    }
}
