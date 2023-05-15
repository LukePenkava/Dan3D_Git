using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles actual pahtfinding, selecting target and everything around pathfinding
public class ActorBase : MonoBehaviour {
	
	AstarPathfinding astar;
	Grid grid;

	public GameObject targetObject;
	public bool useRandomTarget = false;
	public bool debugVisuals;

	List<Vector3> pathPositions = new List<Vector3>();
	int pathIndex = 0;
	bool arrived = false;
	bool neverArrive = true;
	bool navToTarget = false;	//Navigate directly to target, not through path, ie when close to the target
	protected Vector3 randomTarget;
	Vector3 lastNavPosition = Vector3.zero;
	bool hasPath = false;	

	public float recalculatePathInterval = 1f;	//How often should whole new path be requested
	public float switchNodeDist = 0.5f;		//Distance between Actore and Node, if smaller switch to next  node
	public float arrivedDistance = 0.01f;		//Check against target ( last node ), if smaller distance stop moving.



	public void Init()
	{
		astar = GameObject.FindGameObjectWithTag("Astar").GetComponent<AstarPathfinding>();
		//SelectRandomTarget();
		InvokeRepeating("GetPath", 1f, recalculatePathInterval);
	}

	public void PathUdate() 
	{
		//if(Director.paused){ return; }

		if(pathPositions.Count > 0 && pathIndex < (pathPositions.Count)) //|| navToTarget
		{ 			
			Vector3 navPosition = Vector3.zero;	
			Vector3 linecastPosition = Vector3.zero;

			//Navigate either to target or nodes in path
			//last node in the pathPosition, ie target is in the node sector, switch to Target itself
			if(pathIndex == (pathPositions.Count-1) || pathPositions.Count == 0)
			{				
				navPosition = (useRandomTarget) ? (randomTarget) : targetObject.transform.position;				
				navToTarget = true;
			}				
			else
			{ 
				//Move towards node in the path
				navPosition = pathPositions[pathIndex];
				navToTarget = false; 
			}

			if(!arrived)
			{				
				Vector3 nextNavPosition = navPosition;
				int newIndex = pathIndex+1;
				if(newIndex < pathPositions.Count) {
					nextNavPosition = pathPositions[newIndex] /*+ parent.position*/;
				}					
			
				Movement(navPosition, nextNavPosition);			

				SelectNode(navPosition);
				CheckForTarget();
			}
            else
            {
				//Arrived
				//SelectRandomTarget();
            }

		} else {

			if(pathPositions.Count == 0 && navToTarget == false) {
				//SelectRandomTarget();
				GetPath();
			}
		}
	}

	public virtual void Movement(Vector3 navPosition, Vector3 nextNavPosition){}	


	void GetPath()
	{
		//Get Path is local, using nodes as they were created, ignroing shifting of the world. Ie actor is still at 0,0 as far as nodes are concerned, eventho his actual world position is thousands of units shifted. Nodes are local
		Vector3 targetPos = (useRandomTarget) ? (randomTarget) : targetObject.transform.localPosition;	

		pathPositions = astar.FindPath(this.transform.localPosition, targetPos); 		
		pathIndex = 0;		
		hasPath = true;
	}

