using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This class handles enemy movement behavior, taking damage (including
 * dying and dropping loot) and animations for those actions. It also handles
 * attacking behavior logic with a public reference to a WeaponControllerEnemy
 * whose Attack() method can be invoked according to its cooldownTimer.
 */
public abstract class Enemy : Effectable
{
    public enum statuses {
        alerted,
        poisoned,
        NOT_FOUND // special enum for searching statusEffect when trying to add duplicate or remove
    }

    public List<(statuses, Coroutine)> statusEffects;

    public override bool IsEnemy => true;

    public float hp;
    float maxHp;
    public Animator squashAndStretch;
    public Animator mySpriteAnimator;

    protected AudioSource myAudioSource;

    protected Transform player;

    protected bool isRunning;
    protected Vector2 looking;

    protected bool beenHit;

    protected Material matWhite;

    protected GameObject damagedParticlesPrefab;
    protected GameObject deathParticlesPrefab;

    protected EnemyHPBarGroupController myHPBar;

    // One day... I will explore serializable dictionaries
    // public Dictionary<LootScriptableObject, int> myLootInfo;

    // That day is today! (8/11/2020)
    public LootDropInfo[] dropInfo;

    protected AudioClip deathSfx;

    [HideInInspector]
    public RoomManager myRoomManager;
    protected Rigidbody2D rb;

    protected void Start() {
        matWhite = Resources.Load<Material>("WhiteFlash");

        damagedParticlesPrefab = Resources.Load("Prefabs/Damaged", typeof(GameObject)) as GameObject;
        deathParticlesPrefab = Resources.Load("Prefabs/Dead", typeof(GameObject)) as GameObject;
        deathSfx = Resources.Load("Sfx/Death", typeof(AudioClip)) as AudioClip;

        player = PlayerController.instance.transform;
        statusEffects = new List<(statuses, Coroutine)>();

        // Spawn whiteflash
        mySprite.material = matWhite;
        Invoke("ResetMaterial", 0.1f);

        myAudioSource = GetComponent<AudioSource>();

        var roomCollider = Physics2D.OverlapPoint(transform.position, LayerMask.GetMask("Room Bounds"));
        if (roomCollider == null) {
            Debug.LogError($"Enemy {gameObject.name} was unable to find a RoomManager object.");
            return;
        }

        myRoomManager = roomCollider.gameObject.GetComponent<RoomManager>();
        myRoomManager.myEnemies.Add(this);

        maxHp = hp;

        // Add HP bar, with stauts effects cosmetics
        Vector2 hpBarOffset = new Vector2(-0.35f, 1f);
        var bar = Resources.Load("Prefabs/HP Bar (Enemy)", typeof(GameObject)) as GameObject;
        myHPBar = Instantiate(bar, transform).GetComponent<EnemyHPBarGroupController>();
        myHPBar.Inititalize(hpBarOffset, transform);

        // Query status effect manager to find new cosmetic manager
        myStatusEffectManager.FindCosmeticsManager();

        rb = GetComponent<Rigidbody2D>();
    }

    // Used in tandem with Vector2 looking, to be called
    // whenever an enemy should update its looking animation
    protected void updateLookingAnim() {
        mySpriteAnimator.SetFloat("lookingHorizontal", looking.normalized.x);
        mySpriteAnimator.SetFloat("lookingVertical", looking.normalized.y);
    }

    protected void Update() {
        OnUpdate();
    }

    protected void FixedUpdate() {
        OnFixedUpdate();
        DampenKnockback();
    }

    public override void TakeDamage(float damage, Vector2 forceDirection, float kb) {
        hp -= damage;
        OnTakeDamage();
        if (hp <= 0) {
            OnDeath(true, true);
        }

        // Update HP bar
        myHPBar.UpdateHPBar(hp / maxHp);

        knockback += forceDirection * kb;
        beenHit = true;
    }

    public override void PostProjectileDamage(bool doHitEffects) {
        base.PostProjectileDamage(doHitEffects);
        if (!doHitEffects) return;

        Instantiate(damagedParticlesPrefab, transform.position, Quaternion.Euler(-90, 0, 0));
        squashAndStretch.SetTrigger("big squash");
        mySprite.material = matWhite;
        Invoke("ResetMaterial", 0.1f);
    }

    public override void PostNonprojectileDamage() {
        base.PostNonprojectileDamage();
        squashAndStretch.SetTrigger("squash");
        // mySprite.material = matWhite;
        // Invoke("ResetMaterial", 0.05f);
    }

