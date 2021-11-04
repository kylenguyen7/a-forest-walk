using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectPoisoned : StatusEffect {
    public override Material EffectMaterial => Resources.Load<Material>("Materials/StatusEffects/Poisoned");

    public override int MaxStacks => 10;

    float poisonDelay = 0.5f;
    float poisonTimer = 0.5f;
    float poisonDamage; // Poison DPS for duration

    public override void OnInit(float severity) {
        poisonDamage = severity;
    }

    public override void OnUpdate() {
        if(poisonTimer <= 0f) {
            MyEffectable.TakeDamage(poisonDamage * poisonDelay, Vector2.zero, 0);
            MyEffectable.PostNonprojectileDamage();

            Utility.CreateDamageText(poisonDamage * poisonDelay, MyEffectable.transform.position, false, effects.poison);
            poisonTimer = poisonDelay;
        }
        poisonTimer -= Time.deltaTime;
    }
}
