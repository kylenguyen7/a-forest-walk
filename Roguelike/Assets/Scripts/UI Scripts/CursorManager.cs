using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotInfo {
    public Item myItem;
    public int quantity;

    public void setSlotInfo(Item newItem, int newQuantity) {
        myItem = newItem;
        quantity = newQuantity;
        if (quantity < 0 || quantity > myItem.maxStack) {
            Debug.LogError("An item's quantity was set outside of the allowed quantity bounds.");
        }
    }

    public void setSlotInfo(SlotInfo newSlotInfo) {
        if(newSlotInfo == null) {
            myItem = null;
            quantity = 0;
            return;
        }

        myItem = newSlotInfo.myItem;
        quantity = newSlotInfo.quantity;

        if (myItem != null && (quantity < 0 || quantity > myItem.maxStack)) {
            Debug.LogError("An item's quantity was set outside of the allowed quantity bounds.");
        }
    }

    public void changeQuantity(int changeAmount) {
        quantity += changeAmount;

        if(quantity == 0) {
            myItem = null;
        }
        
        if(myItem != null && (quantity < 0 || quantity > myItem.maxStack)) {
            Debug.LogError("An item's quantity was changed outside of the allowed quantity bounds.");
        }
    }

    public bool HasSpace {
        get {
            return myItem == null || quantity < myItem.maxStack;
        }
    }
}

// Handles all cursor actions
public class CursorManager : MonoBehaviour
{
    #region singleton
    public static CursorManager instance;

    private void Awake() {
        if(instance != null) {
            Debug.LogError("Found more than one CursorManager!");
            return;
        }
        instance = this;
    }
    #endregion

    public SlotInfo cursorSlotInfo;

    private void Update() {
        // Debug.Log($"Cursor slot is holding {cursorSlotInfo.myItem}");
    }
}
