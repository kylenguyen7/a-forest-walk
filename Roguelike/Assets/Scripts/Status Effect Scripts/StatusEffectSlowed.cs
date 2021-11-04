using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectSlowed : StatusEffect
{
    public override Material EffectMaterial => Resources.Load<Material>("Materials/StatusEffects/Slow");

    public override int MaxStacks => 3;

    public override void OnInit(float severity) {
        MyEffectable.slowModifier = MyEffectable.slowModifier + 0.15f;
    }

    public override void OnFinish() {
        MyEffectable.slowModifier = Mathf.Max(MyEffectable.slowModifier - 0.15f, 0f);
    }
}
