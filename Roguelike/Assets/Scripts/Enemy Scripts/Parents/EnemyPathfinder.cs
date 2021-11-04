using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public abstract class EnemyPathfinder : Enemy
{
    // This class's main function is the FollowPath() which sets its
    // Rigidbody2D to follow a path towards a target transform.
    // HardRestartPath() should be called when the path should be recalculated.
    // The most common use case is when transform has changed.

    // Transform of current target, defaults to player
    protected Transform target;

    // Other path variables
    Path path;
    int waypointIndex;
    bool reachedEndPath;
    Seeker seeker;
    private float nextWaypointDistance = 0.7f;
    protected float restartPathInterval = 0.5f;

    public new void Start() {
        base.Start();
        seeker = GetComponentInParent<Seeker>();
        target = player.transform;

        InvokeRepeating("RestartPath", Random.Range(0, restartPathInterval), restartPathInterval);
    }

    // recalculates path and stores new path in p
    protected void RestartPath() {
        if (seeker.IsDone()) {
            seeker.StartPath(rb.position, target.position, OnPathComplete);
        }
    }

    protected void HardRestartPath() {
        seeker.StartPath(rb.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p) {
        if (!p.error) {
            path = p;
            waypointIndex = 0;
        }
    }

    private bool HasLOStoPlayer() {
        // Initiate alert if the enemy has LOS with the player
        Vector2 dir = (player.position - transform.position).normalized;
        LayerMask blocksLOS = LayerMask.GetMask("Player", "Rocks");
        return Physics2D.Raycast(transform.position, dir, 100f, blocksLOS).transform == PlayerController.instance.transform;
    }

    // Sets velocity towards next point on path
    // Once reaching a point, advances and sets velocity to new point
    bool pathfinding = false;
    protected void FollowPath(float spd) {
        if (path == null) return;

        Vector2 calculatedVelocity;

        if (pathfinding) {
            // Check if a waypoint has been reached, if so, attempt to advance to next waypoint
            bool reachedWaypoint = Vector2.Distance(path.vectorPath[waypointIndex], transform.position) < nextWaypointDistance;

            if (reachedWaypoint) {
                waypointIndex = Mathf.Min(waypointIndex + 1, path.vectorPath.Count - 1);
            }

            // If the end of the path has been reached, return
            if (waypointIndex == path.vectorPath.Count - 1) return;

            Vector2 direction = ((Vector2)path.vectorPath[waypointIndex] - rb.position).normalized;
            calculatedVelocity = direction * spd;

            // If a waypoint was reached this frame, reset looking towards next waypoint
            if (reachedWaypoint) {
                looking = direction;
                updateLookingAnim();
            }

            if (HasLOStoPlayer()) {
                pathfinding = false;
            }
        } else {
            Vector2 direction = (PlayerController.instance.transform.position - transform.position).normalized;
            calculatedVelocity = direction * spd;
            looking = direction;
            updateLookingAnim();

            if (!HasLOStoPlayer()) {
                pathfinding = true;
                HardRestartPath();
            }
        }

        rb.velocity = (Vector2.Lerp(rb.velocity, calculatedVelocity, 0.25f) + knockback) * (1 - slowModifier);
    }

    public override void OnDeath(bool doDrop, bool doEffects) {
        // Destroy pathfinder
        Destroy(GetComponentInParent<Seeker>().gameObject);

        base.OnDeath(doDrop, doEffects);
    }
}
