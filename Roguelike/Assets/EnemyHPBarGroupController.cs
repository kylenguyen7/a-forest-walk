using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBarGroupController : MonoBehaviour
{
    Transform myEnemy;
    SpriteRenderer mySprite;

    SpriteRenderer[] children;
    EnemyHPBarController[] bars;

    public void Inititalize(Vector2 offset, Transform creator) {
        transform.localPosition = offset;
        myEnemy = creator;
    }

    private void Start() {
        children = GetComponentsInChildren<SpriteRenderer>();
        bars = GetComponentsInChildren<EnemyHPBarController>();

        mySprite = GetComponent<SpriteRenderer>();

        SetAlpha(0f, 0f);
    }

    private void Update() {
        if (myEnemy == null) {
            Destroy(gameObject);
            return;
        }
    }

    public void Appear() {
        SetAlpha(1f, 0.1f);
        StopAllCoroutines();
        StartCoroutine(ResetAlpha());
    }

    public void Disappear() {
        SetAlpha(0f, 0.1f);
    }

    public void UpdateHPBar(float ratio) {
        foreach(EnemyHPBarController bar in bars) {
            bar.ratio = ratio;
        }

        Appear();
    }

    IEnumerator ResetAlpha() {
        yield return new WaitForSeconds(3f);

        // If doing status effects, reset timer instead
        bool doingStatusEffects = gameObject.GetComponentInChildren<SECosmeticsManager>().HasEffects;
        if (doingStatusEffects) {
            StartCoroutine(ResetAlpha());
        } else {
            Disappear();
        }
    }

    private void SetAlpha(float alpha, float time) {
        LeanTween.alpha(gameObject, alpha, time);
        foreach (SpriteRenderer child in children) {
            LeanTween.alpha(child.gameObject, alpha, time);
        }
    }
}
