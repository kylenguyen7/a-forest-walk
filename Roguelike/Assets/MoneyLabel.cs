using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MoneyLabel : MonoBehaviour
{
    TextMeshProUGUI myText;
    Animator animator;
    int myGold = 0;

    private void Start() {
        myText = GetComponent<TextMeshProUGUI>();
        animator = GetComponent<Animator>();
        myGold = PlayerController.gold;
        myText.text = myGold.ToString();

        StartCoroutine(MatchGold());
    }

    IEnumerator MatchGold() {
        while(true) {
            int gold = PlayerController.gold;

            if (gold != myGold) {
                myGold += (int)Mathf.Sign((gold - myGold));
                myText.text = myGold.ToString();

                if (animator != null) animator.SetTrigger("pulse");
            }

            yield return new WaitForSecondsRealtime(0.01f);
        }
    }
}
