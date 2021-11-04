using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootCoin : LootController {
    public override bool DoTrueHoming => true;
    [SerializeField] Transform myShadow;
    [SerializeField] float maxShadowHeight;

    public override void Pickup() {
        PlayerController.gold += 1;
        GameManager.instance.StatTotalGold += 1;
        DoPickupEffects();
        Destroy(gameObject);
    }

    private new void FixedUpdate() {
        base.FixedUpdate();

        var pos = myShadow.position;
        pos.y = groundY;
        if(Mathf.Abs(transform.position.y - pos.y) > maxShadowHeight) {
            pos.y = transform.position.y - maxShadowHeight;
        }
        myShadow.position = pos;
    }
}
