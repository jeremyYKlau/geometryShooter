using System.Collections;
using UnityEngine;
using UnityEditor; //required to inherit editor

//the whole point of this class is so that the map actively changes during gameplay
[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor {

    public override void OnInspectorGUI()
    {
        MapGenerator map = target as MapGenerator;
        if (DrawDefaultInspector()) //used this insetad of base.OnInspectorGUI() because this uses a bool and only redraws if a value is changed
        {
            map.generateMap();
        } 

        if( GUILayout.Button("Generate Map"))
        {
            map.generateMap();
        }
    }
}
