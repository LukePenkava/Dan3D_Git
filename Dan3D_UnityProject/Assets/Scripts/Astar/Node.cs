using UnityEngine;
using System.Collections;

public class Node : IHeapItem<Node>
{
	public bool walkable = false;	//if node is inside obstalce or not
	public Vector3 worldPosition = Vector3.zero;
	public int gridX;	//grid index X, so its easy to find Node
	public int gridY;

	public int gCost;	//cost of movement from node to node, total g cost from start node
	public int hCost;	//cost of movement from this node to the target
	public Node parent;

	int heapIndex;

	//Constructor
	public Node(bool Walkable, Vector3 WorldPosition, int GridX, int GridY)
	{
		walkable = Walkable;
		worldPosition = WorldPosition;
		gridX = GridX;
		gridY = GridY;
	}

	//Calculate fCost
	public int fCost
	{
		get{ return gCost + hCost; }
	}

	public int HeapIndex
	{
		get
		{
			return heapIndex;
		}
		set
		{
			heapIndex = value;
		}
	}

	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if(compare == 0)
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}

		return -compare;
	}
}
