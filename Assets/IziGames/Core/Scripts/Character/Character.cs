using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

[Icon("Assets/IziGames/Gizmos/character.png")]
public class Character : MonoBehaviour
{
    public bool isPlayer = false;

    [Header("Movement")]
    public float speed = 5f;

    public float sprintSpeed = 10f;
    public float walkSpeed = 3f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 2f;
    public float accelerationdelta = 0.1f;

    [Header("Collider")]
    public float Height = 2f;
    public float Radius = 0.5f;

    public float centerOffset = 0.0f;

    [Header("Physics")]
    public float Weight = 80f;
    public float gravity = 9.81f;

    [Header("Animations")]
    public LocomotionState animationState;
    public Animator animator;
    public GameObject mannequin;

    private CharacterController controller;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction crouchAction;
    private InputAction sprintAction;
    private float verticalVelocity = 0f;

    void Awake()
    {
        if (mannequin == null) {
            mannequin = gameObject;
        }
        
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.stepOffset = Mathf.Min(0.3f, Height / 2 + Radius);
        }
        controller.hideFlags = HideFlags.HideInInspector;
    }

    void Start()
    {
        LocomotionAnimationSetup.ApplyLocomotionStateToCharacter(this);
        var inputActionsAsset = Resources.Load<InputActionAsset>("CharacterActions");
        moveAction = inputActionsAsset.FindAction("Move", true);
        jumpAction = inputActionsAsset.FindAction("Jump", true);
        crouchAction = inputActionsAsset.FindAction("Crouch", true);
        sprintAction = inputActionsAsset.FindAction("Sprint", true);
        jumpAction.performed += ctx => Jump();
        crouchAction.performed += ctx => Crouch(true);
        crouchAction.canceled += ctx => Crouch(false);
        sprintAction.performed += ctx => Sprint(true);
        sprintAction.canceled += ctx => Sprint(false);

        if (isPlayer) {
            moveAction.Enable();
            jumpAction.Enable();
            crouchAction.Enable();
            sprintAction.Enable();
        }
        if (animator == null && mannequin != null) {
            animator = mannequin.GetComponent<Animator>();
        }
    }

    void OnEnable()
    {
        if (isPlayer) {
            moveAction?.Enable();
            jumpAction?.Enable();
            crouchAction?.Enable();
            sprintAction?.Enable();
        }
    }

    void Update()
    {
        if (isPlayer)
            Move();
        UpdateControllerCollider();
        UpdateAnimator();
    }

    void UpdateAnimator()
    {
        if (animator == null) return;
        Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        float targetSpeedX = input.x;
        float targetSpeedZ = input.y;
        float currentSpeedX = animator.GetFloat("Speed-X");
        float currentSpeedZ = animator.GetFloat("Speed-Z");
        float dampTime = animationState != null ? 1.0f / animationState.blendingSpeed : 0.1f;
        animator.SetFloat("Speed-X", Mathf.Lerp(currentSpeedX, targetSpeedX, Time.deltaTime * 10f));
        animator.SetFloat("Speed-Z", Mathf.Lerp(currentSpeedZ, targetSpeedZ, Time.deltaTime * 10f));
        float speedMagnitude = input.magnitude;
        float currentSpeed = animator.GetFloat("Speed");
        float targetSpeed = speedMagnitude * (speed / walkSpeed);
        animator.SetFloat("Speed", Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f));
        animator.SetFloat("Grounded", controller.isGrounded ? 1f : 0f);
        animator.SetFloat("Stand", Height >= 1.9f ? 1f : 0.5f);
        animator.SetFloat("Speed-Y", verticalVelocity);
}

    void Crouch(bool isCrouching)
    {
        if (isCrouching) {
            speed = crouchSpeed;
            Height = 1f;
            Radius = 0.5f;
            centerOffset = 0.5f;
        } else {
            speed = walkSpeed;
            Height = 2f;
            Radius = 0.5f;
            centerOffset = 0.0f;
        }
    }

    void Sprint(bool isSprinting)
    {
        if (isSprinting) {
            speed = sprintSpeed;
        } else {
            speed = walkSpeed;
        }
    }

    void Jump()
    {
        if (isPlayer && controller.isGrounded) {
            verticalVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
        }
    }

    void Move()
    {
        if (controller == null ) return;
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 horizontalMove = transform.right * input.x + transform.forward * input.y;
        if (controller.isGrounded && verticalVelocity < 0) {
            verticalVelocity = -2f;
        }
        else {
            verticalVelocity -= gravity * Time.deltaTime;
        }
        Vector3 move = horizontalMove * speed;
        move.y = verticalVelocity;
        controller.Move(move * Time.deltaTime);
    }

    void UpdateControllerCollider()
    {
        controller.height = Height;
        controller.radius = Radius;
        float maxStepOffset = Height / 2 + Radius;
        if (controller.stepOffset > maxStepOffset) {
            controller.stepOffset = maxStepOffset;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        float h = Mathf.Max(0, Height - 2 * Radius);
        Vector3 center = new Vector3(0, 0, 0);

        Vector3 bottom = center + Vector3.down * (h / 2f);
        Vector3 top = center + Vector3.up * (h / 2f);

        Gizmos.DrawWireSphere(bottom, Radius);
        Gizmos.DrawWireSphere(top, Radius);
        Gizmos.DrawWireCube(center, new Vector3(Radius * 2, h, Radius * 2));

        Gizmos.matrix = oldMatrix;
    }

    void OnDisable()
    {
        if (isPlayer)
        {
            moveAction?.Disable();
            jumpAction?.Disable();
            crouchAction?.Disable();
            sprintAction?.Disable();
        }
    }
}