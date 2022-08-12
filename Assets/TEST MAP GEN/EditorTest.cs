using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapGenerator))]
public class EditorTest : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MapGenerator mapGen = (MapGenerator)target;

        if(GUILayout.Button("Generate Dungeon"))
        {
            mapGen.generateDungeon();
        }
    }
}
