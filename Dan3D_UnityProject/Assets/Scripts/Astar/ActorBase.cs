using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Handles actual pahtfinding, selecting target and everything around pathfinding
public class ActorBase : MonoBehaviour {

	public delegate void ArrivedDelegate();
	public event ArrivedDelegate Arrived;
	
	AstarPathfinding astar;
	Grid grid;

	GameObject targetObject;
	Vector3 targetObjectOffset;
	bool useStaticTarget = false;
	public bool debugVisuals;

	List<Vector3> pathPositions = new List<Vector3>();
	int pathIndex = 0;
	bool arrived = false;
	bool neverArrive = true;
	bool navToTarget = false;	//Navigate directly to target, not through path, ie when close to the target
	protected Vector3 staticTarget;
	Vector3 lastNavPosition = Vector3.zero;
	bool hasPath = false;	

	public float recalculatePathInterval = 1f;	//How often should whole new path be requested
	public float switchNodeDist = 0.5f;		//Distance between Actore and Node, if smaller switch to next  node
	public float arrivedDistance = 0.01f;		//Check against target ( last node ), if smaller distance stop moving.

	protected Vector3 navPos;


	public void Init()
	{
		astar = GameObject.FindGameObjectWithTag("Astar").GetComponent<AstarPathfinding>();
		grid = astar.grid;
		
		// if(useRandomTarget) {
		// 	SelectRandomTarget();
		// }

		InvokeRepeating("GetPath", 1f, recalculatePathInterval);
	}

	public void PathUdate() 
	{
		//if(Director.paused){ return; }
		//if(hasPath == false) { return; }

		//There are poth position and actor did not reach end yet
		if(pathPositions.Count > 0 && pathIndex < (pathPositions.Count)) //|| navToTarget
		{ 			
			//Vector3 navPosition = Vector3.zero;	
			navPos = Vector3.zero;
			Vector3 linecastPosition = Vector3.zero;

			//Navigate either to target or nodes in path
			//last node in the pathPosition, ie target is in the node sector, switch to Target itself
			if(pathIndex == (pathPositions.Count-1) || pathPositions.Count == 0)
			{				
				navPos = (useStaticTarget) ? (staticTarget) : (targetObject.transform.position + targetObjectOffset);				
				navToTarget = true;
			}				
			else
			{ 
				//Move towards node in the path
				navPos = pathPositions[pathIndex];
				navToTarget = false; 
			}

			if(!arrived)
			{				
				Vector3 nextNavPosition = navPos;
				int newIndex = pathIndex+1;
				if(newIndex < pathPositions.Count) {
					nextNavPosition = pathPositions[newIndex] /*+ parent.position*/;
				}					
			
				//Movement(navPos, nextNavPosition);			

				SelectNode(navPos);
				CheckForTarget();
			}
            else
            {
				//Arrived
				// navPos = Vector3.zero;
				// hasPath = false;
				
				// if(Arrived != null) {
				// 	Arrived();
				// }
            }

		} else {

			// //There is no path and not navigating to target, get new path
			// if(pathPositions.Count == 0 && navToTarget == false) {
			// 	if(useRandomTarget) {
			// 		SelectRandomTarget();
			// 	}				
			// 	GetPath();
			// }
		}
	}

	public virtual void Movement(Vector3 navPosition, Vector3 nextNavPosition){}	


	void GetPath()
	{
		//Get Path is local, using nodes as they were created, ignroing shifting of the world. Ie actor is still at 0,0 as far as nodes are concerned, eventho his actual world position is thousands of units shifted. Nodes are local
		Vector3 targetPos = (useStaticTarget) ? (staticTarget) : (targetObject.transform.position + targetObjectOffset);	

		pathPositions = astar.FindPath(this.transform.localPosition, targetPos); 		
		pathIndex = 0;		
		hasPath = true;
	}

	public void SelectRandomTarget()
	{		      
		print("Select Random Target");

		useStaticTarget = true;

		//targetPos can be any position inside the Grid
		int randomIndex = Random.Range(0, grid.allNodes.Count);
		Vector3 targetPos = grid.allNodes[randomIndex].worldPosition;        	
		bool validPos = false;
		int attempts = 0;

		//Vector3 myPos = transform.localPosition;

		while (!validPos)
		{
			bool walkable = false;
			
			Node tempNode = grid.GetNodeFromWorldPoint(targetPos);
			if (tempNode != null)
			{
				walkable = tempNode.walkable;
			}
            else
            {
				print("Node is Null");
            }

			attempts++;          

            if(walkable) 
            {
				validPos = true;
            }
		}

		if (attempts > 50)
		{
			print("Target Attempts " + attempts);
		}	

		staticTarget = targetPos;
		hasPath = false;
		arrived = false;

		GetPath();
	}

	public void PathToTarget(GameObject target, Vector3 offset) {
		targetObject = target;
		targetObjectOffset = offset;
		useStaticTarget = false;

		GetPath();
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
		Vector3 targetPos = (useStaticTarget) ? (staticTarget) : (targetObject.transform.position + targetObjectOffset);
		Vector3 myPos = new Vector3(transform.position.x, 0, transform.position.z);
		targetPos = new Vector3(targetPos.x, 0, targetPos.z);
		float distanceToTarget = Vector3.Distance(myPos, targetPos);

		if (navToTarget)
		{
			//print(distanceToTarget + " " + arrivedDistance);
		}

		DebugManager.Instance.Debug_ValueWithPosition("#ZimDistance3", "DistanceToTarget", distanceToTarget, new Vector2(0f, 120f), this.transform.position);
		print("myPos " + myPos + " targetPos " + targetPos);

		if(distanceToTarget < arrivedDistance)
		{		
			arrived = true;

			if(Arrived != null) {
				Arrived();
			}

			// if(useStaticTarget)
			// {	
			// 	arrived = true;
			// 	//SelectRandomTarget();	

			// 	if(Arrived != null) {
			// 		Arrived();
			// 	}
			// } 
			// else {
			// 	//Using moving target, dont flag arrived as true
			// }
		}
	}


	void OnDrawGizmos()
	{		
		if(debugVisuals)
		{
			Gizmos.color = Color.red;

			if(useStaticTarget) {
				if(staticTarget == null) {
					return;
				}
			}
			else {
				if(targetObject == null) {
					return;
				}
			}

			Vector3 targetPos = (useStaticTarget) ? staticTarget : (targetObject.transform.position + targetObjectOffset);
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
