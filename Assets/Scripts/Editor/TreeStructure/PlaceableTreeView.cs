using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaceableTreeView : TreeView
{
    private ParentPaths _paths;
    private int lastId = 1;

    public PlaceableTreeView(TreeViewState state) : base(state)
    {
        Reload();
    }

    protected override TreeViewItem BuildRoot()
    {
        return new TreeViewItem { id = 0, depth = -1 };
    }

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
            lastId++;
            var treeViewItem = new TreeViewItem { id = lastId, displayName = tree.Name };
            root.AddChild(treeViewItem);
            rows.Add(treeViewItem);
            SetupTree(tree.Parents, treeViewItem, rows);
        }
    }
}