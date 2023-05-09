using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// TO DO
//Inventory UI, Items

//Area logic (what area it is, load surrounding areas)
//Map (ui, display position)
//Areas loading additively. Setup Home area, left, right area, forward and backward area.
//Setup area map, first index sets y direction ( 0 default where home is, 1, going forward, -1 gobing backwards)
//So 0,1 is first area on the right of home. 0,-4, is fourth area on the left of home. 2,3, is two lanes forward from hom and 3 to the right
//Have UI map, display position

//Setup home, enter home, 3d temp art, logic. Probabaly have some map for home, do room upgrades there? how to build new rooms and upgrades

//AI for characters to move between areas and in the area.

// LATER
//Add animated enemy
//Enemies should not be attacking, but more messing with Dan, running around, scaring Chompy. When chompy is scared he runs away
//Implement Chompy
//Chomp logic, character, can run away
//Zim running around collecting things. when he runs to something like a flower, you can press a button to collect as well, even faster

// NOTES
//Scare meter for Chompy? It's more like you are protecting Chompy, Zim talks about how Chomp realy does not like this. Chompy can sense
//their dark energy as he is more magical creature as well, so it affects him a bit more.
//Dan health? reduced only from survival elements or enemies also?
//Enemies should be obnoxious, running arond Dan, biting him etc or do something else? what enemies do? they don't attack


namespace Player 
{    
    public class PlayerController : MonoBehaviour
    {
        CharacterController controller;
        Animator anim;
        CustomInput input = null;

        public GameObject visual;    

        //Input
        string inputDevice = "";
        Vector2 moveVector = Vector2.zero;
        float targetRotation = 0.0f;
        bool sprint = false;
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

        //Attack
        public GameObject weapon;
        public GameObject weaponCollider;



        ///////////////////////////


        void Awake() {
            input = new CustomInput();   
        }

        void Start()
        {                  
            controller = GetComponent<CharacterController>();
            anim = visual.GetComponent<Animator>();

            //Animation IDs
            animID_MoveSpeed = Animator.StringToHash("MoveSpeed");
            animID_MoveEnabled = Animator.StringToHash("MoveEnabled");
            animID_Sprint = Animator.StringToHash("Sprint");
            animID_Interaction = Animator.StringToHash("Interaction");
            animID_Attack = Animator.StringToHash("Attack");

            anim.SetBool(animID_MoveEnabled, true);

            weapon.SetActive(false);
        }

        void OnEnable() {
            input.Player.Enable();
            input.Player.Movement.performed += OnMovementPerformed;
            input.Player.Movement.canceled += OnMovementCancelled;
            input.Player.Sprint.performed += OnSprintPerformed;
            input.Player.Interact.performed += OnInteractPerformed;
            input.Player.Attack.performed += OnAttackPerformed;
        }    

        void OnDisable() {
            input.Player.Disable();
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.canceled -= OnMovementCancelled;
            input.Player.Interact.performed -= OnSprintPerformed;
            input.Player.Interact.performed -= OnInteractPerformed;
            input.Player.Attack.performed -= OnAttackPerformed;
        }

        // Update is called once per frame
        void Update()
        {
            animStateInfo = anim.GetCurrentAnimatorStateInfo(0);
            AnimationsInfo();
           

            //Debug.Log(moveVector);      
            Move();
        }

        private void LateUpdate()
        {
            //Camera
        }


        ///////////////////////////


        //Hand Input Events
        void OnMovementPerformed(InputAction.CallbackContext context) {        
            inputDevice = context.control.device.displayName;      
            moveVector = context.ReadValue<Vector2>();
        }

        void OnMovementCancelled(InputAction.CallbackContext context) {
            moveVector = Vector2.zero;
        }

        void OnSprintPerformed(InputAction.CallbackContext context) {            
            sprint = context.ReadValue<float>() == 1f ? true : false;       
        }    

        void OnInteractPerformed(InputAction.CallbackContext context) {
            if(context.ReadValue<float>() == 1f) {
                TriggerInteraction();
            }       
        }  

        void OnAttackPerformed(InputAction.CallbackContext context) {
            if(context.ReadValue<float>() == 1f) {                
                Attack();
            } 
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
            if(PlayerDirector.PlayerState == PlayerDirector.PlayerStates.Idle || PlayerDirector.PlayerState == PlayerDirector.PlayerStates.Walking) {            

                // set target speed based on move speed, sprint speed and if sprint is pressed
                targetSpeed = sprint ? sprintSpeed : moveSpeed;
            
                if (moveVector == Vector2.zero) {
                    targetSpeed = 0.0f;
                }

                if (moveVector != Vector2.zero) {
                    PlayerDirector.PlayerState = PlayerDirector.PlayerStates.Walking;

                    //Calculate angle from inputs (atan2 is arctangent, gets angle between adjance and opposite, covert to angel by * rad)
                    targetRotation = Mathf.Atan2(moveVector.x, moveVector.y) * Mathf.Rad2Deg;
                    // rotate to face input direction relative to camera position
                    transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);           
                } 
                else {
                    PlayerDirector.PlayerState = PlayerDirector.PlayerStates.Idle;
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
        void TriggerInteraction() {

            if(PlayerDirector.PlayerState == PlayerDirector.PlayerStates.Interacting) { return; }

            PlayerDirector.PlayerState = PlayerDirector.PlayerStates.Interacting;
            anim.SetBool(animID_MoveEnabled, false);
            anim.SetTrigger(animID_Interaction);

            StartCoroutine(ResumeFromInteraction());
        }

        IEnumerator ResumeFromInteraction() {
            yield return new WaitForSeconds(1.4f);

            PlayerDirector.PlayerState = PlayerDirector.PlayerStates.Idle;
            anim.SetBool(animID_MoveEnabled, true);
        }

        //Attack triggered by user input
        void Attack() {
            if(PlayerDirector.PlayerState == PlayerDirector.PlayerStates.Interacting) { return; }

            PlayerDirector.PlayerState = PlayerDirector.PlayerStates.Attacking;
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
            PlayerDirector.PlayerState = PlayerDirector.PlayerStates.Idle;
            anim.SetBool(animID_MoveEnabled, true);
            weapon.SetActive(false);
        }
    }
}
