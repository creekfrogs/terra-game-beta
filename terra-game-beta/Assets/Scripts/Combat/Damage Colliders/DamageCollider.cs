using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;

    [Header("Damage")]
    public float physicalDamage = 0; // Will be split into "Standard", "Strike", "Slash", "Pierce"
    public float kimaDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    private int finalDamage;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damaged")]
    [SerializeField] protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    protected virtual void Awake()
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        if(damageTarget != null)
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            DamageTarget(damageTarget);
        }
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
            return;

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.kimaDamage = kimaDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.lightningDamage = fireDamage;
        damageEffect.contactPoint = contactPoint;

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear();
    }
}
