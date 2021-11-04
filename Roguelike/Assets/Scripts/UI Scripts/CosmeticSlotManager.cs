using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticSlotManager : SlotManager
{
    public SlotManager linkedSlot;

    private void Start() {
        UpdateUI();
        linkedSlot.OnUpdateUICallback += UpdateUI;
    }

    new void UpdateUI() {
        mySlotInfo = linkedSlot.mySlotInfo;
        base.UpdateUI();
    }

    protected override void OnLeftClick() {
        return;
    }

    protected override void OnRightClick() {
        return;
    }
}
