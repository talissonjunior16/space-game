#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridManager))]
public class GridManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridManager instance = (GridManager)target;
        if (GUILayout.Button("Generate Tiles"))
        {
            instance.GenerateTiles();
        }
    }
}
#endif
