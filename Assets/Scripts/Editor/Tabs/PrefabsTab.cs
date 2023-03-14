using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

public class PrefabsTab : WindowTab
{
    private LevelWindowData _windowData;
    private ReorderableList _validTypes;
    private ReorderableList _prefabs;
    private ReflectedTypes<MonoBehaviour> _monos;

    public PrefabsTab(LevelWindowData windowData, SerializedObject serializedObject)
    {
        _windowData = windowData;
        _monos = new ReflectedTypes<MonoBehaviour>();
        var propPrefabs = serializedObject.FindProperty(nameof(windowData.prefabs));
        var propValidTypes = serializedObject.FindProperty(nameof(windowData.validTypes));
        _validTypes = new ReorderableList(serializedObject, propValidTypes)
        {
            list = windowData.validTypes,
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Valid Types"),
            onAddDropdownCallback = OpenContextMenu,
            onRemoveCallback = list =>
            {
                ReorderableList.defaultBehaviours.DoRemoveButton(list);
                UpdatePrefabsList();
                Debug.Log("VAR");
            },
            drawElementCallback = (rect, index, _, _) => EditorGUI.LabelField(rect, windowData.validTypes[index])
        };

        _prefabs = new ReorderableList(serializedObject, propPrefabs)
        {
            list = windowData.prefabs,
            drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Prefabs"),
            displayAdd = false,
            drawElementCallback = (rect, index, _, _) =>
            {
                GUI.enabled = false;
                EditorGUI.ObjectField(rect, windowData.prefabs[index], windowData.prefabs[index].GetType());
                GUI.enabled = true;
            }
        };
    }

    private void OpenContextMenu(Rect rect, ReorderableList list)
    {
        var menu = new GenericMenu();

        foreach (var typeName in _monos.TypesNames)
            menu.AddItem(new GUIContent(typeName), false, () =>
            {
                if (!list.list.Contains(typeName))
                {
                    list.list.Add(typeName);
                    UpdatePrefabsList();
                }
            });

        menu.ShowAsContext();
    }

    private void UpdatePrefabsList()
    {
        _prefabs.list.Clear();
        var prefabs = GetPrefabsOfType(_monos.GetTypeFromString(_windowData.validTypes[0]));
        foreach (var prefab in prefabs)
            _prefabs.list.Add(prefab);
    }

    private List<GameObject> GetPrefabsOfType(Type type)
    {
        var guids = AssetDatabase.FindAssets("t:Prefab");
        var prefabs = new List<GameObject>();
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var toCheck = AssetDatabase.LoadMainAssetAtPath(path);

            var go = toCheck as GameObject;
            if (go == null)
                continue;

            var comp = go.GetComponent(type);
            if (comp != null)
            {
                prefabs.Add(go);
            }
            else
            {
                var comps = go.GetComponentsInChildren(type);
                if (comps.Length > 0)
                {
                    prefabs.Add(go);
                }
            }
        }

        return prefabs;
    }

    public override void OnGui()
    {
        _validTypes.DoLayoutList();
        EditorGUILayout.Space();
        _prefabs.DoLayoutList();
    }
}