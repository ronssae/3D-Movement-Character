using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager Player_Manager;
    AnimatorManager Animator_Manager;
    InputManager Input_Manager;

    Vector3 MovementDirection;
    Transform CameraObject;
    Rigidbody PlayerRigidBody;

    [Header("Falling Status")]
    public float PlayerAirTimer;
    public float LeapingVelocity;
    public float FallingVelocity;
    public float RayCastHeightOffset = 0.5f;
    public LayerMask GroundLayer;

    [Header("Movement Flags")]
    public bool PlayerIs_Sprinting;
    public bool PlayerIs_Grounded;
    public bool PlayerIs_Jumping;

    [Header("Movement Speeds")]
    public float WalkingSpeed;
    public float RunningSpeed;
    public float SprintingSpeed;
    public float RotationSpeed;

    [Header("Jumping Status")]
    public float JumpHeight;
    public float GravityIntensity;

    private void Awake()
    {
        Player_Manager = GetComponent<PlayerManager>();
        Animator_Manager = GetComponent<AnimatorManager>();
        Input_Manager = GetComponent<InputManager>();
        PlayerRigidBody = GetComponent<Rigidbody>();
        CameraObject = Camera.main.transform;
    }
    public void HandleAllMovement()
    {
        HandleFallingandLanding();
        if (Player_Manager.PlayerIsInteracting)
            return;
        HandleMovement();
        HandleRotation();
    }
    private void HandleMovement()
    {
        if (PlayerIs_Jumping)
            return;
        MovementDirection = new Vector3(CameraObject.forward.x, 0f, CameraObject.forward.z) * Input_Manager.VerticalInput; //Movement Input
        MovementDirection = MovementDirection + CameraObject.right * Input_Manager.HorizontalInput;
        MovementDirection.Normalize();
        MovementDirection.y = 0;

        if (PlayerIs_Sprinting)
        {
            MovementDirection = MovementDirection * SprintingSpeed;
        }
        else
        {
            if (Input_Manager.MovementAmount >= 0.5f)
            {
                MovementDirection = MovementDirection * RunningSpeed;
            }
            else
            {
                MovementDirection = MovementDirection * WalkingSpeed;
            }
        }


        MovementDirection = MovementDirection * RunningSpeed;

        Vector3 Movement_Velocity = MovementDirection;
        PlayerRigidBody.velocity = Movement_Velocity;
    }
    private void HandleRotation()
    {
        if (PlayerIs_Jumping)
            return;
        Vector3 TargetDirection = Vector3.zero;

        TargetDirection += CameraObject.forward * Input_Manager.VerticalInput;
        TargetDirection += TargetDirection + CameraObject.right * Input_Manager.HorizontalInput;
        TargetDirection.y = 0;

        if (TargetDirection == Vector3.zero)
        {
            TargetDirection = transform.forward;
        }

        Quaternion TargetRotation = Quaternion.LookRotation(TargetDirection);
        Quaternion PlayerRotation = Quaternion.Slerp(transform.rotation, TargetRotation, RotationSpeed * Time.deltaTime);

        transform.rotation = PlayerRotation;
    }
    private void HandleFallingandLanding()
    {
        RaycastHit Hit;
        Vector3 RayCastOrigin = transform.position;
        Vector3 TargetPosition;
        RayCastOrigin.y = RayCastOrigin.y + RayCastHeightOffset;
        TargetPosition = transform.position;

        if (!PlayerIs_Grounded && !PlayerIs_Jumping)
        {
            if (!Player_Manager.PlayerIsInteracting)
            {
                Animator_Manager.PlayTargetAnimation("Falling", true);
            }

            PlayerAirTimer = PlayerAirTimer + Time.deltaTime;
            PlayerRigidBody.AddForce(transform.forward * LeapingVelocity);
            PlayerRigidBody.AddForce(-Vector3.up * FallingVelocity * PlayerAirTimer);
        }

        if (Physics.SphereCast(RayCastOrigin, 0.2f, -Vector3.up, out Hit, GroundLayer))
        {
            if (!PlayerIs_Grounded && !Player_Manager.PlayerIsInteracting)
            {
                Animator_Manager.PlayTargetAnimation("Landing", true);
            }

            Vector3 RayCastHitPoint = Hit.point;
            TargetPosition.y = RayCastHitPoint.y;
            PlayerAirTimer = 0;
            PlayerIs_Grounded = true;
        }
        else
        {
            PlayerIs_Grounded = false;
        }

        if (PlayerIs_Grounded && !PlayerIs_Jumping)
        {
            if (Player_Manager.PlayerIsInteracting || Input_Manager.MovementAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, TargetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = TargetPosition;
            }
        }
    }
    public void HandleJumping()
    {
        if (PlayerIs_Grounded)
        {
            Animator_Manager.PlayerAnimator.SetBool("PlayerIsJumping", true);
            Animator_Manager.PlayTargetAnimation("Jumping", false);

            float JumpingVelocity = Mathf.Sqrt(-2 * GravityIntensity * JumpHeight);
            Vector3 PlayerVelocity = MovementDirection;
            PlayerVelocity.y = JumpingVelocity;
            PlayerRigidBody.velocity = PlayerVelocity;
        }
    }
}
