using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootItem : LootController {
    public override bool DoTrueHoming => false;

    // This class inherits from LootController; its counterparts
    // are LootCoin and LootHeart (or whatever else) which each
    // define special Pickup behaviors.

    // They all share the magnet behavior of LootController.

    public override void Pickup() {
        if(myLoot.myItem == null) {
            Debug.LogError($"A LootItem, {gameObject.name} is missing a myItem assignment!");
        } else {
            InventoryManager.instance.Pickup(myLoot.myItem);
        }
        Destroy(gameObject);
    }
}
