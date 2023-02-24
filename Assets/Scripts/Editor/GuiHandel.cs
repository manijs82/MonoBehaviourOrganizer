using System;
using UnityEditor;
using UnityEngine;


public class GuiHandel
{
    public event Action OnInputDown; 

    private LevelWindowData _windowData;
    private PlaceableGroup _placeableGroup;
    private Transform _currentParent;
    private Action _repaintAction;
    private SerializedObject _so;
    private SerializedProperty _propPrefabs;
    private SerializedProperty _propOrientToNormal;
    private Vector2 _scrollPos;

    public GuiHandel(LevelWindowData windowData, PlaceableGroup placeableGroup, Transform currentParent,
        Action repaintAction, SerializedObject so)
    {
        _windowData = windowData;
        _placeableGroup = placeableGroup;
        _currentParent = currentParent;
        _repaintAction = repaintAction;
        _so = so;
        _propPrefabs = _so.FindProperty(nameof(_windowData.prefabs));
        _propOrientToNormal = _so.FindProperty(nameof(_windowData.orientToNormal));
    }

    public void OnGUI()
    {
        _so.Update();

        EditorGUILayout.PropertyField(_propPrefabs);
        EditorGUILayout.PropertyField(_propOrientToNormal);
        _currentParent = (Transform)EditorGUILayout.ObjectField("Parent", _currentParent, typeof(Transform), true);
        EditorGUILayout.LabelField("Choose a Prefab from the List and Press C to Place");
        DrawPrefabSelectors();

        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true));
        _placeableGroup.OnGui();
        EditorGUILayout.EndScrollView();

        _so.ApplyModifiedProperties();
        _repaintAction();

        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
        {
            Event.current.Use();
            OnInputDown?.Invoke();
        }
    }

    private void DrawPrefabSelectors()
    {
        EditorGUILayout.Space();
        if (_windowData.prefabs == null) return;
        using (new EditorGUILayout.HorizontalScope())
        {
            foreach (var prefab in _windowData.prefabs)
            {
                if (prefab == null) continue;
                GUIStyle style = new(GUI.skin.GetStyle("Button"));
                if (_windowData.selectedPrefabIndex == _windowData.prefabs.IndexOf(prefab))
                    style.fontStyle = FontStyle.BoldAndItalic;

                if (GUILayout.Button(prefab.name, style))
                    _windowData.selectedPrefabIndex = _windowData.prefabs.IndexOf(prefab);
            }
        }
    }
}