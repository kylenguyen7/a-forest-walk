using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponControllerEnemy : WeaponController
{
    public bool attacking = false;

    private new void Start() {
        base.Start();
        cooldownTimer = 1f + Random.Range(-myWeapon.myAttacks[0].cdVariance, myWeapon.myAttacks[0].cdVariance); // myWeapon.myAttacks[0].cooldown + Random.Range(-myWeapon.myAttacks[0].cdVariance, myWeapon.myAttacks[0].cdVariance);
    }

    protected override void PostAttack() {
        squashAndStretch.SetTrigger("giant squash");
    }

    // Update is called once per frame
    void Update()
    {
        if(attacking) {
            if(cooldownTimer <= 0) {
                Vector2 dir = (player.transform.position - transform.position).normalized;
                Attack(dir);
            }
            cooldownTimer -= Time.deltaTime;
        }
    }

    public void ManualAttack() {
        Vector2 dir = (player.transform.position - transform.position).normalized;
        Attack(dir);
    }
}
