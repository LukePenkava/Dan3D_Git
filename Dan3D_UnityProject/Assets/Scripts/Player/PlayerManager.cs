using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    CharacterController controller;
    UIManager uiManager;
    InteractionManager interactionManager;
    Character_BaseData baseData;
    State playerState;
    Animator anim;          

    public GameObject visual;    

    //Movement
    Vector2 moveVector;
    public Vector2 MoveVector {
        get { return moveVector; }
        set { moveVector = value; }
    }
    bool sprint = false;
    public bool Sprint {
        get { return sprint; }
        set { sprint = value; }
    }
    float targetRotation = 0.0f;       
    public float moveSpeed = 1.0f;
    public float sprintSpeed = 2.0f;

    //Animations
    AnimatorStateInfo animStateInfo;
    int animID_MoveSpeed;
    int animID_MoveEnabled;
    int animID_Sprint;
    int animID_Interaction;
    int animID_Attack;

    //Gravity and Velocity
    float gravity = -15.0f;
    float verticalVelocity;
    float maxVelocity = 53.0f;

    //Interactions
    //Used in PlayerInput to block input, this blocks some input, mostly walking etc, but player can still play minigame etc.
    bool inputEnabled = true;
    public bool InputEnabled {
        get { return inputEnabled; }
        set { inputEnabled = value; }
    }

    //Completely disable all players input. For example Dan navigates on his own, custscene etc
    bool allInputEnabled = true;
    public bool AllInputEnabled {
        get { return allInputEnabled; }
        set { allInputEnabled = value; }
    }

    bool isInteracting = false;
    public bool IsInteracting {
        get { return isInteracting; }
        set { isInteracting = value; }
    }
    Interaction activeInteraction;


    //Attack
    public GameObject weapon;
    public GameObject weaponCollider;

    void Start()
    {                  
        controller = GetComponent<CharacterController>();
        anim = visual.GetComponent<Animator>();
        baseData = GetComponent<Character_BaseData>();
        playerState = GetComponent<State>();

        uiManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>();    
        interactionManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<InteractionManager>();             

        //Animation IDs
        animID_MoveSpeed = Animator.StringToHash("MoveSpeed");
        animID_MoveEnabled = Animator.StringToHash("MoveEnabled");
        animID_Sprint = Animator.StringToHash("Sprint");
        animID_Interaction = Animator.StringToHash("Interaction");
        animID_Attack = Animator.StringToHash("Attack");

        anim.SetBool(animID_MoveEnabled, true);

        weapon.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        animStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        AnimationsInfo();

        Move();
    }

    //Any logic related to what current animation is playing, if animation finished etc
    void AnimationsInfo() {

        if(animStateInfo.IsName("Attack")) {
            if(animStateInfo.normalizedTime >= 1) {
                AttackFinished();
            }
        }
    } 

    //Move and Rotate character
    void Move() {

        Vector3 targetDirection = Vector3.zero;
        float targetSpeed = 0f;

        //When player is interacting or attacking, disable movement
        if(playerState.ActiveState == State.States.Idle || playerState.ActiveState == State.States.Walking) {            

            // set target speed based on move speed, sprint speed and if sprint is pressed
            targetSpeed = sprint ? sprintSpeed : moveSpeed;
        
            if (moveVector == Vector2.zero) {
                targetSpeed = 0.0f;
            }

            if (moveVector != Vector2.zero) {
                playerState.ActiveState = State.States.Walking;

                //Calculate angle from inputs (atan2 is arctangent, gets angle between adjance and opposite, covert to angel by * rad)
                targetRotation = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg;
                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);           
            } 
            else {
                playerState.ActiveState = State.States.Idle;
            }

            targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

            //Gravity
            if (verticalVelocity < maxVelocity) {
                verticalVelocity += gravity * Time.deltaTime;
            }

            anim.SetFloat(animID_MoveSpeed, targetSpeed);
            anim.SetBool(animID_Sprint, sprint);
        } 

        controller.Move(targetDirection.normalized * (targetSpeed * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);            
    }

    //Interaction triggered by user input
    public void TriggerInteraction() {

        if(playerState.ActiveState == State.States.Interacting) { return; }

        playerState.ActiveState = State.States.Interacting;
        anim.SetBool(animID_MoveEnabled, false);
        anim.SetTrigger(animID_Interaction);

        StartCoroutine(ResumeFromInteraction());
    }

    IEnumerator ResumeFromInteraction() {
        yield return new WaitForSeconds(1.4f);

        playerState.ActiveState = State.States.Idle;
        anim.SetBool(animID_MoveEnabled, true);
    }

    //Called from PlayerInput, get Active Interaction from InteractionManager, if there is any 
    public void Interact(int inputSelection) { 
        
        if(isInteracting) { 
            return;
        }        

        //Copy of Interaction Object, dont modify, use only to read, refer back to InteractionManager to modify the Interaction in any way
        activeInteraction = interactionManager.GetActiveInteraction(inputSelection);
        print(activeInteraction.interactionText);

        if(activeInteraction && activeInteraction.InteractionState == InteractionStates.Active) {
      
            isInteracting = true;               

            //If Interaction is of type Dialogue, Interaction does not have set State and Life, because it depends on type of used Dialogue, set it here based on type of the Dialogue
            // if(activeInteraction.interactionType == InteractionManager.InteractionTypes.Dialogue) {
            //     DialogueObject obj = activeInteraction.GetComponent<InteractionDialogue>().GetCurrentDialogue();
            //     activeInteraction.Data.interactionPlayerState = (obj.type == Dialogue.Types.Free) ? InteractionManager.InteractionPlayerStates.Free : InteractionManager.InteractionPlayerStates.Locked;            
            //     GameDirector.gameState = (obj.type == Dialogue.Types.Free) ? GameDirector.GameState.World : GameDirector.GameState.LockedDialogue;             
            // }

            // if(activeInteraction.interactionType == InteractionManager.InteractionTypes.MG) {                      
            //     GameDirector.gameState = GameDirector.GameState.MG; 
            // }
            
            //Lock Input
            //inputEnabled = (activeInteraction.Data.interactionPlayerState == InteractionManager.InteractionPlayerStates.Free) ? true : false;           

            //Set Animation State if there is any
            //if(activeInteraction.Data.animationState != State.States.Null) { state.ActiveState = activeInteraction.Data.animationState; }

            //Face the Interacdtion
            //state.FacePosition(activeInteraction.gameObject.transform.position);            

            //Handle specific Types of Interactinos

            //Wait for given time, then activate the interaction
            if(activeInteraction.Data.interactionPlayerState == InteractionManager.InteractionPlayerStates.LockedForTime) {
                StartCoroutine(TimedInteraction(activeInteraction.Data.time));
            }

            //! NOT BEING USED
            //Immediately Trigger Interaction, player can activate next interaction, like next line of dialogue immediately with next button press
            // if(activeInteraction.Data.interactionLife == InteractionManager.InteractionLife.Single || activeInteraction.Data.interactionLife == InteractionManager.InteractionLife.Consumable) {
            //     interactionManager.ActivateInteraction(activeInteraction.Data.selectionMenuStaysOn);
            //     isInteracting = false;
            // }

            // if(activeInteraction.Data.interactionLife == InteractionManager.InteractionLife.Active) {
            //     interactionManager.ActivateInteraction(activeInteraction.Data.selectionMenuStaysOn);
            //     isInteracting = true;
            // }

            //! NOT BEING USED
            // if(activeInteraction.Data.interactionLife == InteractionManager.InteractionLife.ActiveForTime) {
            //     interactionManager.ActivateInteraction(activeInteraction.Data.selectionMenuStaysOn);
            //     isInteracting = true;
            //     StartCoroutine(ActiveForTime(activeInteraction.Data.time));
            // }

        }
        else {
            isInteracting = false;
        }
    }

     //For Example Collecting Resource, Player is locked for some time, then interaction is activated
    IEnumerator TimedInteraction(float timer) {

        yield return new WaitForSeconds(timer);
        inputEnabled = true;        
        isInteracting = false;
        interactionManager.ActivateInteraction(activeInteraction.Data.selectionMenuStaysOn);
        playerState.ActiveState = State.States.Idle;     
    }

    //Attack triggered by user input
    public void Attack() {
        if(playerState.ActiveState == State.States.Interacting) { return; }

        playerState.ActiveState = State.States.Attacking;
        anim.SetBool(animID_MoveEnabled, false);
        anim.SetTrigger(animID_Attack);
        StartCoroutine(ActivateCollider());
        weapon.SetActive(true);
    }

    IEnumerator ActivateCollider() {
        weaponCollider.SetActive(false);
        yield return new WaitForSeconds(0.36f);
        weaponCollider.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        weaponCollider.SetActive(false);
    }

    //Called when attack animation finishes
    void AttackFinished() {
        playerState.ActiveState = State.States.Idle;
        anim.SetBool(animID_MoveEnabled, true);
        weapon.SetActive(false);
    }
}
