using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The most common variant of enemy, takes a reference to a WeaponControllerEnemy
 * since it... has a weapon, and doesn't just use its body.
 */
public abstract class EnemyRanged : Enemy
{
    public WeaponControllerEnemy myWeaponController;
}
