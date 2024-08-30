using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterManager : NetworkBehaviour
{
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public Rigidbody characterRigidbody;
    [HideInInspector] public Animator characterAnimator;
    

    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool applyRootMotion = false;
    public bool canMove = true;
    public bool canRotate = true;
    public bool isJumping = false;
    public bool isGrounded = true;

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterRigidbody = GetComponent<Rigidbody>();
        characterAnimator = GetComponent<Animator>();
        
    }

    protected virtual void Start()
    {
        IgnoreSelfColliders();
    }

    protected virtual void Update()
    {
        characterAnimator.SetBool("isGrounded", isGrounded);
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
        if(characterNetworkManager.currentHealth.Value <= 0)
        {
            if (IsOwner)
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
    }

    public virtual void ReviveCharacter()
    {

    }

    protected virtual void IgnoreSelfColliders()
    {
        Collider locomotionCollider = GetComponent<Collider>();
        Collider[] damageableCharacterCollider = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new List<Collider>();

        foreach (var collider in damageableCharacterCollider)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(locomotionCollider);

        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);
            }
        }
    }
}