using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : NpcBase
{  
    float intervalTimer = 0f;
    Vector3 navPosition = Vector3.zero;
    bool isAttacking = false;

    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        base.Init();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        base.BaseUpdate();

        if(isAttacking) {
            navPosition = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
            SetMoveVector(navPosition);
        } 
        else 
        {
            //Set timer and countdown time to set new nav position
            intervalTimer -= Time.deltaTime;
            if(intervalTimer <= 0f) {
                //Get new nav position
                intervalTimer = Random.Range(4f, 6f);

                isAttacking = Random.Range(0, 100) > 90 ? true : false;

                if(isAttacking) {
                    SetAttacking();
                } else {
                    navPosition = GetNavPosition();
                    SetMoveVector(navPosition);
                }
            }
        }

        //If enemy is close to the nav pos, stop moving by setting move vector to zero
        float distance = Vector3.Distance(new Vector3(this.transform.position.x, 0f, this.transform.position.z), navPosition);
        if(distance < 0.3f) {
            moveVector = Vector3.zero;
        }
    }

    Vector3 GetNavPosition() {
        return new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
    }

    public void GotHit() {  

        if(isAttacking) {     
            isAttacking = false;
            visual.GetComponent<Renderer>().material.color = Color.green;
            intervalTimer = 10f;

            sprint = false;
            navPosition = (new Vector3(player.transform.position.x, 0f, player.transform.position.z) - new Vector3(this.transform.position.x, 0f, this.transform.position.z)).normalized * -3f; 
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
        visual.GetComponent<Renderer>().material.color = Color.red;
        //Vector3 navPos = new Vector3(player.transform.position.x, 0f, player.transform.position.z);
        sprint = true;

        //return navPos;
    }
}
