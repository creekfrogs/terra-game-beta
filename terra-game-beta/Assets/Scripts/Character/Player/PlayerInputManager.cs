using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerManager player;

    PlayerControls playerControls;

    [Header("Movement")]
    [SerializeField] Vector2 movementInput;
    public float moveAmount;
    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;

    [Header("Camera")]
    [SerializeField] Vector2 cameraInput;
    [HideInInspector] public float cameraHorizontalInput;
    [HideInInspector] public float cameraVerticalInput;

    [Header("Player Actions")]
    [SerializeField] bool jump_Input;
    [SerializeField] bool rb_Input;

    [Header("Lock On")]
    [SerializeField] bool lockOn_Input;
    [SerializeField] bool lockOnLeft_Input;
    [SerializeField] bool lockOnRight_Input;
    private Coroutine lockOnCoroutine;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
        if(playerControls != null)
        {
            playerControls.Disable();
        }
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;

            if(playerControls != null)
            {
                playerControls.Enable();
            }
        }
        else
        {
            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
        }

        playerControls.Locomotion.Move.performed += i => movementInput = i.ReadValue<Vector2>();
        playerControls.Camera.Look.performed += i => cameraInput = i.ReadValue<Vector2>();

        playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
        playerControls.PlayerActions.RB.performed += i => rb_Input = true;

        playerControls.PlayerActions.LockOn.performed += i => lockOn_Input = true;
        playerControls.PlayerActions.SeekLeftLockOnTarget.performed += i => lockOnLeft_Input = true;
        playerControls.PlayerActions.SeekRightLockOnTarget.performed += i => lockOnRight_Input = true;


        playerControls.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }

    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void Update()
    {
        HandleMovementInput();
        HandleCameraMovementInput();
        HandleJumpInput();
        HandleRBInput();
        HandleLockOnInput();
        HandleLockOnSeekingInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = (Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));
        moveAmount = Mathf.Clamp01(moveAmount);

        if (moveAmount > 0 && moveAmount < 0.5)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount >= 0.5 && moveAmount < 1)
        {
            moveAmount = 1f;
        }

        if(player.playerNetworkManager.isLockedOn.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParams(horizontalInput, verticalInput);
        }
        else
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParams(0, moveAmount);
        }
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }

    private void HandleJumpInput()
    {
        if(jump_Input)
        {
            jump_Input = false;
            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }
    private void HandleRBInput()
    {
        if(rb_Input)
        {
            rb_Input = false;

            //if UI open, return

            player.playerNetworkManager.SetCharacterActionHand(true);

            //if two handing, run 2h action

            player.playerCombatManager.PerformWeaponAction(player.playerInventoryManager.currentRightHandWeapon.oh_rb_Action, player.playerInventoryManager.currentRightHandWeapon);
        }
    }

    private void HandleLockOnInput()
    {
        if(player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null)
                return;

            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;

                if (lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine);

                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }
        }

        if (lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;
            player.playerNetworkManager.isLockedOn.Value = false;
            PlayerCamera.instance.ClearLockOnTargets();
            // DISABLE LOCK ON
            return;
        }

        if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;

            // IF USING A RANGED WEAPON AND AIMING DO NOT ALLOW LOCK ON

            PlayerCamera.instance.HandleFindLockOnTargets();

            if(PlayerCamera.instance.nearestLockOnTarget != null)
            {
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }

    private void HandleLockOnSeekingInput()
    {
        if(lockOnLeft_Input)
        {
            lockOnLeft_Input = false;

            if(player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleFindLockOnTargets();

                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }
        if (lockOnRight_Input)
        {
            lockOnRight_Input = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleFindLockOnTargets();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }
    }
}
