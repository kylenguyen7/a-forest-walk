using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHPBarController : MonoBehaviour
{
    [HideInInspector]
    public float ratio = 1f;
    [SerializeField]
    float lerpPerFrame = 0.1f;

    SpriteRenderer myBar;
    float startWidth;
    float currentWidth;

    [SerializeField] SpriteRenderer parentSprite;

    private void Start() {
        myBar = GetComponent<SpriteRenderer>();
        startWidth = myBar.size.x;
        currentWidth = startWidth;
    }


    // Update is called once per frame
    void Update()
    {
        currentWidth = Mathf.Lerp(currentWidth, ratio * startWidth, lerpPerFrame);
        if (currentWidth < 0.01 * startWidth) currentWidth = 0;

        myBar.size = new Vector2(currentWidth, myBar.size.y);

        var col = myBar.color;
        col.a = parentSprite.color.a;
        myBar.color = col;
    }
}
