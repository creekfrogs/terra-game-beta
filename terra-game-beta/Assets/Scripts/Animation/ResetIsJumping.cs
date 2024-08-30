using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetIsJumping : StateMachineBehaviour
{
    CharacterManager character;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }

        character.isJumping = false;
    }
}
