using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotbarSelectorController : MonoBehaviour
{
    #region singleton
    public static HotbarSelectorController instance;
    private void Awake() {
        if(instance != null) {
            Debug.LogError("Found more than one HotbarSelectorController in the scene!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField] CosmeticSlotManager[] hotbarSlots;
    [SerializeField] CanvasGroup hotbarCanvasGroup;
    public CosmeticSlotManager currentSlot;
    Item currentItem;
    int index = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        if(hotbarSlots.Length == 0) {
            Debug.LogError("The hotbar selector was not assigned any cosmetic slots to iterate over!");
            return;
        }

        Invoke("UpdatePosition", 0.1f);
    }

    // Update is called once per frame
    void Update()
    {
        if(hotbarCanvasGroup.alpha == 0) {
            return;
        }

        var scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if(scroll > 0) {
            index = (index + 1) % hotbarSlots.Length;
            UpdatePosition();
        } else if(scroll < 0) {
            index -= 1;
            if(index < 0) {
                index = hotbarSlots.Length - 1;
            }
            UpdatePosition();
        }

        // Attempts to use item in the slot; on success,
        // decrements slot quantity and updates UI
        if (Input.GetKeyDown(KeyCode.Space)) {

            SlotManager inventorySlot = currentSlot.linkedSlot;

            // Can't use an item in an empty slot
            if(inventorySlot.mySlotInfo.myItem == null) {
                return;
            }

            if (inventorySlot.mySlotInfo.myItem.Use()) {
                inventorySlot.mySlotInfo.changeQuantity(-1);
            }
            inventorySlot.UpdateUI(); 
        }
    }

    void UpdatePosition() {
        currentSlot = hotbarSlots[index];
        transform.position = currentSlot.transform.position;
        UISFX.instance.PlaySound(UISFX.sounds.scroll);
    }
}
