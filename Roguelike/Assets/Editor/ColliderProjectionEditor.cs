using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ColliderProjection))]
public class ColliderProjectionEditor : Editor
{
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        // Refuse to delete this >:)

        /*
        ColliderProjection projection = target as ColliderProjection;
        LayerMask myLayerMask = projection.collisionLayers;
        LayerMask tempMask = EditorGUILayout.MaskField(InternalEditorUtility.LayerMaskToConcatenatedLayersMask(myLayerMask), InternalEditorUtility.layers);
        projection.collisionLayers = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
        */
    }
}
