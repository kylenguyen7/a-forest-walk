using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Projectile_DESC", menuName = "ScriptableObjects/ProjectileScriptableObject", order = 3)]
public class Projectile : ScriptableObject {
    public float lifetime;
    public float speed;
    public float drag;
    public Sprite sprite;
    public AnimatorOverrideController optionalAnimator;
    public float damage;
    public float damageVariance;
    public float critChance = 0.05f;
    public float knockback;
    public float size;
    public bool isPiercing;
    public float spread;

    public bool trail = false;
    public bool glow = false;
    public bool doHitEffects = true;

    public AudioClip impactSfx;
    public string[] collisionTags;

    public enum paths {
        straight,
        sine,
        cosine,
        spirals,
        homing
    }
    public paths myPath;

    public enum searchRange {
        cone,
        circle
    }
    public searchRange optionalHomingSearch;

    [System.Serializable]
    public struct StatusEffectInfo {
        public StatusEffect.effects effect;
        public float severity;
        public float duration;
    }
    public StatusEffectInfo statusEffectInfo;
    
}
