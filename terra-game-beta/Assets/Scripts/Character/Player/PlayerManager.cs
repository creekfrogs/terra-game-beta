using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : CharacterManager
{
    [Header("DEBUG MENU")]
    [SerializeField] bool forceKillCharacter = false;
    [SerializeField] bool forceReviveCharacter = false;

    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;

    protected override void Awake()
    {
        base.Awake();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
    }

    protected override void Update()
    {
        base.Update();

        if (!IsOwner)
            return;

        playerLocomotionManager.HandleAllMovement();
        playerStatsManager.RegenerateStamina();

        
        DebugMenu();
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if (IsOwner)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendDeathPopUp();
        }

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            WorldSaveGameManager.instance.player = this;
            PlayerUIManager.instance.player = this;

            playerNetworkManager.DebugResetStats();

            playerNetworkManager.essence.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;

            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;

            playerNetworkManager.currentWeapon.OnValueChanged += playerNetworkManager.OnCurrentWeaponIDChange;
        }

        if(playerNetworkManager.currentHealth.Value <= 0)
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
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

        Vector3 currentPosition = new Vector3(currentSaveData.xPosition, currentSaveData.yPosition + 1, currentSaveData.zPosition);
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

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();
        if(IsOwner)
        {
            isDead.Value = false;
            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

            playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
        }
    }

    private void DebugMenu()
    {
        if(forceKillCharacter)
        {
            forceKillCharacter = false;
            playerNetworkManager.currentHealth.Value = 0;
        }

        if(forceReviveCharacter)
        {
            forceReviveCharacter = false;
            ReviveCharacter();
        }
    }


}
