using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RespManager : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler,
    IPointerClickHandler
{
    public ResponseObject myResponse;
    TextMeshProUGUI myText;

    public Color textDefault;
    public Color textMouseOver;

    public Image mouseOverImage;

    AudioSource mySFX;
    public AudioClip mouseOverSFX;

    public int index;

    private void Awake() {
        myText = GetComponentInChildren<TextMeshProUGUI>();
        if (myText == null) {
            Debug.LogError("Found ResponseObject without TMP Text child!");
        }

        myText.color = textDefault;
        mySFX = GetComponentInChildren<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(myText.text != myResponse.response) {
            myText.text = myResponse.response;
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        Textbox.instance.ReceiveResponse(index);
    }

    public void OnPointerEnter(PointerEventData eventData) {
        mySFX.PlayOneShot(mouseOverSFX);

        myText.color = textMouseOver;

        var col = mouseOverImage.color;
        col.a = 1;
        mouseOverImage.color = col;
    }

    public void OnPointerExit(PointerEventData eventData) {

        myText.color = textDefault;

        var col = mouseOverImage.color;
        col.a = 0;
        mouseOverImage.color = col;
    }

}
