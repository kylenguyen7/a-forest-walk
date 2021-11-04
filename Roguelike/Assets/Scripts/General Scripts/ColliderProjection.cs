using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ColliderProjection : MonoBehaviour
{
    List<GameObject> myCollisions = new List<GameObject>();
    [SerializeField] LayerMask collisionLayers;
    public bool debugStatements = false;

    // Gets the first object in the collisions list (the most recent)
    public GameObject Object {
        get { return myCollisions[0]; }
    }


    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.transform == transform.parent) return;
        if (collisionLayers == (collisionLayers | (1 << collision.gameObject.layer))) {
            myCollisions.Add(collision.gameObject);

            if(debugStatements) {
                Debug.Log($"Adding {collision.gameObject} to myCollisions.");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (myCollisions.Contains(collision.gameObject)) {
            myCollisions.Remove(collision.gameObject);
        }
    }

    public bool IsColliding() {
        return myCollisions.Count != 0;
    }
}
