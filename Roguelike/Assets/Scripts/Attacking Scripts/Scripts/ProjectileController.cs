using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class ProjectileController : MonoBehaviour
{
    public Projectile myProjectile;
    // WeaponController is responsible for setting initial travelDirection
    // after first frame
    public Vector2 travelDirection;
    private float travelAngle;

    private float lifetimeRemaining;
    private float lifetime;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    public float slowFactor = 0f;

    public List<GameObject> enemiesInBounds;

    private GameObject myTrail;

    void Start()
    {
        lifetimeRemaining = myProjectile.lifetime;

        // Rigidbody reference and drag set
        rb = gameObject.GetComponent<Rigidbody2D>();
        rb.drag = myProjectile.drag;

        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        // Sprite reference and set
        if (myProjectile.optionalAnimator != null) {
            var animator = gameObject.GetComponent<Animator>();
            animator.runtimeAnimatorController = myProjectile.optionalAnimator;
        } else if (myProjectile.sprite != null) {
            spriteRenderer.sprite = myProjectile.sprite;
        }
        spriteRenderer.material = StatusEffect.GetMaterial(myProjectile.statusEffectInfo.effect);

        gameObject.AddComponent<PolygonCollider2D>().isTrigger = true;

        // For ProjectileControllers like the kunai, which are created by PlayerController.Deploy,
        // and not WeaponController.CreateProjectile
        if(travelDirection == null) {
            travelDirection = ((Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) - (Vector2)transform.position).normalized;
            Debug.Log("Used default setting");
        }

        // Initial velocity set (this happens after it is set by WeaponController?)
        rb.velocity = travelDirection.normalized * myProjectile.speed * (1 - slowFactor);
        travelAngle = Mathf.Atan(travelDirection.y / travelDirection.x);
        if(travelDirection.x < 0) {
            travelAngle += Mathf.PI;
        }

        // Initial scale set
        transform.localScale = new Vector3(1, 1, 1) * myProjectile.size;
        if (rb.velocity.x < 0) {
            spriteRenderer.flipY = true;
        }

        if(myProjectile.glow) {
            Instantiate(Resources.Load("Prefabs/Glow", typeof(GameObject)) as GameObject,
                transform.position, Quaternion.identity).transform.parent = gameObject.transform;
        }

        if(myProjectile.trail) {
            myTrail = Instantiate(Resources.Load<GameObject>("Prefabs/Projectile Trail"), transform, false);
        }

        // If homing, add homing zone
        if(myProjectile.myPath == Projectile.paths.homing) {
            GameObject searchRange = null;

            switch(myProjectile.optionalHomingSearch) {
                case Projectile.searchRange.circle: {
                        searchRange = Resources.Load<GameObject>("Prefabs/Homing Circle");
                        break;
                    }
                case Projectile.searchRange.cone: {
                        searchRange = Resources.Load<GameObject>("Prefabs/Homing Cone");
                        break;
                    }
            }

            Instantiate(searchRange, transform, false);
        }

        // StartCoroutine(DisableTrail());
    }
    
    /*IEnumerator DisableTrail() {
        yield return new WaitForSeconds(0.25f);
        var trail = GetComponent<TrailRenderer>();
        if (trail != null) trail.emitting = false;
    }*/

    // Update is called once per frame
    void Update()
    {
        // move based on myPath
        switch(myProjectile.myPath) {
            case Projectile.paths.straight: MoveStraight(); break;
            case Projectile.paths.sine: MoveSine(); break;
            case Projectile.paths.cosine: MoveCosine(); break;
            case Projectile.paths.homing: MoveHoming(); break;
        }
        transform.right = travelDirection;

        // end of lifetime
        if(lifetime >= lifetimeRemaining) {
            MyDestroy();
        }
        lifetime += Time.deltaTime;
    }

    public void MyDestroy() {
        GameObject sparkle = Resources.Load("Prefabs/Sparkle") as GameObject;
        Instantiate(sparkle, transform.position, Quaternion.identity);

        /*GameObject poof = Resources.Load("Prefabs/Poof") as GameObject;
        Instantiate(poof, transform.position, Quaternion.Euler(-90, 0, 0));*/

        if(myTrail != null) {
            myTrail.transform.parent = null;
            myTrail.transform.localScale = transform.localScale;
            myTrail.GetComponent<ParticleSystem>().Stop();
        }

        Destroy(gameObject);
    }

    private void MoveStraight() {
        // no modification to travelDirection
        // velocity is never reset to allow drag to slow projectile
    }

    // max variance of angle from amplitude
    private float amplitude = Mathf.PI / 6f;
    // second to complete one oscillation
    private float period = 0.6f;
    private void MoveSine() {
        float currentAngle = travelAngle + amplitude * Mathf.Sin((lifetime / period) * Mathf.PI * 2);
        travelDirection = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)).normalized;

        rb.velocity = travelDirection * myProjectile.speed * (1 - slowFactor);
    }

    private void MoveCosine() {
        float currentAngle = travelAngle + amplitude * Mathf.Cos((lifetime / period) * Mathf.PI * 2);
        travelDirection = new Vector2(Mathf.Cos(currentAngle), Mathf.Sin(currentAngle)).normalized;

        rb.velocity = travelDirection * myProjectile.speed * (1 - slowFactor);
    }

    private void MoveHoming() {
        // If enemies found, adjust travel direction
        if (enemiesInBounds.Count != 0) {
            AdjustTravelDirHoming();
        }
        
        rb.velocity = travelDirection * myProjectile.speed * (1 - slowFactor);
    }

    private float scale = 8f;
    private void AdjustTravelDirHoming() {
        if (enemiesInBounds[0] == null) return;

        var target = enemiesInBounds[0].transform.position;

        // Rotate amount is a value on [-1, 1]
        // -1 indicates the target direction is directly counterclockwise
        // 1 indicates the target direction is directly clockwise
        Vector2 targetDirection = ((Vector2)target - rb.position);
        float rotateAmount = Vector3.Cross(targetDirection.normalized, travelDirection.normalized).z;

        // Angle is calculated from travelDirection and incremented by deltaAngle,
        // which is an artificial scale of rotateAmount
        float deltaAngle = -rotateAmount * scale;

        float currentAngle = Vector2.SignedAngle(new Vector2(1, 0), travelDirection);
        if(currentAngle < 0) {
            currentAngle = currentAngle + 360;
        }

        float targetAngle = currentAngle + deltaAngle;

        // Resulting angle is converted back to travel direction
        travelDirection = new Vector2(Mathf.Cos(targetAngle * Mathf.Deg2Rad),
                                      Mathf.Sin(targetAngle * Mathf.Deg2Rad)).normalized;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(myProjectile.collisionTags.Contains(collision.gameObject.tag)) {

            var target = collision.gameObject.GetComponent<Effectable>();
            float damage = myProjectile.damage + Random.Range(-myProjectile.damageVariance, myProjectile.damageVariance);
            if (target != null) {
                bool crit = false;
                if(Random.value < myProjectile.critChance + target.critModifier) {
                    damage *= 2f;
                    crit = true;
                }

                target.TakeDamage(damage, travelDirection, myProjectile.knockback);
                target.PostProjectileDamage(myProjectile.doHitEffects);
                Utility.CreateDamageText(damage, target.transform.position, crit, StatusEffect.effects.none);

                if (myProjectile.statusEffectInfo.effect != StatusEffect.effects.none) {
                    var info = myProjectile.statusEffectInfo;
                    target.AddStatusEffect(info.effect, info.severity, info.duration);
                }

                var sfx = Instantiate(Resources.Load("Prefabs/Sound Effect") as GameObject, transform.position, Quaternion.identity);
                sfx.GetComponent<AudioSource>().PlayOneShot(myProjectile.impactSfx);

                if(target.IsEnemy) {
                    var enemy = target.gameObject.GetComponent<Enemy>();
                    BuffManager.instance.OnHit(transform.position, travelDirection, enemy, damage);
                }
            }

            if(!myProjectile.isPiercing) MyDestroy();
        }
    }


}
