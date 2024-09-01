using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Omega Studios/Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0; // Will be split into "Standard", "Strike", "Slash", "Pierce"
    public float kimaDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    private int finalDamage;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;

    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manualDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool playDamageSFX = true;
    public AudioClip elementalDamageSoundFx;

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;
    public Vector3 contactPoint;

    public override void ProcessEffect(CharacterManager character)
    {
        base.ProcessEffect(character);

        if (character.isDead.Value)
            return;

        CalculateDamage(character);
        PlayDirectionalAnimation(character);
        PlayDamageVFX(character);
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner)
        {
            return;
        }

        if(characterCausingDamage != null)
        {
            // Check for modifiers
        }

        finalDamage = Mathf.RoundToInt(physicalDamage + kimaDamage + fireDamage + lightningDamage);

        if (finalDamage <= 0)
        {
            finalDamage = 1;
        }

        character.characterNetworkManager.currentHealth.Value -= finalDamage;
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        //Play Kima VFX

        character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }

    //PlayDamageSFX

    private void PlayDirectionalAnimation(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        if (character.isDead.Value)
            return;

        poiseIsBroken = true;

        if(angleHitFrom >= 145 && angleHitFrom <= 180)
        {
            damageAnimation = character.characterAnimatorManager.hit_Forward;
        }
        else if(angleHitFrom <= -145 && angleHitFrom >= -180)
        {
            damageAnimation = character.characterAnimatorManager.hit_Forward;
        }
        else if(angleHitFrom >= -45 && angleHitFrom <= 45)
        {
            damageAnimation = character.characterAnimatorManager.hit_Backward;
        }
        else if(angleHitFrom >= -144 && angleHitFrom <= -45)
        {
            damageAnimation = character.characterAnimatorManager.hit_Left;
        }
        else if(angleHitFrom >= 45 && angleHitFrom <= 144)
        {
            damageAnimation = character.characterAnimatorManager.hit_Right;
        }

        if(poiseIsBroken)
        {
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
    }
}
