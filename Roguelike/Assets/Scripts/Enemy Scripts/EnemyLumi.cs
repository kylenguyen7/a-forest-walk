using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The most common variant of enemy, takes a reference to a WeaponControllerEnemy
 * since it... has a weapon, and doesn't just use its body.
 */
public class EnemyLumi : EnemyRanged {

    [SerializeField] SpriteRenderer shadow;
    float moveSpeed = 0.25f;

    private new void Start() {
        base.Start();
        reappearPosition = transform.position;
        StartCoroutine(LumiBehavior());
        dampening = new Vector2(0.05f, 0.05f);
    }

    protected override void OnFixedUpdate() {
        float d = Vector2.Distance(transform.position, reappearPosition);
        Vector2 desiredVelocity;

        if (d > 0.1f)
            desiredVelocity = (reappearPosition - (Vector2)transform.position).normalized * moveSpeed;
        else
            desiredVelocity = Vector2.zero;

        looking = desiredVelocity;
        rb.velocity = (desiredVelocity + knockback) * (1 - slowModifier);
    }

    protected override void OnUpdate() {
        updateLookingAnim();
    }

    void Disappear() {
        mySpriteAnimator.SetTrigger("disappear");
        GetComponentInParent<Collider2D>().enabled = false;
        myHPBar.Disappear();
        shadow.enabled = false;
    }

    void Reappear() {
        transform.root.position = reappearPosition;
        mySpriteAnimator.SetTrigger("reappear");

        if(myStatusEffectManager.HasEffects) {
            myHPBar.Appear();
        }

        GetComponent<Collider2D>().enabled = true;
        shadow.enabled = true;
    }

    bool FindReappearPosition() {
        int maxTries = 25;
        IntRange radius = new IntRange(10, 10);
        LayerMask ignore = LayerMask.GetMask("Player", "Rocks");

        for(int i = 0; i < maxTries; i++) {
            var add = Random.insideUnitCircle * radius.Num();
            var test = (Vector2)PlayerController.instance.transform.position + add;
            // Debug.Log(test);

            if (!Physics2D.OverlapCircle(test, 1f, ignore, -100, 100)) {
                reappearPosition = test;
                return true;
            }
        }

        return false;
    }

    Vector2 reappearPosition;
    IEnumerator LumiBehavior() {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        FindReappearPosition();
        Disappear();
        yield return new WaitForSeconds(1f);
        Reappear();
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++) {
            myWeaponController.ManualAttack();
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine(LumiBehavior());
    }
}
