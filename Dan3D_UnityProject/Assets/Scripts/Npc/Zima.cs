using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zima : NpcBase
{   
    InteractionManager interactionManager;
    Zima_Nav nav;

    float intervalTimer = 0f;
    Vector3 navPosition = Vector3.zero;
    GameObject player;
    Area areaScript;

    //Animations
    AnimatorStateInfo animStateInfo;
    int animID_MoveSpeed;
    int animID_Walk;
    int animID_Sprint; 
    int animID_ReadyToDig;
    int animID_Dig;

    Animator anim;

    float walkSpeed_Animator = 1.0f;
    float runSpeed_Animator = 2.0f;
    float maxDistanceToPlayer = 5.0f;
    bool chasingPlayer = false;
    float navIntervalMin = 10f;
    float navIntervalMax = 15f;
    float navIntervalChasing = 0.25f;

    //Digging
    public GameObject digInteraction;
    bool isDigsite = false;
    bool isReadyToDig = false;
    bool isDigging = false;

    //Arrived Event from Navigation
    bool navigating = false;

    void Start()
    {
        base.Init();
        player = GameObject.FindGameObjectWithTag("Player");
        nav = GetComponent<Zima_Nav>();
        areaScript = GameObject.FindGameObjectWithTag("Area").GetComponent<Area>();
        interactionManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<InteractionManager>();

        nav.Arrived += PathingArrived;

        anim = GetComponent<Animator>();
        digInteraction.SetActive(false);

        //Animation IDs
        animID_MoveSpeed = Animator.StringToHash("MoveSpeed");
        animID_ReadyToDig = Animator.StringToHash("ReadyToDig");
        animID_Dig = Animator.StringToHash("Dig");
    }

    // void OnEnable() {
    //     nav.Arrived += PathingArrived;
    // }

    void PathingArrived() {
        print("Arrived");
        navigating = false;
    }

    void Update()
    {
        base.BaseUpdate();

        float targetSpeed = 0f;   
        sprint = false;     

        //Zim priorities Digging, Be close to player, wander around

        intervalTimer -= Time.deltaTime;
        if(intervalTimer <= 0f && isDigging == false) {
            //Get new nav position
            intervalTimer = chasingPlayer ? navIntervalChasing : Random.Range(navIntervalMin, navIntervalMax);  
            Pathing_RandomNavPos();
        }                     
            
        if(navigating) {
            SetMoveVector(nav.NavPos);
            sprint = true;
            targetSpeed = runSpeed_Animator;
        } else {
            moveVector = Vector3.zero;
        }         

        anim.SetFloat(animID_MoveSpeed, targetSpeed);

        /* Digging
        -mark digsite digged
        */
       
        // //Set timer and countdown time to set new nav position
        // intervalTimer -= Time.deltaTime;
        // if(intervalTimer <= 0f && isDigging == false) {
        //     //Get new nav position
        //     intervalTimer = chasingPlayer ? navIntervalChasing : Random.Range(navIntervalMin, navIntervalMax);  

        //     //If not chasing player and setting new navPos, roll if its digsites, if there are any available
        //     if(chasingPlayer == false) {

        //         // if(areaScript.AvailableDigSites() > 0) {
        //         //     //Roll if this navPos is digsite
        //         //     float roll = Random.Range(0f, 100f);  
        //         //     if(roll > 60f) {
        //         //         isDigsite = true;
        //         //     } else {
        //         //         isDigsite = false;
        //         //     }
        //         // }

        //         // //If current navPos is digsite or currently digging, dont look for new navPos unless it is chasing player
        //         // if(isReadyToDig == false && isDigging == false) {
        //         //     //navPosition = 
        //         //     Pathing_RandomNavPos();
        //         // }

        //     } else {
        //         //navPosition = GetNavPositionToDan();
        //         //Pathing_NavToDan();
        //     }
            
        //     //SetMoveVector(navPosition);            
        //}   

        //If enemy is close to the nav pos, stop moving by setting move vector to zero
        // float distanceToNav = Vector3.Distance(new Vector3(this.transform.position.x, 0f, this.transform.position.z), navPosition);
        // float distanceToPlayer = Vector3.Distance(new Vector3(this.transform.position.x, 0f, this.transform.position.z), player.transform.position);
        
        //Player is too far from Zim, run towards him
        // if(distanceToPlayer > maxDistanceToPlayer && isDigging == false) {
        //     if(chasingPlayer == false) {          
        //         intervalTimer = 0f;
        //         chasingPlayer = true;
        //         print("Call Pathing 1");
        //         Pathing_NavToDan();
        //         navigating = true;
        //     }
        // }

        // if(distanceToNav <= 0.3f) {            
        //     moveVector = Vector3.zero;
        //     navigating = false;
        //     //Arrived at navpos that is digsite
        //     if(isDigsite) {
        //         isReadyToDig = true;
        //         anim.SetBool(animID_ReadyToDig, true);
        //         digInteraction.SetActive(true);
        //         interactionManager.FindInteractions();
        //     }

        //     //Stop chasing player when reach navpos next to him
        //     if(chasingPlayer) {
        //         //Wait for a bit, but not too long after arriving next to the player
        //         anim.SetBool(animID_ReadyToDig, false);
        //         intervalTimer = Random.Range(1.0f, 2.0f); 
        //         chasingPlayer = false;
        //         isDigsite = false;
        //         isReadyToDig = false;
        //         digInteraction.SetActive(false);
        //         interactionManager.FindInteractions();
        //     }
        // } else {             

        //     if(chasingPlayer) {
        //         //if chasing player always run
        //         sprint = true;
        //         targetSpeed = runSpeed_Animator;
        //     }
        //     else 
        //     {
        //         if(distanceToNav > 3.0f) {
        //             //Make sure that run distance is not half unit or something like that
        //             if(sprint == false) {
        //                 if((distanceToNav - 3.0f) > 1.5f) {
        //                     sprint = true;
        //                     targetSpeed = runSpeed_Animator;
        //                 } else {
        //                     targetSpeed = walkSpeed_Animator;
        //                 }
        //             }
        //             else {
        //                 targetSpeed = runSpeed_Animator;
        //             }
                    
        //         } else {                
        //             sprint = false;
        //             targetSpeed = walkSpeed_Animator;
        //         }
        //     }
        // }

        // if(navigating) {
        //     SetMoveVector(nav.NavPos);
        // } else {
        //     SetMoveVector(Vector3.zero);
        // }

        // DebugManager.Instance.Debug_ValueWithPosition("#ZimDistance1", "DistanceToNavPos", distanceToNav, new Vector2(0f, 120f), this.transform.position);
        // DebugManager.Instance.Debug_ValueWithPosition("#ZimDistance2", "DistanceToPlayer", distanceToPlayer, new Vector2(0f, 150f), this.transform.position);

        //anim.SetFloat(animID_MoveSpeed, targetSpeed);
    }

    // public void Dig() {        
       
    //     anim.SetBool(animID_ReadyToDig, false);
    //     anim.SetBool(animID_Dig, true);

    //     isDigging = true;
    //     chasingPlayer = false;
    //     isDigsite = false;
    //     isReadyToDig = false;
    //     digInteraction.SetActive(false);
    //     interactionManager.FindInteractions();

    //     StartCoroutine(DigCompleted());
    // }

    // IEnumerator DigCompleted() {
    //     yield return new WaitForSeconds(1.5f);       

    //     moveVector = Vector3.zero;
    //     navigating = false;

    //     anim.SetBool(animID_Dig, false);
    //     isDigging = false;
    //     isDigsite = false;
    //     isReadyToDig = false;
    //     chasingPlayer = false;
    //     intervalTimer = 0f;

    //     List<Items.ItemName> itemList = new List<Items.ItemName>();
    //     itemList.Add(Items.ItemName.Wood);                   
    //     player.GetComponent<Character_BaseData>().AddItems(itemList, true);

    //     areaScript.DigsiteCompleted();
    // }

    // void Pathing_NavToDan() {
    //     print("Path To Dan");
    //     nav.PathToTarget(player, new Vector3(0,0,-1f));
    //     navigating = true;
    // }

    void Pathing_RandomNavPos() {
        
        print("Get Random pos");
        nav.SelectRandomTarget();
        navigating = true;
    }

    // Vector3 GetNavPosition() {
    //     if(chasingPlayer) {
    //         //when chasing player, set navpos next to him ( closer to camera )
    //         return (player.transform.position + new Vector3(0,0,-1f));
    //     } else {
    //         float navRadiusX = 5f;   
    //         float navRadiusZ = 3f;   
    //         //return new Vector3(player.transform.position.x + Random.Range(-navRadiusX, navRadiusX), 0f, player.transform.position.z + Random.Range(-navRadiusZ, navRadiusZ));   
    //     }
    // }

    void SetMoveVector(Vector3 navPos) {
        moveVector = (navPos - new Vector3(this.transform.position.x, 0f, this.transform.position.z)).normalized;
    }
}
