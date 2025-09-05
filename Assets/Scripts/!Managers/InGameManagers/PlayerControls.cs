using System;
using UnityEngine;
using UnityEngine.InputSystem;
using PixelMate.Input;

public class PlayerControls : Singleton<PlayerControls> //Revisar
{
    //Mouse
    public float MouseSensitivity;
    public bool InvertLookX;
    public bool InvertLookY;
    
    //Gameplay
    public event Action OnSprintKeyStarted, OnSprintKeyCancelled;
    public event Action OnCrouchKey;
    public event Action OnJumpKey;
    public event Action OnStartPrimaryInteractKey, OnEndPrimaryInteractKey;
    public event Action OnStartSecondaryInteractKey, OnEndSecondaryInteractKey;
    public event Action OnPreviousSlotKey, OnNextSlotKey;
    public event Action<int> OnSelectInventorySlotKey;
    public event Action OnGamePlayCancelKey, OnMenuCancelKey;
    public event Action OnClickAttack;
    public event Action OnReleaseAttack;
    public event Action OnPause;

    private InputSystem_Actions _inputActions;

    #region Unity Functions

    protected override void Awake()
    {
        base.Awake();

        _inputActions = new InputSystem_Actions();
        SubscribeToAllActionEvents();
    }

    private void OnEnable()
    {
        _inputActions.Enable();

        _inputActions.Player.Attack.performed -= HandleOnAttackButton;
        _inputActions.Player.Attack.performed += HandleOnAttackButton;

        _inputActions.Player.Attack.canceled -= HandleOnReleaseAttackButton;
        _inputActions.Player.Attack.canceled += HandleOnReleaseAttackButton;
    }

    private void OnDisable()
    {
        if (_inputActions == null)
            return;

        _inputActions.Player.Attack.performed -= HandleOnAttackButton;
        _inputActions.Player.Attack.canceled -= HandleOnReleaseAttackButton;
        _inputActions.Disable();
    }

    #endregion

    private void SubscribeToAllActionEvents()
    {
        //Gameplay
        var gameplayActionMap = GameManager.Controls.Gameplay;
        gameplayActionMap.Sprint.started += KEY_Sprint;
        gameplayActionMap.Sprint.performed += KEY_Sprint;
        gameplayActionMap.Sprint.canceled += KEY_Sprint;
        gameplayActionMap.Crouch.started += KEY_Crouch;
        gameplayActionMap.Jump.started += KEY_Jump;
        gameplayActionMap.Pause.started += KEY_Pause;
    }
    
    
    #region Mouse
    public static Vector3 MouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }
    
    public bool IsMouseMove()
    {
        return GameManager.Controls.Gameplay.Look.ReadValue<Vector2>() != Vector2.zero;
    }
    
    public Vector2 MouseDelta()
    {
        Vector2 mouseDelta = GameManager.Controls.Gameplay.Look.ReadValue<Vector2>(); 
        //Apply mouse sensitivity;
        mouseDelta *=  MouseSensitivity / 10f;
        //Invert input;
        if (InvertLookX) mouseDelta *= Vector2.left;
        if (InvertLookY) mouseDelta *= Vector2.down;
        return mouseDelta;
    }
    #endregion
    
    
    #region Gameplay Inputs
    public Vector2 InputMovement()
    {
        return GameManager.Controls.Gameplay.Move.ReadValue<Vector2>();
    }
    
    private void KEY_Sprint(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSprintKeyStarted?.Invoke();
    }

    private void KEY_Crouch(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnCrouchKey?.Invoke();
    }
    
    private void KEY_Jump(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnJumpKey?.Invoke();
    }
    
    private void KEY_PrimaryInteraction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnStartPrimaryInteractKey?.Invoke();
        if (callbackContext.canceled) OnEndPrimaryInteractKey?.Invoke();
    }

    private void KEY_SecondaryInteraction(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnStartSecondaryInteractKey?.Invoke();
        if (callbackContext.canceled) OnEndSecondaryInteractKey?.Invoke();
    }
    
    private void KEY_PreviousSlot(InputAction.CallbackContext callbackContext)
    {
        OnPreviousSlotKey?.Invoke();
    }
    
    private void KEY_NextSlot(InputAction.CallbackContext callbackContext)
    {
        OnNextSlotKey?.Invoke();
    }
    
    private void KEY_SelectSlot1(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(1);
    }
    
    private void KEY_SelectSlot2(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(2);
    }
    
    private void KEY_SelectSlot3(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(3);
    }
    
    private void KEY_SelectSlot4(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(4);
    }
    
    private void KEY_SelectSlot5(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(5);
    }
    
    private void KEY_SelectSlot6(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(6);
    }
    
    private void KEY_SelectSlot7(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(7);
    }
    
    private void KEY_SelectSlot8(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(8);
    }
    
    private void KEY_SelectSlot9(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(9);
    }
    
    private void KEY_SelectSlot10(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnSelectInventorySlotKey?.Invoke(0);
    }
    
    private void KEY_Gameplay_Cancel(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnGamePlayCancelKey?.Invoke();
    }

    private void HandleOnAttackButton(InputAction.CallbackContext context)
    {
        if (context.performed)
            OnClickAttack?.Invoke();
    }

    private void HandleOnReleaseAttackButton(InputAction.CallbackContext context)
    {
        if (context.canceled)
            OnReleaseAttack?.Invoke();
    }

    private void KEY_Pause(InputAction.CallbackContext context)
    {
        if (PauseManager.Instance == null) return;
        else PauseManager.Instance.Pause();
    }

    #endregion


    #region Menu Inputs
    private void KEY_Menu_Cancel(InputAction.CallbackContext callbackContext)
    {
        if (callbackContext.started) OnMenuCancelKey?.Invoke();
    }
    #endregion
}