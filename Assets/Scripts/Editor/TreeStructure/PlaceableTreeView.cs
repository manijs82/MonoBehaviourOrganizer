using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

public class PlaceableTreeView : TreeView
{
    public PlaceableTreeView(TreeViewState state) : base(state)
    {
        Reload();
    }

    protected override TreeViewItem BuildRoot()
    {
        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };
        var allItems = new List<TreeViewItem>
        {
            new() { id = 1, depth = 0, displayName = "Animals" },
            new() { id = 2, depth = 1, displayName = "Mammals" },
            new() { id = 3, depth = 2, displayName = "Tiger" },
            new() { id = 4, depth = 2, displayName = "Elephant" },
            new() { id = 5, depth = 2, displayName = "Okapi" },
            new() { id = 6, depth = 2, displayName = "Armadillo" },
            new() { id = 7, depth = 1, displayName = "Reptiles" },
            new() { id = 8, depth = 2, displayName = "Crocodile" },
            new() { id = 9, depth = 2, displayName = "Lizard" }
        };

        SetupParentsAndChildrenFromDepths(root, allItems);

        return root;
    }
}