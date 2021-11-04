using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Effectable))]
public class StatusEffectManager : MonoBehaviour
{
    // Handles calling the OnUpdate() of various StatusEffects
    [HideInInspector]
    public Effectable myEffectable;
    List<StatusEffect> myStatusEffects = new List<StatusEffect>();
    private SECosmeticsManager myCosmetics;

    public bool HasEffects { get { return myStatusEffects.Count != 0; } }

    public void Awake() {
        myEffectable = gameObject.GetComponent<Effectable>();
        // AddStatusEffect(StatusEffect.effects.slow, 0f, 10f);
        // AddStatusEffect(StatusEffect.effects.poisoned, 2f, 5f);
    }

    // Called manually after the HP bar is added to the enemy.
    // MUST be called before this object's first Update call
    public void FindCosmeticsManager() {
        myCosmetics = gameObject.GetComponentInChildren<SECosmeticsManager>();
    }

    private void OnTransformChildrenChanged() {
        int numChildren = transform.childCount;

        
        for(int i = 0; i < numChildren; i++) {
            
        }
    }

    public void Update() {
        if (myEffectable != null)
            DoStatusEffects();
        else {
            Debug.LogError($"A StatusEffectManager of {gameObject.name} is missing its Effectable. Destroying self.");
            Destroy(this);
        }
    }

    void DoStatusEffects() {
        foreach(StatusEffect statusEffect in myStatusEffects) {
            statusEffect.OnUpdate();
        }
    }

    public void AddStatusEffect(StatusEffect.effects statusEffect, float severity, float duration) {

        if (statusEffect == StatusEffect.effects.none) return;

        StatusEffect toAdd = null;
        switch(statusEffect) {
            case StatusEffect.effects.poison: toAdd = ScriptableObject.CreateInstance<StatusEffectPoisoned>(); break;
            case StatusEffect.effects.slow: toAdd = ScriptableObject.CreateInstance<StatusEffectSlowed>(); break;
            case StatusEffect.effects.weak: toAdd = ScriptableObject.CreateInstance<StatusEffectWeak>(); break;
        }

        if (toAdd == null) {
            Debug.LogError("Missing a StatusEffect assignment for a StatusEffect enum");
            return;
        }

        int count = 0;
        foreach (StatusEffect effect in myStatusEffects) {
            if (effect.GetType() == toAdd.GetType()) {
                count++;
            }
        }

        if (count >= toAdd.MaxStacks) return;
        if (duration == 0) return;

        // Add status effect
        myStatusEffects.Add(toAdd);
        toAdd.Init(this, severity);
        // Debug.Log($"Adding status effect to {myCosmetics}.");
        var cosmeticController = myCosmetics.AddCosmetic(statusEffect);
        myEffectable.mySprite.material = toAdd.EffectMaterial;

        // Remove status effect after a certain amount of time
        StartCoroutine(RemoveStatusEffect(toAdd, duration, cosmeticController));
    }

    IEnumerator RemoveStatusEffect(StatusEffect statusEffect, float delay, SECosmeticsController cosmeticController) {
        yield return new WaitForSeconds(delay);

        statusEffect.OnFinish();
        myStatusEffects.Remove(statusEffect);
        myEffectable.mySprite.material = GetCurrentMat();
        myCosmetics.RemoveCosmetic(cosmeticController);
    }

    public Material GetCurrentMat() {
        if(myStatusEffects.Count > 0) {
            return myStatusEffects[myStatusEffects.Count - 1].EffectMaterial;
        } else {
            return Resources.Load<Material>("Default");
        }
    }
}
