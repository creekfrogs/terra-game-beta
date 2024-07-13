using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : CharacterController
{
    PlayerManager player;
    public Rigidbody rb;

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

            player.playerAnimatorManager.UpdateAnimatorMovementParams(0, moveAmount);
        }
    }
    public void HandleAllMovement()
    {
        StateMachine();

        HandleGroundedMovement();
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
            rb.velocity = moveDirection * runningSpeed;
        }
        else if (PlayerInputManager.instance.moveAmount <= 0.5f)
        {
            rb.velocity = moveDirection * walkingSpeed;
        }
    }

    private void HandleRotation()
    {
        if (!player.canRotate)
            return;

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
}
