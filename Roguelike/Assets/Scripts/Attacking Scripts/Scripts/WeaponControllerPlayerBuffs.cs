using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponController {
    private float slowDuration = 3f;
    private float slowSeverity = 5f;

    private float poisonDuration = 5f;
    private float poisonSeverity = 5f;

    public Projectile ModifyProjectile(Projectile proj, WeaponScriptableObject.weaponType type) {
        if (type == WeaponScriptableObject.weaponType.none) return proj;
        
        if (type == WeaponScriptableObject.weaponType.sword) {
            ModifySize(proj);
            ModifyDamage(proj);
            MakeSlow(proj);
        } else if(type == WeaponScriptableObject.weaponType.bow) {
            MakePoisonous(proj);
        }

        ModifyCrit(proj);
        return proj;
    }

    public Projectile MakeSlow(Projectile proj) {
        if(Random.value < GameManager.SWORD_SLOW_CHANCE) {
            proj.statusEffectInfo.effect = StatusEffect.effects.slow;
            proj.statusEffectInfo.duration = slowDuration;
            proj.statusEffectInfo.severity = slowSeverity;
        }

        return proj;
    }

    public Projectile MakePoisonous(Projectile proj) {
        if (Random.value < GameManager.BOW_POISON_CHANCE) {
            proj.statusEffectInfo.effect = StatusEffect.effects.poison;
            proj.statusEffectInfo.duration = poisonDuration;
            proj.statusEffectInfo.severity = poisonSeverity;
        }

        return proj;
    }

    public Projectile ModifySize(Projectile proj) {
        proj.size *= GameManager.SIZE_MODIFIER;
        return proj;
    }

    public Projectile ModifyDamage(Projectile proj) {
        proj.damage += GameManager.DAMAGE_MODIFIER;
        return proj;
    }

    public Projectile ModifyCrit(Projectile proj) {
        proj.critChance += GameManager.CRIT_MODIFIER;
        return proj;
    }

    public float CalculateCooldown(AttackScriptableObject attack, WeaponScriptableObject.weaponType type) {
        float result = attack.cooldown + Random.Range(-attack.cdVariance, attack.cdVariance);

        if (type == WeaponScriptableObject.weaponType.bow) result *= 1 / GameManager.AS_MODIFIER;
        return result;
    }
}
