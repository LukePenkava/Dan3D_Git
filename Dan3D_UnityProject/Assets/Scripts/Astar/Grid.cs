using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Obstacles need to have tag and layer set to obstacle
//Walkable ground needs to have Tag "Ground"

public class Grid : MonoBehaviour {

	//set terrain mask, change to walkable mask

	public Vector2 gridWorldSize;
	//public LayerMask unwalkalbeMask;
	public LayerMask terrainMask; //Layer to find points on walkable ground. Was both Ground and Obstacle, but use only Ground
	
    int combinedObstacleMask = 1 << 10; //(1 << 11) | (1 << 13); 

	public LayerMask mask1; //Ground
	public LayerMask mask2;	//Obstacle
	public float nodeRadius;
	public Node[,] grid;
	public List<Node> allNodes = new List<Node>();

	float nodeDiameter;		//Size of the node
	int gridSizeX;
	int gridSizeY;
	float groundClearing = 35f;
	public float obstacleScaleValue = 1;
	float maxAngleToInclude = 30f; //If bigger angle to steep to walk

	public bool showDebugVisuals = true;
	
	// public GameObject obstaclesParent;

	List<Vector3> testPoints = new List<Vector3>();

	void Start()
	{
		ScaleObstaclesUp();
		nodeDiameter = nodeRadius*2f;
		gridSizeX = Mathf.RoundToInt( gridWorldSize.x / nodeDiameter );
		gridSizeY = Mathf.RoundToInt( gridWorldSize.y / nodeDiameter );
		CreateGrid();
		ScaleObstaclesDown();
	}

	public int MaxSize
	{
		get
		{
			return gridSizeX * gridSizeY;
		}
	}

	void ScaleObstaclesUp() {
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

		for(int i = 0; i < obstacles.Length; i++) {
			obstacles[i].transform.localScale = new Vector3(obstacles[i].transform.localScale.x * (1 + (obstacleScaleValue / 100f)), obstacles[i].transform.localScale.y * (1 + (obstacleScaleValue / 100f)), obstacles[i].transform.localScale.z * (1 + (obstacleScaleValue / 100f)));
		}		
	}

	void ScaleObstaclesDown() {
		GameObject[] obstacles = GameObject.FindGameObjectsWithTag("Obstacle");

		for(int i = 0; i < obstacles.Length; i++) {
			obstacles[i].transform.localScale = new Vector3(obstacles[i].transform.localScale.x / (100 + obstacleScaleValue) * 100f, obstacles[i].transform.localScale.y / (100 + obstacleScaleValue) * 100f, obstacles[i].transform.localScale.z / (100 + obstacleScaleValue) * 100f);
		}		
	}

	// bool CheckIsInsideCollider(Vector3 pos) {
	// 	foreach(Transform child in obstaclesParent.transform) {
	// 		if(child.GetComponent<Collider>().bounds.Contains(pos)) {	
	// 			GameObject cube = Instantiate(testCube, pos, Quaternion.identity);
	// 			cube.transform.name = child.transform.name + Random.Range(0, 10000).ToString();
	// 			return true;
	// 		}
	// 	}	

