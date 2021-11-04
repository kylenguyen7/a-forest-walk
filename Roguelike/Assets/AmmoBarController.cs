using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBarController : MonoBehaviour
{
    Stack<GameObject> childrenArrows;
    [SerializeField]
    GameObject arrowPrefab;

    void Start()
    {
        childrenArrows = new Stack<GameObject>();
        for(int i = 0; i < transform.childCount; i++) {
            childrenArrows.Push(transform.GetChild(i).gameObject);
        }
    }

    void Update()
    {
        matchArrowCount();
    }

    private void matchArrowCount() {
        int diff = Mathf.Abs(childrenArrows.Count - WeaponControllerPlayer.instance.Ammo);

        if (childrenArrows.Count > WeaponControllerPlayer.instance.Ammo) {
            for (int i = 0; i < diff; i++) {
                Destroy(childrenArrows.Pop());
            }
        } else if (childrenArrows.Count < WeaponControllerPlayer.instance.Ammo) {
            for (int i = 0; i < diff; i++) {
                GameObject arrow = Instantiate(arrowPrefab, transform);
                childrenArrows.Push(arrow);
            }
        }
    }
}
