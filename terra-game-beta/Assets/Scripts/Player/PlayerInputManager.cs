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

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        SceneManager.activeSceneChanged += OnSceneChanged;
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
        }
        else
        {
            instance.enabled = false;
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

        player.playerAnimatorManager.UpdateAnimatorMovementParams(horizontalInput, verticalInput);
    }

    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;
    }
}
