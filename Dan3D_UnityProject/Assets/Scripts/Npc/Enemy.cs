using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NpcBase
{  
    public GameObject visual;
    
    float intervalTimer = 0f;
    Vector3 navPosition = Vector3.zero;
    bool isAttacking = false;
    bool isRunningAway = false;

    GameObject player;

    Animator anim;
    int animID_MoveSpeed;
    int animID_Walk;

    float walkSpeed_Animator = 1.0f;
    float runSpeed_Animator = 2.0f;


    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        player = GameObject.FindGameObjectWithTag("Player");

        anim = visual.GetComponent<Animator>();
        animID_MoveSpeed = Animator.StringToHash("MoveSpeed");
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();

        float targetSpeed = 0f;       

        if(isRunningAway) {
            targetSpeed = runSpeed_Animator;
        }
        else 
        {
            if(isAttacking) {
                navPosition = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
                SetMoveVector(navPosition);
                targetSpeed = walkSpeed_Animator;
                sprint = false;
            } 
            else 
            {
                moveVector = Vector3.zero;

                //Set timer and countdown time to set new nav position
                intervalTimer -= Time.deltaTime;
                if(intervalTimer <= 0f) {
                    //Get new nav position
                    intervalTimer = Random.Range(3f, 4f);
                    isAttacking = Random.Range(0, 100) > 50 ? true : false;

                    // if(isAttacking) {
                    //     SetAttacking();
                    // } else {
                    //     //navPosition = GetNavPosition();
                    //     //SetMoveVector(navPosition);                    
                    // }
                }
            }
        }

        //If enemy is close to the nav pos, stop moving by setting move vector to zero
        float distance = Vector3.Distance(new Vector3(this.transform.position.x, 0f, this.transform.position.z), navPosition);
        if(distance < 0.3f) {
            moveVector = Vector3.zero;
            targetSpeed = 0f;   
        }

        anim.SetFloat(animID_MoveSpeed, targetSpeed);
    }

    Vector3 GetNavPosition() {
        return new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
    }

    public void GotHit() {  

        if(isAttacking) {                 
            //visual.GetComponent<Renderer>().material.color = Color.green;
            //intervalTimer = 10f;

            isRunningAway = true;
            isAttacking = false;
            sprint = true;
            //navPosition = (new Vector3(player.transform.position.x, 0f, player.transform.position.z) - new Vector3(this.transform.position.x, 0f, this.transform.position.z)).normalized * -6f; 
            navPosition = new Vector3(4.5f, 0f, 9f);
            SetMoveVector(navPosition);

            
        }
        else {
            isAttacking = true;
            SetAttacking();
            //SetMoveVector(navPosition);
        }
    }

    void SetMoveVector(Vector3 navPos) {
        moveVector = (navPos - new Vector3(this.transform.position.x, 0f, this.transform.position.z)).normalized;
    }

    void SetAttacking() {
        //visual.GetComponent<Renderer>().material.color = Color.red;
        //Vector3 navPos = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
        //sprint = true;

        //return navPos;
    }
}
