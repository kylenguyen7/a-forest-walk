using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageTextController : MonoBehaviour
{
    TextMeshPro myText;
    Vector2 targetPos;

    bool facingRight;

    float pulseSize = 1.5f;
    float defaultSize = 1f;

    [SerializeField] Color defaultColor;
    [SerializeField] Color poisonColor;
    [SerializeField] Color critColor;
    
    void Start()
    {
        targetPos = GetRandomPoint();

        // Grow to pulseSize, then return to defaultSize
        LeanTween.scale(gameObject, new Vector3(pulseSize, pulseSize, 1f), 0.1f);
        LeanTween.scale(gameObject, new Vector3(defaultSize, defaultSize, 1f), 0.3f).setDelay(0.2f);

        // Rotate
        if(facingRight) {
            LeanTween.rotateZ(gameObject, -7f, 0.2f);
        } else {
            LeanTween.rotateZ(gameObject, 7f, 0.2f);
        }
        LeanTween.rotateZ(gameObject, 0f, 0.3f).setDelay(0.3f);

        // Move up
        LeanTween.move(gameObject, targetPos, 0.3f);
        LeanTween.move(gameObject, targetPos + new Vector2(0f, 1f), 1f).setDelay(0.7f);

        LeanTween.scale(gameObject, new Vector3(defaultSize + 0.2f, 0, 1f), 0.1f).setDelay(1f);
    }

    Vector2 GetRandomPoint() {
        float angle = Random.Range(Mathf.PI/6, Mathf.PI * 5/6);

        facingRight = angle < Mathf.PI / 2;

        float r = Random.Range(1f, 2f);
        return (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * r;
    }

    public void Init(float damage) {
        GeneralInit(damage);
    }

    public void Init(float damage, bool crit, StatusEffect.effects effect) {
        var text = GeneralInit(damage);

        if (crit) {
            myText.color = critColor;
            pulseSize = 2f;
            defaultSize = 1.5f;
        } else if(effect != StatusEffect.effects.none) {
            text.color = GetStatusColor(effect);
        }

    }

    public TextMeshPro GeneralInit(float damage) {
        float fraction = damage - Mathf.Floor(damage);
        if (Random.value < fraction) damage = Mathf.Ceil(damage);
        else damage = Mathf.Floor(damage);

        myText = GetComponent<TextMeshPro>();
        if (myText == null) return null;

        myText.text = damage.ToString();
        myText.color = defaultColor;
        return myText;
    }

    Color GetStatusColor(StatusEffect.effects effect) {
        switch(effect) {
            case StatusEffect.effects.poison: return poisonColor;
            default: return defaultColor;
        }
    }
}
