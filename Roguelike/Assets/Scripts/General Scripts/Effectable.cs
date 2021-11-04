using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatusEffectManager))]
public abstract class Effectable : MonoBehaviour
{
    // Determines if certain events are triggered such as OnHit buffs
    public abstract bool IsEnemy { get; }

    public abstract void TakeDamage(float damage, Vector2 forceDirection, float kb);

    public virtual void PostNonprojectileDamage() { }

    // doEffects is for whiteflash and particles
    public virtual void PostProjectileDamage(bool doEffects) { }


    // 0 is no slow, 1 is frozen (speed is multiplied by 1 - slowFactor)
    public float slowModifier = 0;

    public float critModifier = 0;

    public SpriteRenderer mySprite;

    protected StatusEffectManager myStatusEffectManager;

    public void Awake() {
        myStatusEffectManager = GetComponent<StatusEffectManager>();
    }

    protected void ResetMaterial() {
        mySprite.material = myStatusEffectManager.GetCurrentMat();
    }

    public void AddStatusEffect(StatusEffect.effects effect, float severity, float duration) {
        myStatusEffectManager.AddStatusEffect(effect, severity, duration);

    }

    public SECosmeticsManager statusEffectsCosmetics;

    protected Vector2 knockback = Vector2.zero;
    protected Vector2 dampening = new Vector2(0.1f, 0.1f);
    protected void DampenKnockback() {
        if (knockback.magnitude > 3f) {
            knockback = knockback.normalized * 3f;
        }
        var newKnockback = knockback;

        newKnockback.x += -Mathf.Sign(newKnockback.x) * dampening.x;
        if (Mathf.Sign(newKnockback.x) != Mathf.Sign(knockback.x)) {
            newKnockback.x = 0;
        }

        newKnockback.y += -Mathf.Sign(newKnockback.y) * dampening.y;
        if (Mathf.Sign(newKnockback.y) != Mathf.Sign(knockback.y)) {
            newKnockback.y = 0;
        }

        knockback = newKnockback;
    }
}
