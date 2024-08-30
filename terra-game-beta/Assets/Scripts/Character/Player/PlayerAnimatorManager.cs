using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorManager : CharacterAnimatorManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    private void OnAnimatorMove()
    {
        if(player.applyRootMotion)
        {
            Vector3 velocity = player.characterAnimator.deltaPosition;
            player.playerLocomotionManager.characterController.Move(velocity * Time.deltaTime);
            player.transform.rotation *= player.characterAnimator.deltaRotation;
        }
    }
}
