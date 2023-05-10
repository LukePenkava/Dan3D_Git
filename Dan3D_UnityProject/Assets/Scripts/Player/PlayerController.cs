using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player 
{    
    public class PlayerController : MonoBehaviour
    {
        CharacterController controller;
        UIManager uiManager;

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

            uiManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<UIManager>();            

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
            input.Player.Inventory.performed += OnInventoryPerformed;
            input.Player.UINavigation.performed += OnUINavigationPerformed;
        }    

        void OnDisable() {
            input.Player.Disable();
            input.Player.Movement.performed -= OnMovementPerformed;
            input.Player.Movement.canceled -= OnMovementCancelled;
            input.Player.Interact.performed -= OnSprintPerformed;
            input.Player.Interact.performed -= OnInteractPerformed;
            input.Player.Attack.performed -= OnAttackPerformed;
            input.Player.Inventory.performed -= OnInventoryPerformed;
            input.Player.UINavigation.performed -= OnUINavigationPerformed;
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
            
            if(GameDirector.gameState == GameDirector.GameState.World) {
                moveVector = context.ReadValue<Vector2>();
            }
        }

        void OnMovementCancelled(InputAction.CallbackContext context) {
            moveVector = Vector2.zero;
        }

         void OnUINavigationPerformed(InputAction.CallbackContext context) {        
            inputDevice = context.control.device.displayName;      
            
            if(GameDirector.gameState == GameDirector.GameState.UI) {
                Vector2 uiInput = context.ReadValue<Vector2>();
                uiManager.NavigateUI(uiInput);
            }
        }

        void OnSprintPerformed(InputAction.CallbackContext context) {            
            sprint = context.ReadValue<float>() == 1f ? true : false;       
        }    

        void OnInteractPerformed(InputAction.CallbackContext context) {
            
            if(context.ReadValue<float>() == 1f) {
                if(GameDirector.gameState == GameDirector.GameState.World) {
                    TriggerInteraction();
                } else if(GameDirector.gameState == GameDirector.GameState.UI) {
                    uiManager.ActivateSelectedElement(); 
                }
            }                  
        }  

        void OnAttackPerformed(InputAction.CallbackContext context) {
            if(context.ReadValue<float>() == 1f) {                
                Attack();
            } 
        }

        void OnInventoryPerformed(InputAction.CallbackContext context) {
            if(context.ReadValue<float>() == 1f) {                
                uiManager.PlayerInventoryInput();
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
