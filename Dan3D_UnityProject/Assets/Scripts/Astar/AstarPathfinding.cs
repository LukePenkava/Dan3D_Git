using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Needs to have tag "Astar" so that actors can find it.
//Get actual path for actor here. Grid creates nodes that are used for navigation, this script calculates the navigation and path

public class AstarPathfinding : MonoBehaviour {
	
	public Grid grid;

	public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
	{
		//Get Nodes in the grid
		Node startNode = grid.GetNodeFromWorldPoint(startPos);
		Node targetNode = grid.GetNodeFromWorldPoint(targetPos);

		//OpenSet are nodes, which are being checked
		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);		
		//Closed set are nodes which have already been checked and used to determine path
		HashSet<Node> closedset = new HashSet<Node>();

		openSet.Add(startNode);

		while(openSet.Count > 0)
		{
			Node currentNode = openSet.RemoveFirst();			

			closedset.Add(currentNode);

			//We are at the end, get paht
			if(currentNode == targetNode)
			{				
				return RetracePath(startNode, targetNode);
			}

			//Go through all neighbours for current Node
			foreach(Node neighbour in grid.GetNeighbours(currentNode))
			{
				if(neighbour == null) {
					print("null neighbour");
					continue;
				}

				//If node is obstacle or is already in closedSet, then skip it
				if(!neighbour.walkable || closedset.Contains(neighbour))
				{ continue; }

				//Gcost is getting added to existing G cost, it means, if its being looked at for a first time, gCost is 0
				//but later, it can be already some value and its getting increased, so that we know total gCost from startNode
				//GetDistance should in the case of neighbour always give 10 or 14, not more
				int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

				//If the g Cost is smaller to move from current node to neighbour than the actual neighbour gCost, recalculate neighbour
				//If neighbour isnt in openset, add him there so he can be checked
				if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
				{
					//Calculating fCost ( g+h )
					neighbour.gCost = newMovementCostToNeighbour;
					neighbour.hCost = GetDistance(neighbour, targetNode);
					neighbour.parent = currentNode;

					if(!openSet.Contains(neighbour))
					{
						openSet.Add(neighbour);
					}
					else
					{
						openSet.UpdateItem(neighbour);
					}
				}
			}
		}

		//Probably means target node could not be reached, return empty list
		print("Could not be reached?");
		List<Vector3> emptyList = new List<Vector3>();
		return emptyList;
	}


	//Find all nodes from end, use parented nodes and then reverse it
	List<Vector3> RetracePath(Node startNode, Node endNode)
	{
		List<Vector3> pathVectors = new List<Vector3>();
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while(currentNode != startNode)
		{
			path.Add(currentNode);
			pathVectors.Add(currentNode.worldPosition);
			currentNode = currentNode.parent;
		}

		path.Reverse();
		pathVectors.Reverse();

		grid.path = path;

		return pathVectors;
	}

	//Calculate h and g cost, its using diagonals, 14 is diagonal cost, 10 straight
	int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		//If the node is not diagonal, its always 14 * 0, its only 14 * 1 if its diagonal

		if(distX > distY)
		{
			return 14*distY + 10*(distX-distY);
		}
		else
		{
			return 14*distX + 10*(distY-distX);
		}
	}
}
