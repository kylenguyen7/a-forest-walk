using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotContainer : MonoBehaviour
{
    // SlotContainer is a class for a parent container
    // of SlotManager objects (i.e. the hotbar). It
    // contains various utility functions such as FindSpaceFor()
    
    SlotManager[] mySlots;

    public SlotManager[] MySlots {
        get { return mySlots; }
    }

    public SlotManager FindSpaceFor(Item item) {
        SlotManager firstEmpty = null;
        for(int i = 0; i < mySlots.Length; i++) {

            // Prefer to find space in a current stack
            if (mySlots[i].mySlotInfo.myItem == item && mySlots[i].HasSpace) {
                return mySlots[i];
            }

            if(firstEmpty == null && mySlots[i].mySlotInfo.myItem == null) {
                firstEmpty = mySlots[i];
            }
        }

        // Otherwise, return first empty slot
        if(firstEmpty != null) {
            return firstEmpty;
        }

        // Otherwise, inventory is full
        return null;
    }

    void Start()
    {
        mySlots = new SlotManager[transform.childCount];

        for(int i = 0; i < transform.childCount; i++) {
            var slot = transform.GetChild(i).GetComponent<SlotManager>();
            if(slot == null) {
                Debug.LogError("A SlotContiner has a child without a SlotManager component.");
                continue;
            }

            mySlots[i] = slot;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
