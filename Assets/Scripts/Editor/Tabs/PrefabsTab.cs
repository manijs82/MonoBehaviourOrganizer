using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PrefabsTab : WindowTab
{
    private SerializedProperty _propPrefabs;
    private ReorderableList _typesList;
    private ReflectedTypes<MonoBehaviour> _monos;
    private List<string> _validTypes;

    public PrefabsTab(SerializedProperty propPrefabs)
    {
        _propPrefabs = propPrefabs;
        _monos = new ReflectedTypes<MonoBehaviour>();
        _validTypes = new List<string>();
        _typesList = new ReorderableList(_validTypes, typeof(string))
        {
            onAddCallback = list => list.list.Add("value")
        };
    }
    public override void OnGui()
    {
        _typesList.DoLayoutList();
        EditorGUILayout.PropertyField(_propPrefabs);
    }
}