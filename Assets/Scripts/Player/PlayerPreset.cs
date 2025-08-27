using UnityEngine;

[CreateAssetMenu(menuName = "Gus/PlayerPreset")]
public class PlayerPreset : ScriptableObject
{
    [Header("Controller")]
    [SerializeField, Range(0.1f, 10f)] private float normalHeight = 1.8f;
    public float NormalHeight => normalHeight;
    [SerializeField, Range(0.1f, 10f)] private float crouchHeight = 1f;
    public float CrouchHeight => crouchHeight;
    [SerializeField, Range(0f, 10f)] private float normalCameraHeight = 1.65f;
    public float NormalCameraHeight => normalCameraHeight;
    [SerializeField, Range(0f, 10f)] private float crouchCameraHeight = 1f;
    public float CrouchCameraHeight => crouchCameraHeight;
    [SerializeField, Range(0f, 1f)] private float crouchTransitionTime = 0.2f;
    public float CrouchTransitionTime => crouchTransitionTime;
    [SerializeField] private LayerMask whatIsSurface;
    public LayerMask WhatIsSurface => whatIsSurface;
    
    
    [Header("Move")]
    [SerializeField, Range(0f, 20f)] private float acceleration = 5;
    public float Acceleration => acceleration;
    [SerializeField, Range(0f, 20f)] private float crouchSpeed = 1.5f;
    public float CrouchSpeed => crouchSpeed;
    [SerializeField] private HeadBobPreset crouchHeadBobPreset;
    public HeadBobPreset CrouchHeadBobPreset => crouchHeadBobPreset;
    [SerializeField, Range(0f, 20f)] private float walkSpeed = 2;
    public float WalkSpeed => walkSpeed;
    [SerializeField] private HeadBobPreset walkHeadBobPreset;
    public HeadBobPreset WalkHeadBobPreset => walkHeadBobPreset;
    public HeadBobPreset SprintHeadBobPreset => sprintHeadBobPreset;
    [SerializeField] private bool canSprintBackward;
    public bool CanSprintBackward => canSprintBackward;
    
    [Header("Sprint")]
    [SerializeField, Range(0f, 20f)] public float minSprintSpeed = 2.5f;
    [SerializeField, Range(0f, 20f)] public float maxSprintSpeed = 4;
    [Space]
    [SerializeField, Range(0f, 20f)] public float burstSprintSpeed = 7;
    [SerializeField] private HeadBobPreset sprintHeadBobPreset;
    
    [Header("Jump")]
    [SerializeField, Range(0f, 10f)] private float jumpHeight = 1.5f;
    public float JumpHeight => jumpHeight;
    [SerializeField, Range(0f, 5f)] private float fallGravityMultiplier = 1.5f;
    public float FallGravityMultiplier => fallGravityMultiplier;
    [SerializeField, Range(0f, 0.5f)] private float coyoteTime = 0.15f;
    public float CoyoteTime => coyoteTime;
    [SerializeField, Range(0f, 0.5f)] private float jumpBufferTime = 0.15f;
    public float JumpBufferTime => jumpBufferTime;
    
    
    [Header("Fall")]
    [SerializeField, Range(1f, 2f)] private float fallingMoveSpeedMultiplier = 1.05f;
    public float FallingMoveSpeedMultiplier => fallingMoveSpeedMultiplier;
    [SerializeField, Range(0f, 2f)] private float landDuration = 0.2f;
    public float LandDuration => landDuration;
    [SerializeField, Range(0f, 2f)] private float recoveryDuration = 0.2f;
    public float RecoveryDuration => recoveryDuration;
}

public enum PlayerState
{
    Idle,
    Walking,
    Sprinting,
    Crouching,
    Falling,
    Landed,
    Recovery,
}