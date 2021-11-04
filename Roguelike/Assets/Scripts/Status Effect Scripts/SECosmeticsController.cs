using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SECosmeticsController : MonoBehaviour
{
    StatusEffect.effects myEffect;
    SpriteRenderer mySprite;
    [HideInInspector] public float xOffset = 0;

    private void Awake() {
        mySprite = GetComponent<SpriteRenderer>();
    }

    public void Init(StatusEffect.effects effect) {
        myEffect = effect;
        mySprite.sprite = StatusEffect.GetSprite(effect);
    }

    private void FixedUpdate() {
        if(transform.localPosition.x != xOffset) {
            transform.localPosition = new Vector3(xOffset, 0, 0);
            // Debug.Log(xOffset);
        }
    }
}
