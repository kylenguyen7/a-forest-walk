using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlimeTimer : MonoBehaviour
{
    EnemySlime mySlime;

    // Start is called before the first frame update
    void Start()
    {
        mySlime = GetComponentInParent<EnemySlime>();
        if(mySlime == null) {
            Debug.LogError("An EnemySlimeTimer could not find the associated EnemySlime");
        }
    }

    public void Takeoff() {
        if (mySlime == null) return;
        mySlime.OnTakeoff();
    }

    public void Land() {
        if (mySlime == null) return;
        mySlime.OnLanding();
    }

    public void PreTakeoff() {
        if (mySlime == null) return;
        mySlime.PreTakeoff();
    }
}
