using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CursorSlotManager : MonoBehaviour
{
    #region singleton
    public static CursorSlotManager instance;

    private void Awake() {
        if (instance != null) {
            Debug.LogError("Found more than one CursorSlotManager!");
            return;
        }
        instance = this;
    }
    #endregion

    [SerializeField] Image myImage;
    [SerializeField] TextMeshProUGUI myQuantityLabel;
    [SerializeField] TextMeshProUGUI myQuantityLabelBkg;

    public Texture2D defaultCursor;

    private void Start() {
        myQuantityLabel.text = "";
        myQuantityLabelBkg.text = "";
        myImage.sprite = Resources.Load("Sprites/Empty", typeof(Sprite)) as Sprite;
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }

    private void Update() {
        transform.position = Input.mousePosition;
    }

    // Called when an inventory slot detects a click;
    // Fetches info from the InventoryManager and updates display
    public void UpdateCursor() {
        var cursorInfo = CursorManager.instance.cursorSlotInfo;

        if(cursorInfo.myItem == null) {
            myQuantityLabel.text = "";
            myImage.sprite = Resources.Load("Sprites/Empty", typeof(Sprite)) as Sprite;
            Cursor.visible = true;
        } else {
            myQuantityLabel.text = cursorInfo.quantity.ToString();
            myImage.sprite = cursorInfo.myItem.itemSprite;
            Cursor.visible = false;
        }

        myQuantityLabelBkg.text = myQuantityLabel.text;
    }
}
