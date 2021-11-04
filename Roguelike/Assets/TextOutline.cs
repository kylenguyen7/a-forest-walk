using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextOutline : MonoBehaviour {
    [SerializeField] TextMeshPro parentText;
    TextMeshPro text;

    private void Start() {
        text = GetComponent<TextMeshPro>();

        if (text == null || parentText == null) {
            Debug.LogError($"There was an error finding components in {gameObject.name}");
        }

        text.text = parentText.text;
    }
}
