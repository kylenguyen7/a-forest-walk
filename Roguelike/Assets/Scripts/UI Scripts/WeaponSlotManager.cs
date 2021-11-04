using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlotManager : SlotManager {
    [SerializeField] bool isPrimary;

    private void Update() {
        UpdateUI();
    }

    new void UpdateUI() {

        if(isPrimary) {
            mySlotInfo.myItem = WeaponControllerPlayer.instance.primaryWeapon.associatedItem;
        } else {
            mySlotInfo.myItem = WeaponControllerPlayer.instance.offhandWeapon.associatedItem;
        }
        
        base.UpdateUI();
    }

    protected override void OnLeftClick() {
        return;
    }

    protected override void OnRightClick() {
        return;
    }
}
