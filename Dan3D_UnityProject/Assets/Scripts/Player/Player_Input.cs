using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Input : MonoBehaviour
{
    UIManager uiManager;
    PlayerManager playerManager;
    MinigameManager mgManager;
    DialogueManager dialogueManager;

    CustomInput input = null;

    Vector2 moveVector = Vector2.zero;

    Director.InputDevices lastInputDevice = Director.InputDevices.Keyboard;

    void Awake()
    {
        input = new CustomInput();
    }

    void Start()
    {
        uiManager = GetComponent<UIManager>();
        mgManager = GetComponent<MinigameManager>();
        dialogueManager = GetComponent<DialogueManager>();
        playerManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Movement.performed += OnMovementPerformed;
        input.Player.Movement.canceled += OnMovementCancelled;
        input.Player.Sprint.performed += OnSprintPerformed;
        input.Player.Jump.performed += OnJumpPerformed;
        input.Player.Interact.performed += OnInteractPerformed;
        input.Player.Action1.performed += OnAction1Performed;
        input.Player.Inventory.performed += OnInventoryPerformed;
        input.Player.UINavigation.performed += OnUINavigationPerformed;
        input.Player.Menu.performed += OnUIMenuPerformed;

        input.Player.ActionButton1.performed += OnActionButton1Performed;
    }

    void OnDisable()
    {
        input.Player.Disable();
        input.Player.Movement.performed -= OnMovementPerformed;
        input.Player.Movement.canceled -= OnMovementCancelled;
        input.Player.Sprint.performed -= OnSprintPerformed;
        input.Player.Jump.performed -= OnJumpPerformed;
        input.Player.Interact.performed -= OnInteractPerformed;
        input.Player.Action1.performed -= OnAction1Performed;
        input.Player.Inventory.performed -= OnInventoryPerformed;
        input.Player.UINavigation.performed -= OnUINavigationPerformed;
        input.Player.Menu.performed -= OnUIMenuPerformed;

        input.Player.ActionButton1.performed -= OnActionButton1Performed;
    }

    #region InputEvents

    void OnMovementPerformed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);

        if (GameDirector.gameState == GameDirector.GameState.World)
        {
            playerManager.MoveVector = context.ReadValue<Vector2>();
        }
        else
        {
            playerManager.MoveVector = Vector2.zero;
        }
    }

    void OnMovementCancelled(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);
        playerManager.MoveVector = Vector2.zero;
    }

    void OnUINavigationPerformed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);

        if (GameDirector.gameState == GameDirector.GameState.UI)
        {
            Vector2 uiInput = context.ReadValue<Vector2>();
            uiManager.NavigateUI(uiInput);
        }
    }

    void OnSprintPerformed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);
        playerManager.Sprint = context.ReadValue<float>() == 1f ? true : false;
    }

    void OnJumpPerformed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);
        playerManager.Jump();
    }

    void OnInteractPerformed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);

        if (context.ReadValue<float>() == 1f)
        {
            if (GameDirector.gameState == GameDirector.GameState.World)
            {
                playerManager.Interact(0);
            }
            else if (GameDirector.gameState == GameDirector.GameState.UI)
            {
                uiManager.ActivateSelectedElement();
            }
            else if (GameDirector.gameState == GameDirector.GameState.LockedDialogue)
            {
                dialogueManager.PlayerInput();
            }
        }
    }

    void OnAction1Performed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);

        if (GameDirector.gameState == GameDirector.GameState.World)
        {
            if (context.ReadValue<float>() == 1f)
            {
                playerManager.Action1();
            }
        }

        // if (GameDirector.gameState == GameDirector.GameState.MG)
        // {
        //     if (context.ReadValue<float>() == 1f)
        //     {
        //         mgManager.StopIndicator();
        //     }
        // }
    }

    void OnInventoryPerformed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);

        if (context.ReadValue<float>() == 1f)
        {
            uiManager.PlayerInventoryInput();
        }
    }

    void OnUIMenuPerformed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);

        if (context.ReadValue<float>() == 1f)
        {
            uiManager.SetGameMenu();
        }
    }

    void OnActionButton1Performed(InputAction.CallbackContext context)
    {
        SetInputDevice(context.control.device.displayName);

        if (context.ReadValue<float>() == 1f)
        {
            playerManager.ActionButton(1);
        }
    }

    void SetInputDevice(string device)
    {

        Director.InputDevices newDevice = device == "Keyboard" ? Director.InputDevices.Keyboard : Director.InputDevices.Controller;

        if (newDevice != lastInputDevice)
        {
            lastInputDevice = newDevice;
            Director.inputDevice = newDevice;
            uiManager.SetControlsHelp();
        }
    }

    #endregion
}