    protected virtual void OnTakeDamage() {}

    bool droppedLoot = false;
    protected void DropLoot() {
        if (droppedLoot) return;

        LootUtility.DropLoot(dropInfo, transform.position);
        droppedLoot = true;
    }

    public virtual void OnDeath(bool doDrop, bool doEffects) {
        BuffManager.instance.OnKill(transform.position, this);

        if (doDrop) DropLoot();

        if (doEffects) {
            Instantiate(deathParticlesPrefab, transform.position, Quaternion.identity);
            var sfx = Instantiate(Resources.Load("Prefabs/Sound Effect") as GameObject, transform.position, Quaternion.identity);
            sfx.GetComponent<AudioSource>().PlayOneShot(deathSfx);
        }

        Destroy(gameObject);
    }

    private void OnDestroy() {
        if (myRoomManager == null) return;
        myRoomManager.myEnemies.Remove(this);
    }

    protected abstract void OnUpdate();

    protected abstract void OnFixedUpdate();

    protected enum reactions {
        confused,
        angry
    }

    protected void createReaction(reactions reaction) {
        GameObject reactionPrefab = null;
        switch(reaction) {
            case reactions.confused: reactionPrefab = Resources.Load("Prefabs/Reactions/Confused", typeof(GameObject)) as GameObject; break;
            case reactions.angry: reactionPrefab = Resources.Load("Prefabs/Reactions/Angry", typeof(GameObject)) as GameObject; break;
        }

        if(reactionPrefab != null) {
            var obj = Instantiate(reactionPrefab, transform);
            obj.transform.localPosition = new Vector2(0, 0.75f);
        }
    }


    // Basics:
    // You need to call addStatusEffect(statusEffect, time) to add or refresh a status effect
    // If you pass time = 0, you need to call removeStatusEffectManual() to remove a status effect
    // Otherwise, it will remove itself after time
    // A new call to addStatusEffect will replace all memory of an old one; i.e., timers will be reset
    // or stopped completely in the case of time = 0.

    /* Example use:
     * if(Input.GetKeyDown(KeyCode.Alpha1)) {
            addStatusEffect(statuses.alerted, 0f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2)) {
            addStatusEffect(statuses.alerted, 1f);
        }
     


    #region // STATUS EFFECT CODE

    // Add the passed status effect, and remove it after time seconds. If a non-positive
    // time is passed, the status must be removed manually with removeStatusEffectManual
    protected void addStatusEffect(statuses statusEffect, float time = 0f) {
        // Start coroutine to delete status effect
        Coroutine timer;
        if(time > 0) {
            timer = StartCoroutine(removeStatusEffectWithTimer(statusEffect, time));
        } else {
            timer = null;
        }

        // if the effect was previously added, stop its coroutine and replace it with a new one
        var previouslyAdded = findStatusTuplebyStatus(statusEffect);
        if(previouslyAdded != (statuses.NOT_FOUND, null)) {
            // Debug.Log("replacing previously added status effect");
            int index = statusEffects.IndexOf(previouslyAdded);

            // Stop previous coroutine, if it was counting down
            Coroutine previouslyAddedCoroutine = statusEffects[index].Item2;
            if(previouslyAddedCoroutine != null) {
                // Debug.Log("and replacing its coroutine");
                StopCoroutine(previouslyAddedCoroutine);
            }
            // Add replace old tuple
            statusEffects[index] = (statusEffect, timer);
        } else { // otherwise, add it with the new coroutine
            // Debug.Log("adding new status effect");
            statusEffects.Add((statusEffect, timer));
        }
        
        GetComponentInChildren<StatusEffectsRenderer>().UpdateStatusEffects();
    }

    protected void removeStatusEffect(statuses toRemove) {
        var previouslyAdded = findStatusTuplebyStatus(toRemove);
        statusEffects.Remove(previouslyAdded);
        GetComponentInChildren<StatusEffectsRenderer>().UpdateStatusEffects();
    }

    protected IEnumerator removeStatusEffectWithTimer(statuses toRemove, float time) {
        yield return new WaitForSeconds(time);
        removeStatusEffect(toRemove);
    }

    private (statuses, Coroutine) findStatusTuplebyStatus(statuses status) {
        foreach((statuses, Coroutine) tuple in statusEffects) {
            if(tuple.Item1 == status) {
                return tuple;
            }
        }

        return (statuses.NOT_FOUND, null);
    }
    #endregion

    */
}
