using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMandrakeTimer : MonoBehaviour
{
    public void Uproot() {
        GetComponentInParent<EnemyMandrake>().Uproot();
    }

    public void SelfDestruct() {
        GetComponentInParent<EnemyMandrake>().SelfDestruct();
    }

    public void PlayJumpSFX() {
        GetComponentInParent<EnemyMandrake>().PlayJumpSFX();
    }
}
