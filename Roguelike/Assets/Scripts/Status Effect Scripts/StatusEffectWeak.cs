using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectWeak : StatusEffect {
    float myCritChance = 0.5f;
    public override Material EffectMaterial => Resources.Load<Material>("Materials/StatusEffects/Weak");

    public override int MaxStacks => 1;

    public override void OnInit(float severity) {
        MyEffectable.critModifier += myCritChance;
    }

    public override void OnFinish() {
        MyEffectable.critModifier = Mathf.Max(MyEffectable.critModifier -= myCritChance, 0);
    }
}
