 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public abstract class SlotManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
    [SerializeField] protected Image myImage;
    [SerializeField] protected TextMeshProUGUI myQuantityLabel;
    [SerializeField] protected TextMeshProUGUI myQuantityLabelBkg;

    public SlotInfo mySlotInfo;

    public delegate void OnUpdateUI();
    public OnUpdateUI OnUpdateUICallback;

    protected bool hovered = false;
    Animator animator;

    protected void Start() {
        animator = GetComponent<Animator>();
    }

    public void UpdateUI() {

        // Update quantity label (special case for weapons)
        if (mySlotInfo.myItem != null && mySlotInfo.myItem.myCategory == Item.Category.Weapon) {
            myQuantityLabel.text = "";
        }
        else if (mySlotInfo.quantity == 0) {
            myQuantityLabel.text = "";
            mySlotInfo.myItem = null;
        }
        else {
            myQuantityLabel.text = mySlotInfo.quantity.ToString();
        }

        // Update item sprite
        if (mySlotInfo.myItem == null) {
            myImage.sprite = Resources.Load("Sprites/Empty", typeof(Sprite)) as Sprite;
        } else {
            myImage.sprite = mySlotInfo.myItem.itemSprite;
        }

        myQuantityLabelBkg.text = myQuantityLabel.text;

        if(OnUpdateUICallback != null) {
            OnUpdateUICallback.Invoke();
        }
    }

    public bool HasSpace {
        get { return mySlotInfo.HasSpace; }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        hovered = true;
        Debug.Log($"Hovered over: {gameObject.name}");
        if(animator != null) {
            animator.SetBool("isHovered", true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        hovered = false;
        Debug.Log($"Unhovered over: {gameObject.name}");
        if (animator != null) {
            animator.SetBool("isHovered", false);
        }
    }

    protected abstract void OnLeftClick();
    protected abstract void OnRightClick();

    [SerializeField] bool test = false;
    protected void Update() {
        if (hovered) {
            if (Input.GetMouseButtonDown(0)) {
                OnLeftClick();
                UpdateUI();
            } else if (Input.GetMouseButtonDown(1)) {
                OnRightClick();
                UpdateUI();
            }
        }
        if(test) Debug.Log(hovered);
    }
}

public class InventorySlotManager : SlotManager {
    #region // For random items
    //[SerializeField]
    Item[] randomItems;
    //[SerializeField]
    bool isRandomized = false;
    #endregion

    private new void Start() {
        base.Start();

        if (myImage == null || myQuantityLabel == null) {
            Debug.LogError("Inventory slot is missing an image or quantity label.");
        }

        if(isRandomized) {
            mySlotInfo.myItem = randomItems[Random.Range(0, randomItems.Length)];
            mySlotInfo.quantity = Random.Range(0, mySlotInfo.myItem.maxStack + 1);
        }

        UpdateUI();
    }

    protected override void OnLeftClick() {
        SlotInfo cursor = CursorManager.instance.cursorSlotInfo;

        // Empty cursor; pick up whole stack
        if (cursor.myItem == null) {
            cursor.setSlotInfo(mySlotInfo);
            mySlotInfo.setSlotInfo(null);
        } // Full cursor, different item; swap stacks
        else if(cursor.myItem != mySlotInfo.myItem) {
            var item = cursor.myItem;
            var quantity = cursor.quantity;

            cursor.setSlotInfo(mySlotInfo);
            mySlotInfo.setSlotInfo(item, quantity);
        } // Full cursor, same item; place as many as possible
        else {
            int space = mySlotInfo.myItem.maxStack - mySlotInfo.quantity;
            int numToTake = Mathf.Min(space, cursor.quantity);

            // Place all
            if(numToTake == cursor.quantity) {
                cursor.setSlotInfo(null);
                mySlotInfo.changeQuantity(numToTake);
            } // Place only some
            else {
                cursor.changeQuantity(-numToTake);
                mySlotInfo.changeQuantity(numToTake);
            }
        }

        CursorManager.instance.cursorSlotInfo = cursor;
        CursorSlotManager.instance.UpdateCursor();
    }

    protected override void OnRightClick() {
        if(mySlotInfo.myItem == null) {
            return;
        }

        SlotInfo cursor = CursorManager.instance.cursorSlotInfo;

        // Empty cursor; take 1
        if (cursor.myItem == null) {
            cursor.setSlotInfo(mySlotInfo.myItem, 1);
            mySlotInfo.changeQuantity(-1);
        } // Full cursor, different item; do nothing
        else if (cursor.myItem != mySlotInfo.myItem) {
            return;
        } // Full cursor, same item; take 1 if there is space
        else {
            if(cursor.quantity < cursor.myItem.maxStack) {
                cursor.changeQuantity(1);
                mySlotInfo.changeQuantity(-1);
            }
        }
        CursorSlotManager.instance.UpdateCursor();
    }
}
