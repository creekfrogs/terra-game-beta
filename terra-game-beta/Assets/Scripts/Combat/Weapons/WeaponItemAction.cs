using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Omega Studios/Character Actions/Weapon Action/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;

    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if(playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.currentWeapon.Value = weaponPerformingAction.itemID;
        }
    }
}
