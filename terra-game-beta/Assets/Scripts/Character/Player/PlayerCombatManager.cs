using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeapon;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void PerformWeaponAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        if(player.IsOwner)
        {
            weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
            player.playerNetworkManager.NotifyServerOfActionWeaponRpc(NetworkManager.Singleton.LocalClientId, weaponAction.actionID, weaponPerformingAction.itemID);
        }
    }

    public void DrainStaminaBasedOnAttack()
    {
        if (!player.IsOwner)
            return;

        if (currentWeapon == null)
            return;

        float staminaDeducted = 0;

        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeapon.baseStaminaCost * currentWeapon.lightAttackStaminaMod;
                break;
            default:
                break;
        }

        player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);
        
        if(player.IsOwner)
        {
            PlayerCamera.instance.SetLockCameraHeight();
        }
    }
}
