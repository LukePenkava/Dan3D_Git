using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zima_Nav : ActorBase
{
    public Vector3 NavPos { get { return navPos; } }

    void Start () 
	{
		base.Init();
	}	
	
	void Update () 
	{
        //If should move
		base.PathUdate();
	}

    //Call for new nav position ( random position around Dangoru )

    // public override void Movement (Vector3 navPosition, Vector3 nextNavPosition)
	// {
	// 	transform.position = Vector3.MoveTowards(transform.position, navPosition, Time.deltaTime * 3f);

	// 	//Temp Rotation
	// 	Vector3 pos = new Vector3(navPosition.x, 0, navPosition.z) - new Vector3(transform.position.x, 0, transform.position.z);

	// 	if(pos != Vector3.zero)
	// 	{			
	// 		transform.rotation = Quaternion.LookRotation(pos);
	// 	}
	// }
}
