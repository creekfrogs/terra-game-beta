using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public Rigidbody characterRigidbody;
    [HideInInspector] public Animator characterAnimator;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;

    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool applyRootMotion = false;
    public bool canMove = true;
    public bool canRotate = true;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterRigidbody = GetComponent<Rigidbody>();
        characterAnimator = GetComponent<Animator>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
    }

    protected virtual void Update()
    {
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotation.Value = transform.rotation;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, characterNetworkManager.networkPosition.Value, ref characterNetworkManager.networkPositionVelocity, characterNetworkManager.networkPositionSmoothTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value, characterNetworkManager.networkRotationSmoothTime);
        }
    }

    protected virtual void LateUpdate()
    {

    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if(IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;
        }

        if (!manuallySelectDeathAnimation)
        {
            characterAnimatorManager.PlayTargetActionAnimation("humanoid_death", true);
        }

        yield return new WaitForSeconds(5);
    }

    public virtual void ReviveCharacter()
    {

    }
}