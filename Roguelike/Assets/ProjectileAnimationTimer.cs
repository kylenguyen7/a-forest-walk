using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAnimationTimer : MonoBehaviour
{
    ProjectileController myProjectile;

    private void Start() {
        myProjectile = GetComponent<ProjectileController>();
    }

    public void Destroy() {
        myProjectile.MyDestroy();
    }
}
