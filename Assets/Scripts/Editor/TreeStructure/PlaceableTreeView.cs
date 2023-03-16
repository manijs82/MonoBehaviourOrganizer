using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlaceableTreeView : TreeView
{
    private ParentPaths _paths;
    private List<Type> _validTypes;
    private List<GameObject> _currentList;
    private bool[] _objectListFoldouts;
    private Dictionary<GameObject, Component[]> _objectComponents;
    private Dictionary<GameObject, bool[]> _objectComponentFoldouts;
    private IList<int> _selectedIds;
    private Vector2 _scrollPos;

    public PlaceableTreeView(TreeViewState state, List<string> validTypeNames) : base(state)
    {
        var monos = new ReflectedTypes<MonoBehaviour>();
        _validTypes = new List<Type>();
        foreach (var typeName in validTypeNames) 
            _validTypes.Add(monos.GetTypeFromString(typeName));
        _currentList = new List<GameObject>();
        _objectComponents = new Dictionary<GameObject, Component[]>();
        _objectComponentFoldouts = new Dictionary<GameObject, bool[]>();
        Reload();
        
        LevelWindow.HierarchyChange += Reload;
        GuiHandel.OnSwitchTabs += () =>
        {
            _validTypes.Clear();
            foreach (var typeName in validTypeNames) 
                _validTypes.Add(monos.GetTypeFromString(typeName));
            Reload();
        };
    }
    
    protected override bool CanMultiSelect(TreeViewItem item) => false;

    protected override TreeViewItem BuildRoot()
    {
        return new TreeViewItem { id = 0, depth = -1 };
    }

    protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
    {
        var rows = GetRows() ?? new List<TreeViewItem>(200);

        var objs = new List<Object>();
        foreach (var type in _validTypes) 
            objs.AddRange(Object.FindObjectsOfType(type));
        _paths ??= new ParentPaths();
        if (!_paths.AddObjects(objs))
            return rows;

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
            if (gameObject == null || !HasValidType(gameObject))
            {
                objectsToRemove.Add(gameObject);
                continue;
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawGameObject(gameObject, i);
            }

            if (_objectListFoldouts[i] && _objectComponents.ContainsKey(gameObject))
            {
                EditorGUI.indentLevel++;
                int compIndex = 0;
                foreach (var component in _objectComponents[gameObject])
                {
                    _objectComponentFoldouts[gameObject][compIndex] =
                        EditorGUILayout.Foldout(_objectComponentFoldouts[gameObject][compIndex],
                            component.GetPrettyName());

                    if (_objectComponentFoldouts[gameObject][compIndex])
                    {
                        var methods = component.GetMethods();
                        var props = component.GetProperties();
                        foreach (var method in methods) LevlerGUILayout.MethodGui(component, method);
                        foreach (var prop in props) LevlerGUILayout.PropertyGui(component, prop);
                    }

                    compIndex++;
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            i++;
        }

        foreach (var gameObject in objectsToRemove)
        {
            if (_objectComponents.ContainsKey(gameObject)) _objectComponents.Remove(gameObject);
            if (_objectComponentFoldouts.ContainsKey(gameObject)) _objectComponentFoldouts.Remove(gameObject);
            _currentList.Remove(gameObject);
        }
    }

    private bool HasValidType(GameObject gameObject)
    {
        foreach (var type in _validTypes)
        {
            if (gameObject.GetComponent(type) != null)
                return true;
        }

        return false;
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
                var components = gameObject.GetComponents<MonoBehaviour>();
                var validComponents = new List<Component>();

                foreach (var component in components)
                {
                    var methods = component.GetMethods();
                    var props = component.GetProperties();
                    if (methods.HasAttribute() || props.HasAttribute())
                        validComponents.Add(component);
                }

                var componentArray = validComponents.ToArray();
                _objectComponents.TryAdd(gameObject, componentArray);
                _objectComponentFoldouts.TryAdd(gameObject, new bool[componentArray.Length]);
            }
            else
            {
                if (_objectComponents.ContainsKey(gameObject)) _objectComponents.Remove(gameObject);
                if (_objectComponentFoldouts.ContainsKey(gameObject)) _objectComponentFoldouts.Remove(gameObject);
            }
        }

        GUI.enabled = true;
        if (GUILayout.Button("Delete"))
        {
            if (_objectComponents.ContainsKey(gameObject)) _objectComponents.Remove(gameObject);
            if (_objectComponentFoldouts.ContainsKey(gameObject)) _objectComponentFoldouts.Remove(gameObject);
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