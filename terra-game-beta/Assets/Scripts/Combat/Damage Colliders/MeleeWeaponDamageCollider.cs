using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")]
    public float lightAttack01Mod;

    protected override void Awake()
    {
        base.Awake();
        if(damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }
        damageCollider.enabled = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if (damageTarget == characterCausingDamage)
            return;

        if (damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            DamageTarget(damageTarget);
        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
            return;

        charactersDamaged.Add(damageTarget);

        EFFECT_TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.kimaDamage = kimaDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = fireDamage;
        damageEffect.contactPoint = contactPoint;

        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(lightAttack01Mod, damageEffect);
                break;
            default:
                break;
        }

        //damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

        if(characterCausingDamage.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifiyServerOfCharacterDamageRpc(
                damageTarget.NetworkObjectId, 
                characterCausingDamage.NetworkObjectId,
                damageEffect.physicalDamage,
                damageEffect.kimaDamage,
                damageEffect.fireDamage,
                damageEffect.lightningDamage,
                damageEffect.poiseDamage,
                damageEffect.angleHitFrom,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z
                );
        }
    }

    private void ApplyAttackDamageModifiers(float modifier, EFFECT_TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.kimaDamage *= modifier;
        damage.fireDamage *= modifier;
        damage.lightningDamage *= modifier;
        damage.poiseDamage *= modifier;
    }
}
