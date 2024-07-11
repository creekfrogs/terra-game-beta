using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : CharacterManager
{
    [HideInInspector] public PlayerController playerController;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;

    protected override void Awake()
    {
        base.Awake();
        playerController = GetComponent<PlayerController>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (!IsOwner)
            return;

        playerController.HandleAllMovement();
        playerStatsManager.RegenerateStamina();

        if (Input.GetKeyDown(KeyCode.F))
        {
            playerNetworkManager.currentStamina.Value -= 10f;
        }

    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            WorldSaveGameManager.instance.player = this;

            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnLvl(playerNetworkManager.vitality.Value);
            playerNetworkManager.currentStamina.Value = playerStatsManager.CalculateStaminaBasedOnLvl(playerNetworkManager.vitality.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
        }
    }

    public void SaveToCharacterData(ref CharacterSaveData currentSaveData)
    {
        currentSaveData.characterName = playerNetworkManager.characterName.Value.ToString();

        currentSaveData.xPosition = transform.position.x;
        currentSaveData.yPosition = transform.position.y;
        currentSaveData.yPosition = transform.position.z;
    }

    public void LoadToCharacterData(ref CharacterSaveData currentSaveData)
    {
        playerNetworkManager.characterName.Value = currentSaveData.characterName;

        Vector3 currentPosition = new Vector3(currentSaveData.xPosition, currentSaveData.yPosition, currentSaveData.zPosition);
        transform.position = currentPosition;
    }
}
