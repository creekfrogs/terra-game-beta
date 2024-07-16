using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Equipment")]
    public NetworkVariable<int> currentWeapon = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void SetCharacterActionHand(bool rightHandedAction)
    {
        if(rightHandedAction)
        {
            isUsingRightHand.Value = true;
            isUsingLeftHand.Value = false;
        }
        else
        {
            isUsingRightHand.Value = false;
            isUsingLeftHand.Value = true;
        }
    }

    public void SetNewMaxHealthValue(int oldEssence, int newEssence)
    {
        maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnLvl(newEssence);
        PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
        currentHealth.Value = maxHealth.Value;
    }

    public void SetNewMaxStaminaValue(int oldVitality, int newVitality)
    {
        maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnLvl(newVitality);
        PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
        currentStamina.Value = maxStamina.Value;
    }

    public void OnCurrentWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = WorldItemDatabase.instance.GetWeaponByID(newID);
        player.playerCombatManager.currentWeapon = newWeapon;
    }

    [Rpc(SendTo.Server)]
    public void NotifyServerOfActionWeaponRpc(ulong clientID, int actionID, int weaponID)
    {
        if (IsServer)
        {
            NotifyClientsOfActionWeaponRpc(clientID, actionID, weaponID);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void NotifyClientsOfActionWeaponRpc(ulong clientID, int actionID, int weaponID)
    {
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponAction(actionID, weaponID);
        }
    }

    private void PerformWeaponAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponAction = WorldActionDatabase.instance.GetWeaponItemAction(actionID);

        if(weaponAction != null)
        {
            weaponAction.AttemptToPerformAction(player, WorldItemDatabase.instance.GetWeaponByID(weaponID));
        }
        else
        {
            Debug.LogError("ACTION IS NULL, ACTION NOT PERFORMED");
        }
    }
}