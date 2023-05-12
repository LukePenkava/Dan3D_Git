using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    UIManager uiManager;
    PlayerManager playerManager;
    MinigameManager mgManager;

    CustomInput input = null; 

    string inputDevice = "";
    Vector2 moveVector = Vector2.zero;

    void Awake() {
        input = new CustomInput();   
    }

    void Start() {
        uiManager = GetComponent<UIManager>();
        mgManager = GetComponent<MinigameManager>();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
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

    #region InputEvents

    void OnMovementPerformed(InputAction.CallbackContext context) {        
        inputDevice = context.control.device.displayName;      
        
        if(GameDirector.gameState == GameDirector.GameState.World) {
            playerManager.MoveVector = context.ReadValue<Vector2>();
        } else {
            playerManager.MoveVector = Vector2.zero;
        }
    }

    void OnMovementCancelled(InputAction.CallbackContext context) {        
        playerManager.MoveVector = Vector2.zero;        
    }

    void OnUINavigationPerformed(InputAction.CallbackContext context) {        
        inputDevice = context.control.device.displayName;      
        
        if(GameDirector.gameState == GameDirector.GameState.UI) {
            Vector2 uiInput = context.ReadValue<Vector2>();
            uiManager.NavigateUI(uiInput);
        }
    }

    void OnSprintPerformed(InputAction.CallbackContext context) {            
        playerManager.Sprint = context.ReadValue<float>() == 1f ? true : false;       
    }    

    void OnInteractPerformed(InputAction.CallbackContext context) {
        
        if(context.ReadValue<float>() == 1f) {
            if(GameDirector.gameState == GameDirector.GameState.World) {
                playerManager.Interact(0);
            } else if(GameDirector.gameState == GameDirector.GameState.UI) {
                uiManager.ActivateSelectedElement(); 
            }
        }                  
    }  

    void OnAttackPerformed(InputAction.CallbackContext context) {
        if(GameDirector.gameState == GameDirector.GameState.World) {
            if(context.ReadValue<float>() == 1f) {                
                playerManager.Attack();
            } 
        }

        if(GameDirector.gameState == GameDirector.GameState.MG) {
            if(context.ReadValue<float>() == 1f) {                
                mgManager.StopIndicator();
            } 
        }
    }

    void OnInventoryPerformed(InputAction.CallbackContext context) {
        if(context.ReadValue<float>() == 1f) {                
            uiManager.PlayerInventoryInput();
        } 
    }    

    #endregion
}
