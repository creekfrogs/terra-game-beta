using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    MeleeWeaponDamageCollider meleeDamageCollider;

    private void Awake()
    {
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    public void SetWeaponDamage(CharacterManager attacker, WeaponItem weapon)
    {
        meleeDamageCollider.characterCausingDamage = attacker;
        meleeDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeDamageCollider.kimaDamage = weapon.kimaDamage;
        meleeDamageCollider.fireDamage = weapon.fireDamage;
        meleeDamageCollider.lightningDamage = weapon.lightningDamage;
    }
}
