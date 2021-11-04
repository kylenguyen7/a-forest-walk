using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTargetControllerManual : MonoBehaviour
{
    #region singleton
    public static CameraTargetControllerManual instance;
    private void Awake() {
        instance = this;
    }
    #endregion

    Transform focus;
    Vector2 myOffset;

    [SerializeField] PolygonCollider2D startingRoom;

    private void Start() {
        focus = PlayerController.instance.transform;
        SetBounds(startingRoom);
    }

    void Update()
    {
        // Vector2 toTarget = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - focus.position);
        transform.position = (Vector2)focus.position + myOffset;
    }

    public void ResetFocus() {
        focus = PlayerController.instance.transform;
        myOffset = Vector2.zero;
    }

    public void SetFocus(Transform target, Vector2 offset) {
        focus = target;
        myOffset = offset;
    }

    public void SetBounds(PolygonCollider2D bounds) {
        var confiner = FindObjectOfType<CinemachineConfiner>();
        confiner.m_BoundingShape2D = bounds;
        confiner.InvalidatePathCache();
    }
}
