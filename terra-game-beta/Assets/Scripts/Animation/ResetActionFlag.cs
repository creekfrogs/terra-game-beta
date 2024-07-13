using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetActionFlag : StateMachineBehaviour
{
    CharacterManager character;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }

        character.isPerformingAction = false;
        character.canMove = true;
        character.canRotate = true;
        character.applyRootMotion = false;
    }
}
