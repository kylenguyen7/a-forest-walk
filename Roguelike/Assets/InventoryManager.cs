using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // This is the master controller of the SlotContainers,
    // which are in turn the controllers of various SlotManagers.
    // This is the class that defines behavior for picking up items,
    // which requires scanning the entire inventory for valid space.

    // This is different from dropping items or moving items around/between
    // inventories, which are handled individually by the SlotManagers
    // (Those functions only require interaction between two SlotManagers).


    #region singleton
    public static InventoryManager instance;
    public void Awake() {
        if(instance != null) {
            Debug.LogError("More than one instance of InventoryManager found!");
            return;
        }
        instance = this;
    }
    #endregion

    SlotContainer[] mySlotContainers;
    private void Start() {
        mySlotContainers = GetComponentsInChildren<SlotContainer>();
    }

    public bool HasSpaceFor(Item item) {
        if (item == null) {
            Debug.LogError("HasSpaceFor was called on a null object. InventoryManager returning false.");
            return false;
        }

        foreach(SlotContainer container in mySlotContainers) {
            SlotManager slot = container.FindSpaceFor(item);
            if (slot != null) {
                Debug.Log($"Found space in {slot.gameObject.name}.");
                return true;
            }
        }

        return false;
    }

    public bool Pickup(Item item) {
        foreach (SlotContainer container in mySlotContainers) {
            SlotManager slot = container.FindSpaceFor(item);

            if (slot != null) {
                // For the case of adding to an empty slot
                slot.mySlotInfo.myItem = item;
                slot.mySlotInfo.changeQuantity(1);
                slot.UpdateUI();

                PickupManager.instance.CreatePickupText(item, PlayerController.instance.origin.position);
                return true;
            }
        }

        return false;
    }
}
