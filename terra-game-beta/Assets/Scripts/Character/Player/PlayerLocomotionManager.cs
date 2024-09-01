using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotionManager : CharacterLocomotionManager
{
    PlayerManager player;

    [Header("Locomotion")]
    private float verticalMovement;
    private float horizontalMovement;
    private float moveAmount;
    private Vector3 moveDirection;

    [Header("Rotation")]
    [SerializeField] float rotationSpeed;

    [Header("Speeds")]
    [SerializeField] float walkingSpeed = 2;
    [SerializeField] float runningSpeed = 5;

    [Header("Player Movement State")]
    public PlayerMoveState state;

    [Header("Jump Variables")]
    [SerializeField] float jumpStaminaCost = 25;
    [SerializeField] float jumpHeight = 4;
    [SerializeField] float jumpForwardSpeed = 5;
    [SerializeField] float freeFallSpeed = 2;
    private Vector3 jumpDirection;

    public enum PlayerMoveState
    {
        idle,
        walking,
        running,
        backwards,
    }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (player.IsOwner)
        {
            player.characterNetworkManager.networkVerticalMovement.Value = verticalMovement;
            player.characterNetworkManager.networkHorizontalMovement.Value = horizontalMovement;
            player.characterNetworkManager.networkMoveAmount.Value = moveAmount;
        }
        else
        {
            verticalMovement = player.characterNetworkManager.networkVerticalMovement.Value;
            horizontalMovement = player.characterNetworkManager.networkHorizontalMovement.Value;
            moveAmount = player.characterNetworkManager.networkMoveAmount.Value;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParams(horizontalMovement, verticalMovement);
            }
            else
            {
                player.playerAnimatorManager.UpdateAnimatorMovementParams(0, moveAmount);
            }
        }
    }
    public void HandleAllMovement()
    {
        StateMachine();

        HandleGroundedMovement();
        HandleJumpMovement();
        HandleFreeFallMovement();
        HandleRotation();
    }

    private void GetAllInputs()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;

        //CLAMP THESE
    }

    private void HandleGroundedMovement()
    {
        if (!player.canMove)
            return;

        GetAllInputs();

        moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
        moveDirection += PlayerCamera.instance.transform.right * horizontalMovement;
        moveDirection.Normalize();
        moveDirection.y = 0;

        if (PlayerInputManager.instance.moveAmount > 0.5f)
        {
            characterController.Move(moveDirection * runningSpeed * Time.deltaTime);
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            characterController.Move(moveDirection * walkingSpeed * Time.deltaTime);
        }
    }

    private void HandleJumpMovement()
    {
        if(player.isJumping)
        {
            characterController.Move(jumpDirection * jumpForwardSpeed * Time.deltaTime);
        }
    }

    private void HandleFreeFallMovement()
    {
        if(!player.isGrounded)
        {
            Vector3 freeFallDirection;

            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;
            freeFallDirection.y = 0;

            characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate)
            return;
        if (player.isDead.Value)
            return;
        if(player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null)
                return;

            Vector3 targetDirection;
            targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
            targetDirection.y = 0;
            targetDirection.Normalize();

            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = finalRotation;
        }
        else
        {
            Vector3 targetDirection = Vector3.zero;
            targetDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;


            if (targetDirection == Vector3.zero)
                targetDirection = transform.forward;

            if (state == PlayerMoveState.backwards)
                targetDirection = PlayerCamera.instance.cameraObject.transform.forward;

            targetDirection.Normalize();
            targetDirection.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }

    private void StateMachine()
    {
        if (verticalMovement < 0)
        {
            state = PlayerMoveState.backwards;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            state = PlayerMoveState.running;
        }
        else if (moveAmount > 0 && moveAmount <= 0.5f)
        {
            state = PlayerMoveState.walking;
        }
        else
        {
            state = PlayerMoveState.idle;
        }
    }

    public void AttemptToPerformJump()
    {
        if (player.isPerformingAction)
            return;

        if (player.playerNetworkManager.currentStamina.Value <= 0)
            return;

        if (player.isJumping)
            return;

        if (!player.isGrounded)
            return;

        player.playerAnimatorManager.PlayTargetActionAnimation("humanoid_jump_start", false);

        player.isJumping = true;

        player.playerNetworkManager.currentStamina.Value -= jumpStaminaCost;

        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;
        jumpDirection.y = 0;

        if(jumpDirection != Vector3.zero)
        {
            if (PlayerInputManager.instance.moveAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.25f;
            }
        }

    }

    public void ApplyJumpingVelocity()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }
}
