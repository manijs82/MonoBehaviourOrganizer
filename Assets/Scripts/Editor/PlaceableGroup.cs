using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;


public class PlaceableGroup
{
    public readonly Dictionary<Transform, List<PlaceableObject>> Groups;
    public readonly List<PlaceableObject> NoParentObjects;
    public readonly List<bool> ShowList;

    public PlaceableGroup()
    {
        NoParentObjects = new List<PlaceableObject>();
        Groups = new Dictionary<Transform, List<PlaceableObject>>();
        ShowList = new List<bool>();
    }

    public void AddObject(PlaceableObject obj)
    {
        if (!PrefabUtility.IsOutermostPrefabInstanceRoot(obj.gameObject)) return;
        if (obj.transform.parent == null)
        {
            NoParentObjects.Add(obj);
            return;
        }

        var parent = PrefabUtility.GetOutermostPrefabInstanceRoot(obj).transform.parent;
        if (parent == null)
            parent = obj.transform.root;
        if (Groups.ContainsKey(parent))
        {
            Groups[parent].Add(obj);
        }
        else
        {
            Groups.Add(parent, new List<PlaceableObject>());
            ShowList.Add(true);
            Groups[parent].Add(obj);
        }
    }

    public void RemoveObject(PlaceableObject obj)
    {
        if (NoParentObjects.Contains(obj))
        {
            NoParentObjects.Remove(obj);
            return;
        }

        foreach (var parent in Groups.Keys)
        {
            if (!Groups[parent].Contains(obj)) continue;
            Groups[parent].Remove(obj);
            return;
        }
    }

    public void OnGui()
    {
        EditorGUILayout.Space();

        EditorGUILayout.Space();
        DrawObjectsGroup(NoParentObjects);

        int i = 0;
        foreach (var group in Groups)
        {
            if (group.Key == null || group.Value.Count == 0) continue;

            EditorGUILayout.Space();
            ShowList[i] =
                EditorGUILayout.Foldout(ShowList[i], group.Key.gameObject.name);
            if (ShowList[i])
                DrawObjectsGroup(group.Value);
            i++;
        }
    }

    private void DrawObjectsGroup(List<PlaceableObject> objectList)
    {
        var objToRemove = new List<PlaceableObject>();
        foreach (var obj in objectList)
        {
            if (obj == null)
            {
                objToRemove.Add(obj);
                continue;
            }

            using (new EditorGUILayout.HorizontalScope())
            {
                DrawObjectDetail(obj);
            }
        }

        foreach (var obj in objToRemove)
        {
            RemoveObject(obj);
        }
    }

    private static void DrawObjectDetail(PlaceableObject obj)
    {
        GUI.enabled = false;
        //EditorGUILayout.ObjectField(obj, obj.GetType());
        EditorGUILayout.LabelField(SearchUtils.GetHierarchyPath(obj.gameObject, false));
        GUI.enabled = true;
        if (GUILayout.Button("Delete"))
        {
            Undo.DestroyObjectImmediate(obj.gameObject);
        }

        if (GUILayout.Button("Select")) Selection.activeGameObject = obj.gameObject;
        if (GUILayout.Button("Focus"))
        {
            Selection.activeGameObject = obj.gameObject;
            SceneView.FrameLastActiveSceneView();
        }
    }
}