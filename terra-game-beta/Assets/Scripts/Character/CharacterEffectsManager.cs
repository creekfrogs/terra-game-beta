using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    //PROCESS INSTANT EFFECTS

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        if(bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        //Default
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
    }

    // PROCESS TIMED EFFECTS

    // PROCESS STATIC EFFECTS


}
