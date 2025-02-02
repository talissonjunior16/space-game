#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ColliderAutoFit))]
public class ColliderAutoFitEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Add a button to apply the collider
        ColliderAutoFit colliderAutoFit = (ColliderAutoFit)target;
        if (GUILayout.Button("Apply Collider"))
        {
            colliderAutoFit.ApplyCollider();
        }
    }
}
#endif