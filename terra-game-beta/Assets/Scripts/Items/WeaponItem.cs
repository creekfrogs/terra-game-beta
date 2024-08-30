using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : Item
{
    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int kimaDamage = 0;
    public int fireDamage = 0;
    public int lightningDamage = 0;

    [Header("Weapon Base Poise Damage")]
    public float poiseDamage = 10;

    [Header("Attack Modifiers")]
    public float lightAttack01Mod = 1.1f;

    [Header("Stamina Costs Modifiers")]
    public int baseStaminaCost = 20;
    public float lightAttackStaminaMod = 0.9f;

    [Header("Actions")]
    public WeaponItemAction oh_rb_Action;
}
