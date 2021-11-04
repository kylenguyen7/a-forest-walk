using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class BlackScreenController : MonoBehaviour
{
    public delegate void OnHalfwayPoint();
    public OnHalfwayPoint OnHalfwayPointCallback;

    [SerializeField] float fadeTime = 1f;
    [SerializeField] float hangTime = 0.3f;
    bool fadingIn = true;

    Image black;

    private void Awake() {
        black = GetComponent<Image>();
        var col = black.color;
        col.a = 0;
        black.color = col;
    }

    private void Start() {
        StartCoroutine(DoTransition());
    }

    IEnumerator DoTransition() {

        // Debug.Log("Transition fading in.");
        while(black.color.a < 1) {
            var col = black.color;
            col.a += Time.deltaTime / fadeTime;
            black.color = col;
            yield return new WaitForEndOfFrame();
        }

        // Debug.Log($"Transition waiting {hangTime} seconds.");
        if(OnHalfwayPointCallback != null) {
            OnHalfwayPointCallback.Invoke();
        }
        yield return new WaitForSeconds(hangTime);

        // Debug.Log("Transition fading out.");
        while (black.color.a > 0) {
            var col = black.color;
            col.a -= Time.deltaTime / fadeTime;
            black.color = col;
            yield return new WaitForEndOfFrame();
        }

        Destroy(black.gameObject);
    }
}
