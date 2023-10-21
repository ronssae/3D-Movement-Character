using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator PlayerAnimator;
    InputManager Input_Manager;
    PlayerLocomotion Player_Locomotion;
    CameraManager Camera_Manager;

    public bool PlayerIsInteracting;

    private void Awake()
    {
        Input_Manager = GetComponent<InputManager>();
        Camera_Manager = FindObjectOfType<CameraManager>();
        Player_Locomotion = GetComponent<PlayerLocomotion>();
        PlayerAnimator = GetComponent<Animator>();
    }
    private void Update()
    {
        Input_Manager.HandleAllInput();
    }
    private void FixedUpdate()
    {
        Player_Locomotion.HandleAllMovement();
    }
    private void LateUpdate()
    {
        Camera_Manager.HandleAllCameraMovement();

        PlayerIsInteracting = PlayerAnimator.GetBool("PlayerIsInteracting");
        Player_Locomotion.PlayerIs_Jumping = PlayerAnimator.GetBool("PlayerIsJumping");
        PlayerAnimator.SetBool("PlayerIsGrounded", Player_Locomotion.PlayerIs_Grounded);
    }

}
