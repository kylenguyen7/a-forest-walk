using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileSearchController : MonoBehaviour
{
    ProjectileController parent;

    private void Start() {
        parent = GetComponentInParent<ProjectileController>();
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Enemy")) {
            parent.enemiesInBounds.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.CompareTag("Enemy")) {
            parent.enemiesInBounds.Remove(collision.gameObject);
        }
    }
}
