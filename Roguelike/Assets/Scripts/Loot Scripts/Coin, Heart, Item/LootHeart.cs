using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootHeart : LootController {
    public override bool DoTrueHoming => true;

    public override void Pickup() {
        Destroy(gameObject);
    }
}
