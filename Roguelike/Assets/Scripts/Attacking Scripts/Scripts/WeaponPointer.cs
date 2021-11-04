using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPointer : MonoBehaviour
{
    public Transform rightHand;
    public Transform leftHand;
    public Transform weaponsPivot;
    public float offsetFromPivot;

    private PlayerController player;
    private SpriteRenderer mySprite;
    private Rigidbody2D rb;
    public Animator animator;

    private bool deadTrigger = false;

    private void Start() {
        player = GetComponentInParent<PlayerController>();
        mySprite = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0) return;
        
        if (player.isAlive) {
            aliveMovement();
        } else if (!deadTrigger) { // one-time trigger when dead
            rb.simulated = true;
            var vel = new Vector2(Random.Range(1f, 2f), Random.Range(1f, 2f));

            if (Random.value > 0.5) vel.x = -vel.x;
            if (Random.value > 0.5) vel.y = -vel.y;

            rb.velocity = vel;
            rb.angularVelocity = Random.Range(200, 360);
            deadTrigger = true;
        }
    }

    public void aliveMovement() {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 point = mousePos - (Vector2)transform.position;

        transform.up = point;

        Vector3 targetPos;
        /*
        if (mousePos.x > transform.parent.position.x) {
            targetPos = player.lookingRight ? rightHand.position : leftHand.position;
        } else {
            targetPos = player.lookingRight ? leftHand.position : rightHand.position;
        }
        */

        if(mousePos.y > weaponsPivot.position.y) {
            mySprite.sortingOrder = 9;
        } else {
            mySprite.sortingOrder = 11;
        }

        targetPos = (Vector2)weaponsPivot.position + point.normalized * offsetFromPivot;

        var currentPos = transform.position;
        currentPos += (targetPos - currentPos) / 3;
        transform.position = currentPos;
    }
}
