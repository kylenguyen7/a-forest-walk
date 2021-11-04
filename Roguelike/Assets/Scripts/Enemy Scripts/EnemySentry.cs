using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySentry : EnemyRanged {
    public float range = 5f;
    private float deaggroTime = 0.5f;
    private float deaggroTimer;

    bool attackingPlayer = false;
    bool returningToStation = false;

    private Vector2 startPos;
    private float moveSpeed = 1f;

    private new void Start() {
        base.Start();
        startPos = transform.position;
        dampening = new Vector2(0.05f, 0.05f);
    }

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    protected override void OnUpdate() {

        bool playerInRange = Vector2.Distance(player.position, transform.position) < range;
        if(!attackingPlayer && playerInRange) {
            myWeaponController.attacking = true;
            // addStatusEffect(statuses.alerted);
            attackingPlayer = true;
        }

        if (playerInRange) deaggroTimer = deaggroTime;

        if(attackingPlayer && !playerInRange) {
            if(deaggroTimer <= 0) {
                myWeaponController.attacking = false;
                // createReaction(reactions.confused);
                attackingPlayer = false;
            }
            deaggroTimer -= Time.deltaTime;
        }

        mySpriteAnimator.SetBool("isRunning", rb.velocity.magnitude > 0.1f);
        updateLookingAnim();
    }

    public override void OnDeath(bool doDrop, bool doEffects) {
        // SimpleCameraShakeInCinemachine.Freeze(0.2f);
        SimpleCameraShakeInCinemachine.Shake(0.5f, 1f, 1f);

        base.OnDeath(doDrop, doEffects);
    }

    protected override void OnFixedUpdate() {
        float d = Vector2.Distance(transform.position, startPos);
        Vector2 desiredVelocity = Vector2.zero;

        if (d > 1f) returningToStation = true;
        
        if(returningToStation) {
            desiredVelocity = (startPos - (Vector2)transform.position).normalized * moveSpeed;
            if(d < 0.1f) returningToStation = false;
        }

        looking = desiredVelocity;
        rb.velocity = (desiredVelocity + knockback) * (1 - slowModifier);
    }
}