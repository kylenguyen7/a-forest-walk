using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffMagicMissile : Buff
{
    float chance = 0.1f;
    private Projectile magicMissile;

    public override void Init() {
        magicMissile = Resources.Load<Projectile>("Buffs/Projectile_MagicMissile");
    }

    public override void OnAttack(Vector2 position, Vector2 direction) {
        if(magicMissile == null) {
            return;
        }

        if(Random.value < chance) {
            var projectile = WeaponControllerPlayer.CreateProjectile(PlayerController.instance.gameObject, direction, magicMissile,
                PlayerController.instance.slowModifier);
        }
    }
}
