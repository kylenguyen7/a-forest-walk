using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Collider2D))]
public abstract class LootController : MonoBehaviour
{
    public Loot myLoot;
    private Rigidbody2D rb;
    protected float groundY;

    private bool bouncing = true;
    private float speed = 9f;
    private Vector2 direction;
    private float bounceChance = 0.5f;

    public abstract bool DoTrueHoming { get; }

    private bool homing = false;
    private float homingDelay = 0.2f;
    private float homingAccel = 1f;
    private float homingSpeed = 0.5f;
    private float homingMaxSpeed = 13f;
    private Vector2 homingDir;
    private float trueHomingRange = 3f;

    private float fakeHomingRange = 2f;

    private void Start() {
        // Set up gravity and sprite based on associated Loot object
        groundY = transform.position.y + Random.Range(-1f, 0.3f);
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = myLoot.gravity;

        GetComponent<SpriteRenderer>().sprite = myLoot.sprite;
        gameObject.AddComponent<PolygonCollider2D>().isTrigger = true;

        var angle = Random.Range(50, 130) * Mathf.Deg2Rad;
        direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
        rb.velocity = direction * speed;
    }

    protected void FixedUpdate() {
        // Trigger homing behavior
        if (homingDelay < 0 && !homing) {
            if(DoTrueHoming) {
                homing = Vector2.Distance(PlayerController.instance.transform.position, transform.position) < trueHomingRange;
                // Initial homingDir set
                if (homing) {
                    Vector2 toPlayer = (PlayerController.instance.transform.position - transform.position).normalized;
                    float angle = Vector2.SignedAngle(new Vector2(1, 0), toPlayer);
                    if (angle < 0) {
                        angle = angle + 360;
                    }

                    angle += Random.Range(-130, 130);

                    homingDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad),
                                            Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;

                }
            } else {
                homing = Vector2.Distance(PlayerController.instance.transform.position, transform.position) < fakeHomingRange;
            }
        } else {
            homingDelay -= Time.deltaTime;
        }

        // Do homing behavior once triggered
        if (homing) {
            if (DoTrueHoming) {
                homingDir = Utility.GetHomingDir(homingDir, transform.position, PlayerController.instance.transform.position, 15f);
                rb.velocity = homingDir * homingSpeed;
                homingSpeed = Mathf.Min(homingSpeed + homingAccel, homingMaxSpeed);
            } else {
                homingDir = (PlayerController.instance.transform.position - transform.position).normalized;
                rb.velocity = homingDir * homingSpeed;
                homingSpeed += homingAccel;
            }
            return;
        }

        // Else, just bounce
        if (transform.position.y <= groundY && rb.velocity.y < 0) {
            if (Random.value < bounceChance) {
                speed *= 0.7f;
                bounceChance /= 2f;
                rb.velocity = direction * speed;
            } else {
                rb.velocity = Vector2.zero;
                rb.gravityScale = 0f;
                bouncing = false;
            }
        }
    }

    protected void DoPickupEffects() {
        GameObject sparkle = Resources.Load("Prefabs/Sparkle") as GameObject;
        Instantiate(sparkle, transform.position, Quaternion.identity);

        if (myLoot.pickupSfx == null) return;
        var sfx = Instantiate(Resources.Load("Prefabs/Sound Effect") as GameObject, transform.position, Quaternion.identity);
        sfx.GetComponent<AudioSource>().PlayOneShot(myLoot.pickupSfx);
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("RoomBounds")) {
            RoomManager room = collision.gameObject.GetComponent<RoomManager>();
            if (room == null) {
                Debug.LogError("An object tagged RoomBounds doesn't have a RoomManager component assigned!");
                return;
            }

            room.OnRoomFinished += OnRoomFinished;
        }
    }

    public void OnRoomFinished() {
        trueHomingRange = 100f;
    }

    public abstract void Pickup();
}

[System.Serializable]
public class LootDropInfo {
    public float chance;
    public Loot loot;
    public int minQuantity;
    public int maxQuantity;
}
public class LootUtility : MonoBehaviour {
    public static void DropLoot(LootDropInfo[] dropInfo, Vector2 position) {

        if (dropInfo == null) {
            Debug.LogError("Attempted to call DropLoot on a null LootDropInfo[].");
            return;
        }

        // Cycles through the dropInfos, dropping a [min, max] number
        // of the appropriate loot object if the chance is high enough.
        foreach (LootDropInfo info in dropInfo) {
            if (Random.value < info.chance) {
                int num = Random.Range(info.minQuantity, info.maxQuantity);
                GameObject prefab = null;

                // Get prefab (heart, coin, item), which are separate because
                // they have different LootControllers
                switch (info.loot.myLootType) {
                    case Loot.lootType.coin: {
                            prefab = Resources.Load("Prefabs/LootPrefabs/Coin", typeof(GameObject)) as GameObject;
                        }
                        break;
                    case Loot.lootType.heart: {
                            prefab = Resources.Load("Prefabs/LootPrefabs/Heart", typeof(GameObject)) as GameObject;
                        }
                        break;
                    case Loot.lootType.item: {
                            prefab = Resources.Load("Prefabs/LootPrefabs/Item", typeof(GameObject)) as GameObject;
                        }
                        break;
                }

                if (prefab == null) {
                    Debug.LogError("When attempting to drop loot, failed to find prefab (heart, coin, item) to make.");
                    continue;
                }

                for (int i = 0; i < num; i++) {
                    var loot = Instantiate(prefab, position, Quaternion.identity).GetComponent<LootController>();
                    loot.myLoot = info.loot;
                }
            }
        }
    }
}
