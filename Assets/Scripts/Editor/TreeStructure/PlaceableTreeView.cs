using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditor.Search;
using UnityEngine;

public class PlaceableTreeView : TreeView
{
    private ParentPaths _paths;
    private List<GameObject> _currentList;

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
        _currentList = _paths.GetParentTreeById(selectedIds[0], _paths.Tree).children;
    }

    public override void OnGUI(Rect rect)
    {
        var treeRect = new Rect(rect.x, rect.y, rect.width / 3, rect.height);

        using (new GUILayout.HorizontalScope())
        {
            GUILayout.Space(rect.width / 3 + 3);

            using (new GUILayout.VerticalScope())
            {
                DrawList();
            }

            base.OnGUI(treeRect);
        }
    }

    private void DrawList()
    {
        foreach (var gameObject in _currentList)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUI.enabled = false;
                //EditorGUILayout.ObjectField(obj, obj.GetType());
                EditorGUILayout.LabelField(SearchUtils.GetHierarchyPath(gameObject, false));
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
    }
}