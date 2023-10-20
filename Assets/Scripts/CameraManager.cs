using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    InputManager Input_Manager;

    public Transform TargetTransform;    //The object that the camera will follow
    public Transform CameraPivot;    //The object that the camera will use to pivot (Look Up and Down)
    public Transform CameraTransform; //The transform of the actual camera object in the scene
    private float DefaultPosition;
    public LayerMask CollisionLayers; //The layers we want our camera to collide with

    private Vector3 CameraVectorPosition;
    private Vector3 CameraFollowVelocity = Vector3.zero;

    public float CameraCollisionOffset; //How much the camera will jump off of objects its colliding with
    public float MinCollisionOffset;
    public float CameraCollisionRadius;
    public float CameraFollowSpeed = 0.2f;
    public float CameraLookSpeed;
    public float CameraPivotSpeed;

    public float LookAngle; // For Camera to look up and down
    public float PivotAngle; // For Camera to look left and right
    public float MinPivotAngle;
    public float MaxPivotAngle;

    private void Awake()
    {
        Input_Manager = FindObjectOfType<InputManager>();
        TargetTransform = FindObjectOfType<PlayerManager>().transform;
        CameraTransform = Camera.main.transform;
        DefaultPosition = CameraTransform.localPosition.z;
    }
    public void HandleAllCameraMovement()
    {
        FollowTarget();
        RotateCamera();
        CameraCollisions();
    }
    private void FollowTarget()
    {
        Vector3 TargetPosition = Vector3.SmoothDamp(transform.position, TargetTransform.position, ref CameraFollowVelocity, CameraFollowSpeed);
        transform.position = TargetPosition;
    }
    private void RotateCamera()
    {
        Vector3 CameraRotation;
        Quaternion TargetRotation;

        LookAngle = LookAngle + (Input_Manager.CameraInput_X * CameraLookSpeed);
        PivotAngle = PivotAngle - (Input_Manager.CameraInput_Y * CameraPivotSpeed);
        PivotAngle = Mathf.Clamp(PivotAngle, MinPivotAngle, MaxPivotAngle);

        CameraRotation = Vector3.zero;
        CameraRotation.y = LookAngle;
        TargetRotation = Quaternion.Euler(CameraRotation);
        transform.rotation = TargetRotation;

        CameraRotation = Vector3.zero;
        CameraRotation.x = PivotAngle;
        TargetRotation = Quaternion.Euler(CameraRotation);
        CameraPivot.localRotation = TargetRotation;
    }
    private void CameraCollisions()
    {
        float TargetPosition = DefaultPosition;
        RaycastHit Hit;
        Vector3 Direction = CameraTransform.position - CameraPivot.position;
        Direction.Normalize();

        if (Physics.SphereCast(CameraPivot.transform.position, CameraCollisionRadius, Direction, out Hit, Mathf.Abs(TargetPosition), CollisionLayers))
        {
            float distance = Vector3.Distance(CameraPivot.position, Hit.point);
            TargetPosition = -(distance - CameraCollisionOffset);

        }

        if (Mathf.Abs(TargetPosition) < MinCollisionOffset)
        {
            TargetPosition = TargetPosition - MinCollisionOffset;
        }

        CameraVectorPosition.z = Mathf.Lerp(CameraTransform.localPosition.z, TargetPosition, 0.2f);
        CameraTransform.localPosition = CameraVectorPosition;
    }
}
