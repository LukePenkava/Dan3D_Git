using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State : MonoBehaviour
{
    bool grounded = false;
    public bool Grounded {
        get { return grounded; }
        set { grounded = value;}
    }

    public enum States {
        Null,

        Idle,
        Walking,
        Running,
        Attacking,
        Jumping,
        Falling,
        Interacting,
        Sitting,
        Sleeping,
        Cursed,
        Navigating
    };  

    public enum Directions {
        Left,
        Right,
        None
    };

    States activeState = States.Idle;   
    public States ActiveState {
        get { return activeState; }
        set { activeState = value; }
    }

    Directions direction = Directions.Right;
    public Directions Direction {
        get { return direction; }
        set { direction = value; }
    }

    // public void FacePosition(Vector3 pos) {

    //     if(pos.x > this.transform.position.x) {
    //         direction = Directions.Right;         
    //     } else {
    //         direction = Directions.Left;            
    //     }
    // }
}