	//Grid is square, origin at center
	void CreateGrid()
	{
		grid = new Node[gridSizeX, gridSizeY];

		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2f - Vector3.forward * gridWorldSize.y/2f;

		for(int x = 0; x < gridSizeX; x++)
		{
			for(int y = 0; y < gridSizeY; y++)
			{
				Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * ( y * nodeDiameter + nodeRadius);
			
				//Set y Positions
				float posY = 0f;
				float height = 3000f;
				RaycastHit hit;
				if(Physics.Raycast(new Vector3(worldPoint.x, transform.position.y + height, worldPoint.z), Vector3.down, out hit, height*2f, terrainMask))
				{
					posY = hit.point.y + 0.1f; //Offset points above ground, so that linecast can be made between point and not collide with ground

					//Advanced Raycast to get nodes under rocks as well. Sometimes Rock block raycast to the ground, but there is enough space to go under neath it. This should address it.
					//Shoot raycast down, check if the hit is rock or terrain.				
					//If its terrain, leave the point and add it as walkable point
					//If its rock, create another raycast, but ignore the obstacle, get terrain point under the rock and check how far it is to get the rock hit

					if(hit.collider.tag == "Ground") {
						Vector3 tempPos1 = new Vector3(worldPoint.x, posY, worldPoint.z);
						bool clear1 = !(Physics.CheckSphere(tempPos1, nodeRadius, combinedObstacleMask));

                        bool angleClear = true;						
						float angle = Vector3.Angle(Vector3.up, hit.normal);
						//Valid terrain position only if the slope of the terrain is small (ie peaks and hills are exluded, find flat terrain )
						if (angle > maxAngleToInclude)
						{
							angleClear = false;
						}
                        else
                        {
                            //the ground might be at good angle in the middle of node, but there can be peak at the edge of it or somewhere in the node
                            //Take the center of the node and shoot raycast to all directions, if there is some steep ground, mark it as non walkable node
							Vector3[] directions = new Vector3[] {new Vector3(1, 0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1), new Vector3(0, 0, -1) };
							for (int direction = 0; direction < directions.Length; direction++)
                            {
								RaycastHit sideHit;
								if (Physics.Raycast(new Vector3(hit.point.x, hit.point.y + (nodeRadius/2f), hit.point.z), directions[direction], out sideHit, nodeRadius, terrainMask))
                                {
									float sideAngle = Vector3.Angle(Vector3.up, sideHit.normal);
									if (sideAngle > maxAngleToInclude)
									{
										angleClear = false;
									}
								}
							}
						}                        

						if (clear1 && angleClear) {
							SetPoint(true, worldPoint.x, posY, worldPoint.z, x, y);
						}
						else {
							SetPoint(false, worldPoint.x, posY, worldPoint.z, x, y);
						}
					}					
					else 
					{
						SetPoint(false, worldPoint.x, posY, worldPoint.z, x, y);
					}

				}
			}
		}
	}

	void SetPoint(bool walkable, float posX, float posY, float posZ, int x, int y) {

		Vector3 worldPoint = new Vector3(posX, posY, posZ);
		grid[x,y] = new Node(walkable, worldPoint, x, y);
		if(walkable) {
			allNodes.Add(grid[x,y]);
		}
	}

	//Get neighbours of the node
	public List<Node> GetNeighbours(Node node)
	{
		List<Node> neighbours = new List<Node>();

		for( int x = -1; x <= 1; x++)
		{
			for( int y = -1; y <= 1; y++)
			{
				if(x == 0 && y == 0)
				{	continue;	}

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbours.Add(grid[checkX, checkY]);
				}
			}
		}

		return neighbours;
	}

	//Convert world Pos to the node index
	public Node GetNodeFromWorldPoint(Vector3 worldPos)
	{
		//Calculate index by percent, because size is basicly -15 to 15 for example, it has to be offseted to have percent from 0.0 to 1.0
		float percentX = (worldPos.x + gridWorldSize.x/2f) / gridWorldSize.x;
		float percentY = (worldPos.z + gridWorldSize.y/2f) / gridWorldSize.y;

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid[x,y];
	}

	 bool IsPointInside(Mesh aMesh, Vector3 aLocalPoint)
     {
         var verts = aMesh.vertices;
         var tris = aMesh.triangles;
         int triangleCount = tris.Length / 3;
         for (int i = 0; i < triangleCount; i++)
         {
             var V1 = verts[ tris[ i*3     ] ];
             var V2 = verts[ tris[ i*3 + 1 ] ];
             var V3 = verts[ tris[ i*3 + 2 ] ];
             var P = new Plane(V1,V2,V3);
             if (P.GetSide(aLocalPoint))
                 return false;
         }
         return true;
     }

	public List<Node> path;
	void OnDrawGizmos()
	{
		if(showDebugVisuals)
		{

			Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

			if(grid != null && grid.Length > 0)
			{			
				foreach(Node n in grid)
				{
					if(n == null) { continue; }
					Gizmos.color = (n.walkable) ? Color.cyan : Color.red;

					if(path != null)
					{
						if(path.Contains(n))
						{
							Gizmos.color = Color.green;
						}
					}

					if(!n.walkable)
					{
						Gizmos.DrawCube(n.worldPosition + this.transform.parent.position, Vector3.one * (nodeDiameter - 0.1f));
					}
					else
					{
						Gizmos.DrawSphere(n.worldPosition + this.transform.parent.position, nodeDiameter/5f);
					}
				}

				if(path != null)
				{
					if(path.Count > 1)
					{
						Gizmos.color = Color.yellow;

						for(int i = 1; i < path.Count; i++)
						{
							Gizmos.DrawLine(path[i-1].worldPosition + this.transform.parent.position, path[i].worldPosition + this.transform.parent.position);
						}
					}
				}

				foreach(Vector3 temp in testPoints) {
					Gizmos.color = Color.yellow;
					Gizmos.DrawSphere(temp, nodeDiameter/5f);
				}
			}

		}
	}
}
