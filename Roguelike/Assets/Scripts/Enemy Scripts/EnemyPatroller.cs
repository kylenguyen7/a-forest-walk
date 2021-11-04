using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatroller : EnemyPathfinder {

    // A simple enemy that is always heading towards a target transform,
    // which its parent, a Seeker, moves towards, through a parent class,
    // EnemyPathfinder
    
    public float chaseSpeed = 4f;
    public float wanderSpeed = 2f;
    protected float currentSpeed = 0f;
    
    public Transform[] myWaypoints;
    private int waypointIndex = 0;

    public float idleTime = 3f;
    private float idleTimer;
    public float deaggroDistance = 7f;
    public float detectDistance = 1f;
    
    protected enum enemyPatrollerStates {
        idle,
        heading,
        alert
    }
    protected enemyPatrollerStates state = enemyPatrollerStates.idle;

    private new void Start() {
        base.Start();

        // Generate three random waypoints, if none exist
        if(myWaypoints.Length == 0) {
            float randRange = 3f;
            Transform[] waypoints = {
                Instantiate(Resources.Load("LevelGen/Waypoint", typeof(GameObject)) as GameObject,
                 (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-randRange, randRange), UnityEngine.Random.Range(-randRange, randRange)), Quaternion.identity).transform,
                Instantiate(Resources.Load("LevelGen/Waypoint", typeof(GameObject)) as GameObject,
                 (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-randRange, randRange), UnityEngine.Random.Range(-randRange, randRange)), Quaternion.identity).transform,
                Instantiate(Resources.Load("LevelGen/Waypoint", typeof(GameObject)) as GameObject,
                 (Vector2)transform.position + new Vector2(UnityEngine.Random.Range(-randRange, randRange), UnityEngine.Random.Range(-randRange, randRange)), Quaternion.identity).transform };

            myWaypoints = waypoints;
        }
    }

    private void OnDrawGizmos() {
        // Gizmos.DrawWireSphere(transform.position, detectDistance);
        // Gizmos.DrawWireSphere(transform.position, deaggroDistance);
    }

    protected override void OnUpdate() {
        // if taken damage, stay on alert, no longer checking LOS
        if (beenHit && state != enemyPatrollerStates.alert) {
            // createReaction(reactions.angry);
            InitiateAlert();
        }
        
        switch(state) {
            case enemyPatrollerStates.idle:     IdleBehavior(); break;
            case enemyPatrollerStates.heading:  HeadingBehavior(); break;
            case enemyPatrollerStates.alert:    AlertBehavior(); break;
        }

        mySpriteAnimator.SetBool("isRunning", isRunning);
    }

    public static bool HasSightToPlayer(Vector2 origin) {
        // Initiate alert if the enemy has LOS with the player
        Vector2 dir = ((Vector2)PlayerController.instance.transform.position - origin).normalized;
        LayerMask blocksLOS = LayerMask.GetMask("Player", "Rocks");
        return Physics2D.Raycast(origin, dir, 100f, blocksLOS).transform == PlayerController.instance.transform;
    }

    // Determines if player is in range and there is clear LOS between patroller and player
    private bool CheckInRange(float range) {
        if(Vector2.Distance(player.position, transform.position) < range) {
            return true;
            
            // Check LOS
            /*var ray = RaycastToPlayer();
            if(ray.transform == player) {
                return true;
            }*/
        }
        return false;
    }

    private void InitiateAlert() {
        squashAndStretch.SetTrigger("big squash");
        state = enemyPatrollerStates.alert;
        // addStatusEffect(statuses.alerted);
        HardRestartPath();
    }

    private void Deaggro() {
        squashAndStretch.SetTrigger("big squash");
        rb.velocity = knockback * (1 - slowModifier);
        state = enemyPatrollerStates.idle;
        // createReaction(reactions.confused);
    }

    float deaggroTimer = 0.25f;
    float deaggroTime = 0.25f;
    private void AlertBehavior() {
        target = player;
        currentSpeed = chaseSpeed;
        if (!CheckInRange(deaggroDistance) && !beenHit) {
            // only deaggro if passing the condition for a period of 0.25 seconds
            if(deaggroTimer <= 0) {
                Deaggro();
            }
            deaggroTimer -= Time.deltaTime;
        } else {
            deaggroTimer = deaggroTime;
        }
        isRunning = true;
    }


    private void InitiateHeading() {
        state = enemyPatrollerStates.heading;
        currentSpeed = wanderSpeed;
        squashAndStretch.SetTrigger("squash");

        var index = waypointIndex;
        while(waypointIndex == index)
            waypointIndex = UnityEngine.Random.Range(0, myWaypoints.Length);

        target = myWaypoints[waypointIndex];
        HardRestartPath();
    }

    private void HeadingBehavior() {
        if (CheckInRange(detectDistance)) InitiateAlert();
        isRunning = true;
        currentSpeed = wanderSpeed;

        if (Vector2.Distance(target.position, transform.position) < 0.8f) {
            state = enemyPatrollerStates.idle;
            rb.velocity = knockback * (1 - slowModifier);
        }
    }

    private void IdleBehavior() {
        if (CheckInRange(detectDistance)) InitiateAlert();
        isRunning = false;

        if(idleTimer <= 0) {
            InitiateHeading();
            idleTimer = idleTime + 0.2f * UnityEngine.Random.Range(-idleTime, idleTime);
        }
        idleTimer -= Time.deltaTime; 
    }

    protected override void OnFixedUpdate() {
        if(state != enemyPatrollerStates.idle) {
            FollowPath(currentSpeed);
        }
    }
}
