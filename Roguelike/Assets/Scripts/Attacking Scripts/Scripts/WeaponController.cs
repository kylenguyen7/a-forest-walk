using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract partial class WeaponController : MonoBehaviour
{
    protected Animator animator;
    public Animator squashAndStretch;
    protected PlayerController player;
    protected SpriteRenderer spriteRenderer;
    protected AudioSource audioSource;

    protected Effectable myEffectable;

    public WeaponScriptableObject myWeapon;

    protected float cooldownTimer = 0f;
    protected float comboResetTimer = 0f;
    protected int comboIndex = 0;

    protected void Awake() {
        myEffectable = GetComponentInParent<Effectable>();
        if(myEffectable == null) {
            Debug.LogError("A WeaponController can't find its Effectable!");
        }
    }

    protected void Start() {
        animator = gameObject.GetComponent<Animator>();
        player = FindObjectOfType<PlayerController>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = myWeapon.sprite;
        audioSource = GetComponent<AudioSource>();

        transform.localScale *= myWeapon.size;
    }

    // private float extraPitch = 0f;
    public void Attack(Vector2 direction) {
        if (myWeapon.myAttacks.Length == 0) return;

        // Get next attack in weapon's queue
        AttackScriptableObject currentAttack = myWeapon.myAttacks[comboIndex];
        float baseAngle;
        if (currentAttack.lockAim) {
            baseAngle = 0;
        } else {
            baseAngle = Vector2.SignedAngle(Vector2.right, direction);
        }


        // create projectiles
        for (int i = 0; i < currentAttack.myProjectiles.Length; i++) {

            // Get angle
            float angle;
            if (i < currentAttack.angles.Length) {
                angle = baseAngle + currentAttack.angles[i];
            } else {
                int angleIndex = i - currentAttack.angles.Length;
                // Gets most negative angle for the projectiles not determined by myWeapon.angles
                float minAngle = baseAngle + -currentAttack.spread * (currentAttack.myProjectiles.Length - currentAttack.angles.Length - 1) / 2f;
                angle = minAngle + currentAttack.spread * angleIndex;
            }

            Vector2 dir = new Vector2(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle)).normalized;
            // Debug.Log(myEffectable);

            Projectile toCreate = Instantiate(currentAttack.myProjectiles[i]);
            toCreate = ModifyProjectile(toCreate, myWeapon.type);
            CreateProjectile(gameObject, dir, toCreate, myEffectable.slowModifier);
        }


        // play sound effect
        audioSource.PlayOneShot(currentAttack.sfx);

        // LOL experimental pitch change
        // audioSource.pitch = 0.8f + extraPitch;
        // extraPitch = (extraPitch + 0.05f) % 0.30f;

        // set cooldown timer and refresh combo reset timer
        ResetCooldown(currentAttack);
        comboResetTimer = myWeapon.comboResetTime;
        comboIndex = (comboIndex + 1) % myWeapon.myAttacks.Length;

        PostAttack();
    }

    protected virtual void ResetCooldown(AttackScriptableObject attack) {
        cooldownTimer = CalculateCooldown(attack, myWeapon.type);
    }

    // Handles object-specific reactions to attacking, such as
    // setting animations, setting states, setting knockback, etc.
    protected abstract void PostAttack();

    public void Reset() {
        comboIndex = 0;
        cooldownTimer = 0f;
        spriteRenderer.sprite = myWeapon.sprite;
        transform.localScale = new Vector3(myWeapon.size, myWeapon.size, myWeapon.size);
    }

    public static ProjectileController CreateProjectile(GameObject creator, Vector2 direction, Projectile toCreate, float slowFactor) {
        // create new base projectile, setting its direction and passing projectile S.O.
        var baseProjectilePrefab = Resources.Load("Prefabs/BaseProjectile", typeof(GameObject)) as GameObject;
        var projectile = Instantiate(baseProjectilePrefab, creator.transform.position, Quaternion.identity).GetComponent<ProjectileController>();
        
        // Add spread (in post)
        if(toCreate.spread > 0) {
            float angle = Vector2.SignedAngle(new Vector2(1, 0), direction);
            angle += Random.Range(-toCreate.spread / 2, toCreate.spread / 2);

            direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                    Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;
        }

        projectile.travelDirection = direction;
        projectile.myProjectile = toCreate;
        projectile.slowFactor = slowFactor;

        return projectile;
    }
}
