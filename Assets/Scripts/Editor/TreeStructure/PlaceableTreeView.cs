using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Search;
using UnityEngine;

public class PlaceableTreeView : TreeView
{
    private ParentPaths _paths;
    private List<GameObject> _currentList;
    private IList<int> _selectedIds;
    private bool _showFoldout;
    private Vector2 _scrollPos;

    public PlaceableTreeView(TreeViewState state) : base(state)
    {
        _currentList = new List<GameObject>();
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
            _currentList = _paths.GetParentTreeById(_selectedIds[0], _paths.Tree).children;

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
        _currentList = _paths.GetParentTreeById(_selectedIds[0], _paths.Tree).children;
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
        foreach (var gameObject in _currentList)
        {
            if (gameObject == null)
            {
                objectsToRemove.Add(gameObject);
                continue;
            }

            using (new GUILayout.HorizontalScope())
            {
                DrawGameObject(gameObject);
            }

            if (_showFoldout)
            {
                var placeComponent = gameObject.GetComponents<Component>();
                foreach (var component in placeComponent)
                {
                    var methods = component.GetType()
                        .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                    foreach (var method in methods)
                    {
                        var parameters = method.GetParameters();
                        if (parameters.Length != 1) continue;

                        var param = parameters[0];
                        if (param.ParameterType == typeof(float))
                        {
                            EditorGUILayout.LabelField(method.Name);
                            method.Invoke(component, new object[] { EditorGUILayout.Slider(1, 0, 1) });
                        }
                        if (param.ParameterType == typeof(Vector3))
                        {
                            EditorGUILayout.LabelField(method.Name);
                            EditorGUILayout.Vector3Field(param.Name, Vector3.one);
                        }
                    }
                }
            }
        }

        foreach (var gameObject in objectsToRemove)
            _currentList.Remove(gameObject);
    }

    private void DrawGameObject(GameObject gameObject)
    {
        GUI.enabled = false;
        //EditorGUILayout.ObjectField(obj, obj.GetType());
        //EditorGUILayout.LabelField(SearchUtils.GetHierarchyPath(gameObject, false));

        _showFoldout = EditorGUILayout.Foldout(_showFoldout, SearchUtils.GetHierarchyPath(gameObject, false));

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