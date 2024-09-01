using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    float vertical;
    float horizontal;

    [Header("Damage Animations")]
    public string hit_Forward = "huminoid_hitF";
    public string hit_Backward = "huminoid_hitB";
    public string hit_Left = "huminoid_hitL";
    public string hit_Right = "huminoid_hitR";

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void UpdateAnimatorMovementParams(float horizontal, float vertical)
    {
        if (character.characterAnimator != null)
        {
            float snappedHorizontal = 0;
            float snappedVertical = 0;

            #region Horizontal Snapping
            if (horizontal > 0 && horizontal <= 0.5f)
            {
                snappedHorizontal = 0.5f;
            }
            else if (horizontal > 0.5f && horizontal <= 1)
            {
                snappedHorizontal = 1;
            }
            else if (horizontal < 0 && horizontal >= 0.5f)
            {
                snappedHorizontal = -0.5f;
            }
            else if (horizontal < -0.5f && horizontal >= -1)
            {
                snappedHorizontal = -1;
            }
            else
            {
                snappedHorizontal = 0;
            }
            #endregion

            #region Vertical Snapping
            if (vertical > 0 && vertical <= 0.5f)
            {
                snappedVertical = 0.5f;
            }
            else if (vertical > 0.5f && vertical <= 1)
            {
                snappedVertical = 1;
            }
            else if (vertical < 0 && vertical >= 0.5f)
            {
                snappedVertical = -0.5f;
            }
            else if (vertical < -0.5f && vertical >= -1)
            {
                snappedVertical = -1;
            }
            else
            {
                snappedVertical = 0;
            }
            #endregion

            character.characterAnimator.SetFloat("horizontal", snappedHorizontal, 0.05f, Time.deltaTime);
            character.characterAnimator.SetFloat("vertical", snappedVertical, 0.05f, Time.deltaTime);
        }
    }

    public virtual void PlayTargetActionAnimation(string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        Debug.Log("PLAYING ANIMATION: " + targetAnimation);
        character.characterAnimator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.characterAnimator.applyRootMotion = applyRootMotion;
        character.canMove = canMove;
        character.canRotate = canRotate;

        character.characterNetworkManager.NotifyServerOfActionAnimationRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public virtual void PlayTargetAttackActionAnimation(AttackType attackType, string targetAnimation, bool isPerformingAction, bool applyRootMotion = true, bool canRotate = false, bool canMove = false)
    {
        character.characterCombatManager.currentAttackType = attackType;
        character.characterAnimator.CrossFade(targetAnimation, 0.2f);
        character.isPerformingAction = isPerformingAction;
        character.characterAnimator.applyRootMotion = applyRootMotion;
        character.canMove = canMove;
        character.canRotate = canRotate;


        character.characterNetworkManager.NotifyServerOfAttackActionAnimationRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }
}
