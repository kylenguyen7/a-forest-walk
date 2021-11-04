using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlime : EnemyPatroller {
    private float pauseTime = 1.5f;
    private Vector2 setVelocity = Vector2.zero;

    // If the slime is in the middle of jumping (no new jump can be initiated)
    bool canInitiateJump = true;

    // If the slime can move (this is in the middle of the jump sequence)
    [HideInInspector] public bool canMove = false;

    protected override void OnFixedUpdate() {
        if(state != enemyPatrollerStates.idle) {
            AttemptFollowPath();
        }
    }

    private void AttemptFollowPath() {
        if(canInitiateJump) {
            mySpriteAnimator.SetTrigger("jump");
            canInitiateJump = false;
            Invoke("ResetJump", Random.Range(0.8f*pauseTime, 1.2f*pauseTime));
        }

        isRunning = canMove;
        if (canMove) {
            FollowPath(currentSpeed);
        } else {
            rb.velocity = setVelocity + knockback;
        }
    }

    #region animation events
    // Trigger events called by EnemySlimeTimer, which is attached
    // to the Animator
    public void OnTakeoff() {
        canMove = true;
    }

    public void OnLanding() {
        canMove = false;
        setVelocity = rb.velocity * 0.5f;
        Invoke("PostLanding", 0.25f);
    }

    // Upon landing, coming to a full stop takes some time
    private void PostLanding() {
        if(!canMove) {
            setVelocity = Vector2.zero;
        }
    }

    public void PreTakeoff() {
        FollowPath(currentSpeed);
        setVelocity = rb.velocity * 0.5f;
    }
    #endregion

    void ResetJump() {
        // Debug.Log("Reseting jump.");
        canInitiateJump = true;
    }
}
