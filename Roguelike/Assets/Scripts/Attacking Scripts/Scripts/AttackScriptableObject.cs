using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Attack_DESC", menuName = "ScriptableObjects/AttackScriptableObject", order = 2)]
public class AttackScriptableObject : ScriptableObject
{
    public Projectile[] myProjectiles;

    public float cooldown;
    public float cdVariance;
    public float spread;
    public float[] angles; // only used if spread == 0 and angles is non-empty
    public float offsetDistance;

    public float knockbackSpeed;
    public AudioClip sfx;
    public bool lockAim = false;
}
