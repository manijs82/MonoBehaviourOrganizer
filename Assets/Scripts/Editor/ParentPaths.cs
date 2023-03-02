using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ParentPaths
{
    public List<ParentTree> Tree;
    private Dictionary<string, ParentTree> _objectsByPath;

    public ParentPaths()
    {
        Tree = new List<ParentTree>();
        _objectsByPath = new Dictionary<string, ParentTree>();
    }

    public List<ParentTree> AddObjects(List<PlaceableObject> gos)
    {
        foreach (var go in gos) 
            AddObject(go.gameObject);

        ShowFolders(Tree);
        return Tree;
    }

    public void AddObject(GameObject go)
    {
        var path = SearchUtils.GetHierarchyPath(go, false);
        path = path.Remove(0, 1);
        var paths = path.Split("/");
        var lastPath = paths[0];
        lastPath += "/";
        for (int i = 0; i < paths.Length; i++)
        {
            if (lastPath.EndsWith("/"))
            {
                AddParentTree(lastPath);
            }
            else
            {
                AddObjectToParent(go, lastPath);
            }

            if (i < paths.Length - 2)
                lastPath += paths[i + 1] + "/";
        }
    }

    private void AddObjectToParent(GameObject go, string objectPath)
    {
        if (_objectsByPath.TryGetValue(objectPath, out var parent)) 
            parent.Children.Add(go);
    }

    private ParentTree AddParentTree(string objectPath)
    {
        if (!_objectsByPath.TryGetValue(objectPath, out var folder))
        {
            var folderPathWithoutEndSlash = objectPath.TrimEnd('/');
            var lastSlashPosition = folderPathWithoutEndSlash.LastIndexOf("/");
            List<ParentTree> folders;
            string folderName;
            if (lastSlashPosition < 0)
            {
                folderName = folderPathWithoutEndSlash;
                folders = Tree;
            }
            else
            {
                var parentFolderPath = objectPath.Substring(0, lastSlashPosition + 1);
                folders = _objectsByPath[parentFolderPath].Parents;
                folderName = folderPathWithoutEndSlash.Substring(lastSlashPosition + 1);
            }

            folder = new ParentTree
            {
                Name = folderName
            };
            folders.Add(folder);
            _objectsByPath.Add(objectPath, folder);
        }

        return folder;
    }

    private static void ShowFolders(List<ParentTree> folders)
    {
        foreach (var folder in folders)
        {
            ShowFolder(folder, 0);
        }
    }

    private static void ShowFolder(ParentTree parentTree, int indentation)
    {
        string folderIndentation = new string(' ', indentation);
        Debug.Log($"{folderIndentation}-{parentTree.Name}");

        foreach (var subfolder in parentTree.Parents)
        {
            ShowFolder(subfolder, indentation + 2);
        }
    }
}

public class ParentTree
{
    public string Name { get; set; }
    public List<ParentTree> Parents { get; set; } = new();
    public List<GameObject> Children { get; set; } = new();
}