	void SelectRandomTarget()
	{
		// /*//Get Position from Grid Nodes
		// int randomIndex = Random.Range(0, grid.allNodes.Count);
		// Vector3 testTarget = grid.allNodes[randomIndex].worldPosition;

		// randomTarget = testTarget;
		// hasPath = false;
		// arrived = false;

		// GetPath();
		// return;*/

		// //Using local position, becase local position is same as when nodes were craeted. Actual position is shifted with the world shifting. Target needs to be selected based on local positions
		// Vector3 myPos = transform.localPosition; //transform.position;
		// Vector3 myDir = transform.forward.normalized;
		// float dotToPass = 0.25f;

		// //Get Position from Grid Nodes
		// //int randomIndex = Random.Range(0, grid.allNodes.Count);
		// //Vector3 testTarget = grid.allNodes[randomIndex].worldPosition;

        // //Get Random position in the zone, check if the node in the zone is walkable, if not, get new one
        // Vector3 targetPos = Vector3.zero;	
		// bool validPos = false;
		// int attempts = 0;

		// while (!validPos)
		// {
		// 	bool walkable = false;
		// 	bool correctDirection = false;

		// 	targetPos = zoneManager.GetZonePos(1);
		// 	targetPos = zoneManager.masterParent.InverseTransformPoint(targetPos);
		// 	Node tempNode = grid.GetNodeFromWorldPoint(targetPos);
		// 	if (tempNode != null)
		// 	{
		// 		walkable = tempNode.walkable;
		// 	}
        //     else
        //     {
		// 		print("Node is Null");
        //     }

		// 	attempts++;

        //     if(walkable)
        //     {
        //         //Right in front of the actor would be 1, behind is -1, 0 on the side
		// 		Vector3 testDir = targetPos - myPos;
		// 		if(Vector3.Dot(myDir, testDir.normalized) < dotToPass)
        //         {
		// 			//If it fails, reduce the criteria angle and test it again, until the angle is anything                    
		// 			dotToPass = dotToPass - 0.05f;
		// 		}
        //         else
        //         {
        //             //passed
		// 			correctDirection = true;
        //         }
		// 	}

        //     if(walkable && correctDirection)
        //     {
		// 		validPos = true;
        //     }
		// }

		// if (attempts > 50)
		// {
		// 	print("Target Attempts " + attempts);
		// }

		// /*Vector3 testDir = targetPos - myPos;

	    // //Get new target which is most inline with current direction of the actor. If it fails, reduce the criteria angle and test it again, until the angle is anything
		// while(Vector3.Dot(myDir, testDir.normalized) < dotToPass) {		
			
		// 	targetPos = zoneManager.GetZonePos(1); 
		// 	testDir = targetPos - myPos;
		// 	dotToPass = dotToPass - 0.01f;
		// }*/

		// randomTarget = targetPos;
		// hasPath = false;
		// arrived = false;

		// GetPath();
		// //linecastHitBox.transform.position = testTarget;
	}


	void SelectNode(Vector3 pos)
	{
		//Distance either to node or target itself
		float distanceToNavPosition = Vector3.Distance(transform.position, pos);

		if(pathIndex < (pathPositions.Count-1))
		{
			//Switch to next node when close enough to current one
			if(distanceToNavPosition < switchNodeDist)
			{
				pathIndex++;
			}
		}
	}

	void CheckForTarget()
	{
		Vector3 targetPos = (useRandomTarget) ? (randomTarget /*+ parent.position*/) : targetObject.transform.position;
		Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
		targetPos = new Vector3(targetPos.x, 0, targetPos.z);
		float distanceToTarget = Vector3.Distance(myPos, targetPos);

		if (navToTarget)
		{
			//print(distanceToTarget + " " + arrivedDistance);
		}

		if(distanceToTarget < arrivedDistance)
		{		
			if(useRandomTarget)
			{	
				arrived = true;
				SelectRandomTarget();	
			} 
			else {
				//Using moving target, dont flag arrived as true
			}
		}
	}


	void OnDrawGizmos()
	{		
		if(debugVisuals)
		{
			Gizmos.color = Color.red;

			Vector3 targetPos = (useRandomTarget) ? randomTarget : targetObject.transform.position;
			Gizmos.DrawSphere(targetPos, 0.3f);

			if(pathPositions.Count > 0)
			{
				for(int i = 0; i < pathPositions.Count; i++)
				{
					Gizmos.color = Color.green;
					Gizmos.DrawSphere(pathPositions[i] /*+ this.transform.parent.position*/, 0.3f);

					if(i > 0)
					{
						Gizmos.color = Color.yellow;
						Gizmos.DrawLine(pathPositions[i-1] /*+ this.transform.parent.position*/, pathPositions[i] /*+ this.transform.parent.position*/);
					}
				}
			}
		}
	}
}
