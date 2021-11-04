using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class WeaponControllerPlayer : WeaponController
{
    #region singleton

    public static WeaponControllerPlayer instance;
    private new void Awake() {
        base.Awake();
        if(instance != null) {
            Debug.LogError("More than one WeaponControllerPlayer found!");
            return;
        }
        instance = this;
    }

    #endregion

    public WeaponScriptableObject primaryWeapon;
    public WeaponScriptableObject offhandWeapon;

    int key;
    public bool holdingPrimary = true;
    int ammo;
    int maxAmmo;
    float ammoReloadTime = 1f;
    float ammoReloadTimer;

    float primaryCooldown = 0f;
    float secondaryCooldown = 0f;

    public int Ammo {
        get { return ammo; }
    }

    public float ReloadProgress {
        get { return Mathf.Min(1, ((ammoReloadTime / GameManager.AS_MODIFIER) - ammoReloadTimer) / (ammoReloadTime / GameManager.AS_MODIFIER)); }
    }


    private new void Start() {
        base.Start();
        maxAmmo = GameManager.MAX_AMMO;
        ammo = GameManager.MAX_AMMO;
        ammoReloadTimer = (ammoReloadTime / GameManager.AS_MODIFIER);
    }

    void Update() {
        if (!player.isAlive || Time.timeScale == 0 || !player.canAttack) return;

        // Switching between primary and offhand
        if (Input.GetMouseButtonDown(0) && primaryWeapon != null && myWeapon != primaryWeapon) {
            myWeapon = primaryWeapon;
            holdingPrimary = true;
            Reset();
            key = 0;
        }

        if (Input.GetMouseButtonDown(1) && offhandWeapon != null && myWeapon != offhandWeapon) {
            myWeapon = offhandWeapon;
            holdingPrimary = false;
            Reset();
            key = 1;
        }

        bool condition;
        if (myWeapon.autoFire) condition = Input.GetMouseButton(key);
        else condition = Input.GetMouseButtonDown(key);

        bool canAttack = holdingPrimary ? primaryCooldown < 0 : secondaryCooldown < 0;

        if(condition && canAttack) {
            Vector2 toMouse = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;

            if(!holdingPrimary) {
                if(ammo > 0) {
                    Attack(toMouse);
                    ammo -= 1;
                }
            } else {
                Attack(toMouse);
            }
        }
        primaryCooldown -= Time.deltaTime;
        secondaryCooldown -= Time.deltaTime;

        ReplenishAmmo();

        // Reset combo index if reset timer has elapsed
        if(comboResetTimer <= 0 && comboIndex > 0) {
            comboIndex = 0;
        }
        comboResetTimer -= Time.deltaTime;

        // Cleanup: updates sprite, if changed and handles rotation defects
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
            transform.Rotate(0, 0, 0);
        }
        spriteRenderer.sprite = myWeapon.sprite;

        if(player.lookingRight) spriteRenderer.flipX = transform.up.x < 0;
        else spriteRenderer.flipX = transform.up.x > 0;
    }

    protected override void ResetCooldown(AttackScriptableObject attack) {
        if(holdingPrimary) {
            primaryCooldown = CalculateCooldown(primaryWeapon.myAttacks[comboIndex], primaryWeapon.type);
        } else {
            secondaryCooldown = CalculateCooldown(offhandWeapon.myAttacks[comboIndex], offhandWeapon.type);
        }
    }

    private void ReplenishAmmo() {
        if(ammoReloadTimer <= 0 && ammo < maxAmmo) {
            ammo = Mathf.Min(ammo + 1, maxAmmo);
            ammoReloadTimer = ammoReloadTime / GameManager.AS_MODIFIER;
        }

        if(ammoReloadTimer > 0 && ammo < maxAmmo) {
            ammoReloadTimer = Mathf.Max(0, ammoReloadTimer - Time.deltaTime);
        }
    }

    protected override void PostAttack() {
        SimpleCameraShakeInCinemachine.Shake();
        animator.SetTrigger("attack");
        squashAndStretch.SetTrigger("squash");
        Vector2 toMouse = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
        player.AttackKnockback(-toMouse.normalized * myWeapon.myAttacks[comboIndex].knockbackSpeed);

        if(BuffManager.instance != null) {
            BuffManager.instance.OnAttack(transform.position, toMouse);
        }
    }
}
