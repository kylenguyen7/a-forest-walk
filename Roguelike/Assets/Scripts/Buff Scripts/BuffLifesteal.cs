using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffLifesteal : Buff {
    GameObject lifestealOrb;
    float chance = 1f;

    float damagePerOrb = 50f;
    float damageAccumulated = 0f;
    
    public override void Init() {
        lifestealOrb = Resources.Load<GameObject>("Buffs/Lifesteal Orb");
    }

    public override void OnHit(Vector2 position, Vector2 direction, Enemy enemy, float damage) {
        damageAccumulated += damage;

        if(damageAccumulated > damagePerOrb) {
            Instantiate(lifestealOrb, position, Quaternion.identity);
            damageAccumulated -= damagePerOrb;
        }
    }
}
