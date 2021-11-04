using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrianManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag("Projectiles")) {
            var position = Vector2.Lerp(transform.position, collision.transform.position, 0.5f);
            var debris = Resources.Load("Prefabs/Debris", typeof(GameObject)) as GameObject;
            Instantiate(debris, position, Quaternion.Euler(-90, 0, 0));
        }

        
    }
}
