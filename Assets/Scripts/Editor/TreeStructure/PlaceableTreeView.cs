using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class PlaceableTreeView : TreeView
{
    private ParentPaths _paths;
    private List<GameObject> _currentList;
    private bool[] _objectListFoldouts;
    private Dictionary<GameObject, Component[]> _objectComponents;
    private Dictionary<GameObject, bool[]> _objectComponentFoldouts;
    private IList<int> _selectedIds;
    private Vector2 _scrollPos;

    public PlaceableTreeView(TreeViewState state) : base(state)
    {
        _currentList = new List<GameObject>();
        _objectComponents = new Dictionary<GameObject, Component[]>();
        _objectComponentFoldouts = new Dictionary<GameObject, bool[]>();
        Reload();
    }

    protected override TreeViewItem BuildRoot()
    {
        return new TreeViewItem { id = 0, depth = -1 };
    }

    protected override bool CanMultiSelect(TreeViewItem item) => false;


    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
        var rows = GetRows() ?? new List<TreeViewItem>(200);

        _paths = new ParentPaths();
        var objs = Object.FindObjectsOfType<PlaceableObject>().ToList();
        _paths.AddObjects(objs);

        rows.Clear();
        SetupTree(_paths.Tree, root, rows);
        SetupDepthsFromParentsAndChildren(root);

        if (_selectedIds != null)
            SetGameObjectList();

        return rows;
    }

    private void SetupTree(List<ParentTree> parents, TreeViewItem root, IList<TreeViewItem> rows)
    {
        foreach (var tree in parents)
        {
            var treeViewItem = new TreeViewItem { id = tree.id, displayName = tree.name };
            root.AddChild(treeViewItem);
            rows.Add(treeViewItem);
            SetupTree(tree.parents, treeViewItem, rows);
        }
    }

    protected override void SelectionChanged(IList<int> selectedIds)
    {
        base.SelectionChanged(selectedIds);
        _selectedIds = selectedIds;
        SetGameObjectList();
    }

    private void SetGameObjectList()
    {
        _currentList = _paths.GetParentTreeById(_selectedIds[0], _paths.Tree).children;
        _objectListFoldouts = new bool[_currentList.Count];
    }

    public override void OnGUI(Rect rect)
    {
        var treeRect = new Rect(rect.x, rect.y, rect.width / 3, rect.height);

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Space(rect.width / 3 + 3);

            using (new GUILayout.VerticalScope())
            {
                _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandWidth(true));
                DrawList();
                EditorGUILayout.EndScrollView();
            }

            base.OnGUI(treeRect);
        }
    }

    private void DrawList()
    {
        var objectsToRemove = new List<GameObject>();

        int i = 0;
        foreach (var gameObject in _currentList)
        {
            if (gameObject == null)
            {
                objectsToRemove.Add(gameObject);
                continue;
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawGameObject(gameObject, i);
            }

            if (_objectListFoldouts[i])
            {
                EditorGUI.indentLevel++;
                int compIndex = 0;
                foreach (var component in _objectComponents[gameObject])
                {
                    _objectComponentFoldouts[gameObject][compIndex] = 
                        EditorGUILayout.Foldout(_objectComponentFoldouts[gameObject][compIndex],
                            ObjectNames.NicifyVariableName(component.GetType().Name));
                    
                    if(_objectComponentFoldouts[gameObject][compIndex])
                    {
                        var methods = component.GetType()
                            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        var props = component.GetType()
                            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                        foreach (var method in methods) ReflectionGuiUtils.MethodGui(component, method);
                        foreach (var prop in props) ReflectionGuiUtils.PropertyGui(component, prop);
                    }

                    compIndex++;
                }
                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            i++;
        }

        foreach (var gameObject in objectsToRemove)
            _currentList.Remove(gameObject);
    }

    private void DrawGameObject(GameObject gameObject, int index)
    {
        GUI.enabled = false;

        EditorGUI.BeginChangeCheck();
        _objectListFoldouts[index] = EditorGUILayout.Foldout(_objectListFoldouts[index], gameObject.name);
        if (EditorGUI.EndChangeCheck())
        {
            if (_objectListFoldouts[index])
            {
                var components = gameObject.GetComponents<Component>();
                _objectComponents.Add(gameObject, components);
                _objectComponentFoldouts.Add(gameObject, new bool[components.Length]);
            }
            else
            {
                _objectComponents.Remove(gameObject);
                _objectComponentFoldouts.Remove(gameObject);
            }
        }

        GUI.enabled = true;
        if (GUILayout.Button("Delete"))
        {
            Undo.DestroyObjectImmediate(gameObject);
        }

        if (GUILayout.Button("Select")) Selection.activeGameObject = gameObject;
        if (GUILayout.Button("Focus"))
        {
            Selection.activeGameObject = gameObject;
            SceneView.FrameLastActiveSceneView();
        }
    }
}