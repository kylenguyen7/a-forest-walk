using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SECosmeticsManager : MonoBehaviour
{
    List<SECosmeticsController> myControllers = new List<SECosmeticsController>();
    float margin = 0.25f;

    public bool HasEffects { get { return myControllers.Count != 0; } }

    public SECosmeticsController AddCosmetic(StatusEffect.effects effect) {
        SECosmeticsController controller = Instantiate(Resources.Load<GameObject>("Prefabs/StatusEffectCosmetic"), transform)
                                            .GetComponent<SECosmeticsController>();

        controller.Init(effect);
        myControllers.Add(controller);

        UpdateOffsets();
        return controller;
    }

    public void RemoveCosmetic(SECosmeticsController toRemove) {
        myControllers.Remove(toRemove);
        Destroy(toRemove.gameObject);
        UpdateOffsets();
    }

    public void UpdateOffsets() {
        float startX = 0; // -((myControllers.Count - 1) * margin)/2;

        for(int i = 0; i < myControllers.Count; i++) {
            myControllers[i].xOffset = startX + i * margin;
        }
    }
}
