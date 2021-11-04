using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scanner : MonoBehaviour
{
    bool scanned = false;

    private void Start() {
        StartCoroutine("Scan");
    }
    IEnumerator Scan() {
        yield return new WaitForSecondsRealtime(1);
        AstarPath.active.Scan();
    }
}
