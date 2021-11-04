using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObjectController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Texture2D defaultCursor;
    public Texture2D hoverCursor;

    public void OnPointerClick(PointerEventData eventData) {
        Application.OpenURL("https://www.youtube.com/watch?v=RbwFaoIavCY&list=PLp8edfuXiFb8XDbPlaag54Sa1XJNAfcOw&ab_channel=KalechippsDevKalechippsDev");
    }

    public void OnPointerEnter(PointerEventData eventData) {
        Cursor.SetCursor(hoverCursor, Vector2.zero, CursorMode.Auto);
    }

    public void OnPointerExit(PointerEventData eventData) {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }


}
