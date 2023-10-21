using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls Player_Controls;
    PlayerLocomotion Player_Locomotion;
    AnimatorManager Animator_Manager;

    public Vector2 Movement_Input;
    public Vector2 Camera_Input;

    public float CameraInput_X;
    public float CameraInput_Y;

    public float MovementAmount;

    public float VerticalInput;
    public float HorizontalInput;

    public bool Shift_Input;
    public bool Jump_Input;

    private void Awake()
    {
        Animator_Manager = GetComponent<AnimatorManager>();
        Player_Locomotion = GetComponent<PlayerLocomotion>();
    }
    private void OnEnable()
    {
        if (Player_Controls == null) 
        { 
            Player_Controls = new PlayerControls();

            Player_Controls.PlayerMovement.Movement.performed += i => Movement_Input = i.ReadValue<Vector2>();
            Player_Controls.PlayerMovement.Camera.performed += i => Camera_Input = i.ReadValue<Vector2>();

            Player_Controls.PlayerActions.Sprint.performed += i => Shift_Input = true;
            Player_Controls.PlayerActions.Sprint.canceled += i => Shift_Input = false;
            Player_Controls.PlayerActions.Jump.performed += i => Jump_Input = true;

        }
        Player_Controls.Enable();
    }
    private void OnDisable()
    {
        Player_Controls.Disable();
    }
    public void HandleAllInput()
    {
        HandleMovementInput();
        HandleSprinting();
        HandleJumpingInput();
    }
    private void HandleMovementInput()
    {
        VerticalInput = Movement_Input.y;
        HorizontalInput = Movement_Input.x;

        CameraInput_X = Camera_Input.x;
        CameraInput_Y = Camera_Input.y;

        MovementAmount = Mathf.Clamp01(Mathf.Abs(HorizontalInput) + Mathf.Abs(VerticalInput));
        Animator_Manager.UpdateAnimatorValues(0, MovementAmount, Player_Locomotion.PlayerIs_Sprinting);
    }
    private void HandleSprinting()
    {
        if (Shift_Input && MovementAmount > 0.5f)
        {
            Player_Locomotion.PlayerIs_Sprinting = true;
        }
        else
        {
            Player_Locomotion.PlayerIs_Sprinting = false;
        }
    }
    private void HandleJumpingInput()
    {
        if (Jump_Input) 
        {
            Jump_Input = false;
            Player_Locomotion.HandleJumping();
        }
    }
}
