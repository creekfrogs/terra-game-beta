using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotationSmoothTime = 0.1f;

    [Header("Animator")]
    public NetworkVariable<float> networkHorizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> networkVerticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> networkMoveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Target")]
    public NetworkVariable<ulong> currentTargetID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    public NetworkVariable<int> essence = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> force = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> finesse = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> focus = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> precision = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentKima = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentAdrenaline = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxKima = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxAdrenaline = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public void CheckHP(int oldValue, int newValue)
    {
        if(currentHealth.Value <= 0)
        {
            StartCoroutine(character.ProcessDeathEvent());
        }
        if(character.IsOwner)
        {
            if (currentHealth.Value > maxHealth.Value)
            {
                currentHealth.Value = maxHealth.Value;
            }
        }
    }

    public void OnLockOnTargetIDChange(ulong oldId, ulong newID)
    {
        if(!IsOwner)
        {
            character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
        }
    }

    public void OnIsLockedOnChanged(bool oldValue, bool isLockedOn)
    {
        if(!isLockedOn)
        {
            character.characterCombatManager.currentTarget = null;
        }
    }

    public void DebugResetStats()
    {
        essence.Value = 1;
        vitality.Value = 1;
        force.Value = 1;
        finesse.Value = 1;
        focus.Value = 1;
        precision.Value = 1;
        currentHealth.Value = 1;
    }

    [Rpc(SendTo.Server)]
    public void NotifyServerOfActionAnimationRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if(IsServer)
        {
            PlayActionAnimationForAllClientsRpc(clientID, animationID, applyRootMotion);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void PlayActionAnimationForAllClientsRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAnimationFromServer(animationID, applyRootMotion);
        }
    }

    private void PerformActionAnimationFromServer(string animationID, bool applyRootMotion)
    {
        character.applyRootMotion = applyRootMotion;
        character.characterAnimator.CrossFade(animationID, 0.2f);
    }

    [Rpc(SendTo.Server)]
    public void NotifyServerOfAttackActionAnimationRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (IsServer)
        {
            PlayActionAnimationForAllClientsRpc(clientID, animationID, applyRootMotion);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void PlayAttackActionAnimationForAllClientsRpc(ulong clientID, string animationID, bool applyRootMotion)
    {
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformAttackActionAnimationFromServer(animationID, applyRootMotion);
        }
    }

    private void PerformAttackActionAnimationFromServer(string animationID, bool applyRootMotion)
    {
        character.applyRootMotion = applyRootMotion;
        character.characterAnimator.CrossFade(animationID, 0.2f);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void NotifiyServerOfCharacterDamageRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        float physicalDamage,
        float kimaDamage,
        float fireDamage,
        float lightningDamage,
        float poiseDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ
        )
    {
        if(IsServer)
        {
            NotifiyClientOfCharacterDamageRpc(damagedCharacterID, characterCausingDamageID, physicalDamage, kimaDamage, fireDamage, lightningDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void NotifiyClientOfCharacterDamageRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        float physicalDamage,
        float kimaDamage,
        float fireDamage,
        float lightningDamage,
        float poiseDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ
        )
    {
        ProcessCharacterDamageFromServer(damagedCharacterID, characterCausingDamageID, physicalDamage, kimaDamage, fireDamage, lightningDamage, poiseDamage, angleHitFrom, contactPointX, contactPointY, contactPointZ);
    }

    public void ProcessCharacterDamageFromServer(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        float physicalDamage,
        float kimaDamage,
        float fireDamage,
        float lightningDamage,
        float poiseDamage,
        float angleHitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ
        )
    {
        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>();

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.kimaDamage = kimaDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.angleHitFrom = angleHitFrom;
        damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
        damageEffect.characterCausingDamage = characterCausingDamage;

        damagedCharacter.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }
}
