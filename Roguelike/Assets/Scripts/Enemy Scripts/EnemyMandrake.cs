using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMandrake : EnemyPathfinder {

    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    // Tweakable stats
    float aggroRange = 5f;
    float explodeRange = 1.5f;
    float speed = 4f;
    bool uprooted = false;
    new float restartPathInterval = 0.25f;

    // Utility vars
    EnemyMandrake[] otherMandrakes;
    [SerializeField] WeaponScriptableObject rootedWeapon;
    [SerializeField] WeaponScriptableObject uprootedWeapon;

    [SerializeField] AudioClip sfxJump;
    [SerializeField] AudioClip sfxExplosion;
    [SerializeField] AudioClip sfxUproot;

    WeaponControllerEnemy myWeaponController;

    private new void Start() {
        base.Start();
        otherMandrakes = FindObjectsOfType<EnemyMandrake>();
        myWeaponController = GetComponentInChildren<WeaponControllerEnemy>();
        myWeaponController.myWeapon = rootedWeapon;
        myWeaponController.attacking = true;
    }

    protected override void OnFixedUpdate() {
        if(uprooted) {
            FollowPath(speed);
        }
    }

    protected override void OnUpdate() {
        float dist = Vector2.Distance(PlayerController.instance.transform.position, transform.position);

        if (dist < aggroRange) {
            OnAggro();
        }

        if(dist < explodeRange) {
            mySpriteAnimator.SetTrigger("explode");
            speed = 0;
        }

        // Start setting looking once OnAggro has been called
        if(alreadyCalled) {
            mySpriteAnimator.SetFloat("lookingHorizontal", (PlayerController.instance.transform.position.x - transform.position.x));
        }
    }

    // Mandrake behavior
    bool alreadyCalled = false;
    public void OnAggro() {
        if (alreadyCalled) return;

        alreadyCalled = true;
        mySpriteAnimator.SetTrigger("aggro");
        myAudioSource.PlayOneShot(sfxUproot);
    }

    // Called by an animation event
    public void Uproot() {
        uprooted = true;
        myWeaponController.ManualAttack();

        myWeaponController.myWeapon = uprootedWeapon;
        myWeaponController.attacking = false;
    }

    public void SelfDestruct() {
        var sfx = Instantiate(Resources.Load("Prefabs/Sound Effect") as GameObject, transform.position, Quaternion.identity);
        sfx.GetComponent<AudioSource>().PlayOneShot(sfxExplosion);

        myWeaponController.ManualAttack();
        OnDeath(false, false);
    }

    public void PlayJumpSFX() {
        myAudioSource.PlayOneShot(sfxJump);
    }

    // On death, aggro all other mandrakes
    public override void OnDeath(bool doDrop, bool doEffects) {
        foreach(EnemyMandrake mandrake in otherMandrakes) {
            if (mandrake == null) continue;
            if(Vector2.Distance(mandrake.transform.position, transform.position) < 20f) {
                mandrake.OnAggro();
            }
        }

        base.OnDeath(doDrop, doEffects);
    }
}
