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

            playerNetworkManager.essence.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
        }
    }

    public void SaveToCharacterData(ref CharacterSaveData currentSaveData)
    {
        currentSaveData.characterName = playerNetworkManager.characterName.Value.ToString();

        currentSaveData.xPosition = transform.position.x;
        currentSaveData.yPosition = transform.position.y;
        currentSaveData.zPosition = transform.position.z;

        currentSaveData.currentHealth = playerNetworkManager.currentHealth.Value;
        currentSaveData.currentStamina = playerNetworkManager.currentStamina.Value;

        currentSaveData.essence = playerNetworkManager.essence.Value;
        currentSaveData.vitality = playerNetworkManager.vitality.Value;
        currentSaveData.force = playerNetworkManager.force.Value;
        currentSaveData.finesse = playerNetworkManager.finesse.Value;
        currentSaveData.focus = playerNetworkManager.focus.Value;
        currentSaveData.precision = playerNetworkManager.precision.Value;
    }

    public void LoadToCharacterData(ref CharacterSaveData currentSaveData)
    {
        playerNetworkManager.characterName.Value = currentSaveData.characterName;

        Vector3 currentPosition = new Vector3(currentSaveData.xPosition, currentSaveData.yPosition, currentSaveData.zPosition);
        transform.position = currentPosition;

        playerNetworkManager.currentHealth.Value = currentSaveData.currentHealth;
        playerNetworkManager.currentStamina.Value = currentSaveData.currentStamina;

        playerNetworkManager.essence.Value = currentSaveData.essence;
        playerNetworkManager.vitality.Value = currentSaveData.vitality;
        playerNetworkManager.force.Value = currentSaveData.force;
        playerNetworkManager.finesse.Value = currentSaveData.finesse;
        playerNetworkManager.focus.Value = currentSaveData.focus;
        playerNetworkManager.precision.Value = currentSaveData.precision;

        playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnLvl(currentSaveData.essence);
        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnLvl(currentSaveData.vitality);
        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
    }
}
