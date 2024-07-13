using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Omega Studios/Character Effects/Instant Effects/Take Damage")]
public class EFFECT_TakeDamageEffect : InstantCharacterEffect
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
}
