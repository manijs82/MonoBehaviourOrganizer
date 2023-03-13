using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PrefabsTab : WindowTab
{
    private SerializedProperty _propPrefabs;
    private ReorderableList _validTypes;
    private ReflectedTypes<MonoBehaviour> _monos;

    public PrefabsTab(LevelWindowData windowData, SerializedObject serializedObject)
    {
        _monos = new ReflectedTypes<MonoBehaviour>();
        _propPrefabs = serializedObject.FindProperty(nameof(windowData.prefabs));
        _validTypes = new ReorderableList(serializedObject, serializedObject.FindProperty(nameof(windowData.validTypes)))  // windowData.validTypes, typeof(string)
        {
            list = windowData.validTypes,
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Valid Types"),
            onAddDropdownCallback = OpenContextMenu,
            drawElementCallback = (rect, index, _, _) => EditorGUI.LabelField(rect, windowData.validTypes[index])
        };
    }

    private void OpenContextMenu(Rect rect, ReorderableList list)
    {
        var menu = new GenericMenu();

        foreach (var typeName in _monos.TypesNames)
            menu.AddItem(new GUIContent(typeName), false, () =>
            {
                if (!list.list.Contains(typeName))
                    list.list.Add(typeName);
            });

        menu.ShowAsContext();
    }

    public override void OnGui()
    {
        _validTypes.DoLayoutList();
        EditorGUILayout.PropertyField(_propPrefabs);
    }
}