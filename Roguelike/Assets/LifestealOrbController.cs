using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifestealOrbController : MonoBehaviour
{
    Vector2 travelDirection;
    Rigidbody2D rb;
    float startSpeed = 3f;
    float endSpeed = 15f;
    float speed;

    float startHomingScale = 1f;
    float endHomingScale = 20f;
    float homingScale;

    public ParticleSystem particles;

    private void Start() {
        travelDirection = Random.insideUnitCircle;
        rb = GetComponent<Rigidbody2D>();

        speed = startSpeed;
        LeanTween.value(gameObject, SetSpeed, startSpeed, endSpeed, 2f);

        homingScale = startHomingScale;
        LeanTween.value(gameObject, SetHomingScale, startSpeed, endSpeed, 0.5f);
    }

    void SetSpeed(float spd) {
        speed = spd;
    }

    void SetHomingScale(float scale) {
        homingScale = scale;
    }

    private void FixedUpdate() {
        travelDirection = Utility.GetHomingDir(travelDirection, transform.position, PlayerController.instance.transform.position, homingScale);
        rb.velocity = travelDirection * speed;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player")) {
            PlayerController player = collision.GetComponent<PlayerController>();
            player.Heal(Random.Range(5f, 10f));

            MyDestroy();
        }
    }

    private void MyDestroy() {
        particles.transform.parent = null;
        particles.Stop();

        Destroy(gameObject);

        Debug.Log("Destroyed");
    }
}
