using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcBase : MonoBehaviour
{
    public enum NpcStates {
        Idle,
        Walking,
        Interacting,
        Attacking
    }
    public static NpcStates NpcState = NpcStates.Idle;

    CharacterController controller;      

    protected Vector3 moveVector = Vector3.zero; //Used by child class to navigate npc
    float targetRotation = 0.0f;
    protected bool sprint = false;
    public float walkSpeed_Movement = 1.0f;
    public float runSpeed_Movement = 2.0f;

    //Gravity and Velocity
    float gravity = -15.0f;
    float verticalVelocity;
    float maxVelocity = 53.0f;

    protected void Init()
    {      
        controller = GetComponent<CharacterController>();           
    }

    protected void BaseUpdate()
    {
        //animStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //AnimationsInfo();   

        if(Director.isLoading) { return; }

        Move();
    }

    //void AnimationsInfo() {

        // if(animStateInfo.IsName("Attack")) {
        //     if(animStateInfo.normalizedTime >= 1) {
        //         AttackFinished();
        //     }
        // }
    //}  

    //Move and Rotate character
    void Move() {

        Vector3 targetDirection = Vector3.zero;
        float targetSpeed = 0f;

        //When player is interacting or attacking, disable movement
        if(NpcState == NpcStates.Idle || NpcState == NpcStates.Walking) {            

            // set target speed based on move speed, sprint speed and if sprint is pressed
            targetSpeed = sprint ? runSpeed_Movement : walkSpeed_Movement;        
            
            if (moveVector == Vector3.zero) {
                targetSpeed = 0.0f;
            }

            if (moveVector != Vector3.zero) {
                NpcState = NpcStates.Walking;

                //Calculate angle from inputs (atan2 is arctangent, gets angle between adjance and opposite, covert to angel by * rad)
                targetRotation = Mathf.Atan2(moveVector.x, moveVector.z) * Mathf.Rad2Deg;
                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);           
            } 
            else {
                NpcState = NpcStates.Idle;
            }

            targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            //Gravity
            if (verticalVelocity < maxVelocity) {
                verticalVelocity += gravity * Time.deltaTime;
            }
        } 

        controller.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);            
    }
}
