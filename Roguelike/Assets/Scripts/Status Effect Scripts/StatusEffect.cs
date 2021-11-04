using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect : ScriptableObject {

    public enum effects {
        none,
        poison,
        slow,
        weak
    }

    // Hooks enums to Resources
    public static Material GetMaterial(effects effect) {
        switch (effect) {
            case effects.poison: return Resources.Load<Material>("Materials/StatusEffects/Poisoned");
            case effects.slow: return Resources.Load<Material>("Materials/StatusEffects/Slow");
            case effects.weak: return Resources.Load<Material>("Materials/StatusEffects/Weak");
            default: return Resources.Load<Material>("Default");
        }
    }

    public static Sprite GetSprite(effects effect) {
        switch (effect) {
            case effects.poison: return Resources.Load<Sprite>("Sprites/StatusEffects/Poisoned");
            case effects.slow: return Resources.Load<Sprite>("Sprites/StatusEffects/Slow");
            case effects.weak: return Resources.Load<Sprite>("Sprites/StatusEffects/Weak");
            default: return null;
        }
    }

    protected StatusEffectManager statusEffectManager;

    /* The Effectable's material is set using this property. */
    public abstract Material EffectMaterial { get; }
    public abstract int MaxStacks { get; }

    protected Effectable MyEffectable => statusEffectManager.myEffectable;

    public void Init(StatusEffectManager manager, float severity) {
        statusEffectManager = manager;
        OnInit(severity);
    }

    // Called after the method is added to the list.
    public abstract void OnInit(float severity);

    /* This method is called by StatusEffectManager after the status
     * effect's duration has completed. This is the cleanup behavior (removing
     * the material is not necessary). This is called before the effect is removed
     * from the list.
     */
    public virtual void OnFinish() {}

    /* This method is called by StatusEffectManager each frame for each StatusEffect
     * in its list of effects. This is the behavior that each status effect performs
     * each frame.
     */
    public virtual void OnUpdate() { }
}
