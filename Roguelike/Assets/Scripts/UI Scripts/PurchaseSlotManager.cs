using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PurchaseSlotManager : SlotManager {
    [SerializeField] TextMeshProUGUI myCostLabel;

    [SerializeField]
    Item[] randomItems;
    [SerializeField]
    bool isRandomized = false;

    int cost = -1;

    private new void Start() {
        base.Start();
        cost = Random.Range(10, 25);
        myCostLabel.text = cost.ToString();

        if (isRandomized) {
            mySlotInfo.myItem = randomItems[Random.Range(0, randomItems.Length)];
            mySlotInfo.quantity = Random.Range(1, mySlotInfo.myItem.maxStack + 1);
        }
        UpdateUI();
    }

    protected override void OnLeftClick() {
        if(mySlotInfo.myItem == null) {
            return;
        }

        if(PlayerController.gold >= cost) {
            PlayerController.gold -= cost;
            InventoryManager.instance.Pickup(mySlotInfo.myItem);
            mySlotInfo.changeQuantity(-1);
        }

        UpdateUI();
    }

    protected override void OnRightClick() {
        return;
    }
}
