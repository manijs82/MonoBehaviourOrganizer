using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ParentPaths
{
    private List<ParentTree> _tree;
    private Dictionary<string, ParentTree> _objectsByPath;

    public ParentPaths()
    {
        _tree = new List<ParentTree>();
        _objectsByPath = new Dictionary<string, ParentTree>();
    }

    public void AddPath(GameObject go)
    {
        var path = SearchUtils.GetHierarchyPath(go, false);
        path = path.Remove(0, 1);
        GetFoldersFormStrings(new List<string>() { path });
    }

    private List<ParentTree> GetFoldersFormStrings(List<string> strings)
    {
        strings.Sort(StringComparer.InvariantCultureIgnoreCase);
        foreach (var str in strings)
        {
            var paths = str.Split("/");
            var lastPath = paths[0];
            lastPath += "/";
            for (int i = 0; i < paths.Length; i++)
            {
                if (lastPath.EndsWith("/")) // we have a folder
                {
                    AddToParentTree(lastPath);
                }

                if (i < paths.Length - 2)
                    lastPath += paths[i + 1] + "/";
            }
        }

        ShowFolders(_tree);
        return _tree;
    }

    private List<ParentTree> GetFoldersFormStrings(string path)
    {
        var paths = path.Split("/");
        var lastPath = paths[0];
        lastPath += "/";
        for (int i = 0; i < paths.Length; i++)
        {
            if (lastPath.EndsWith("/")) // we have a folder
            {
                AddToParentTree(lastPath);
            }

            if (i < paths.Length - 2)
                lastPath += paths[i + 1] + "/";
        }

        return _tree;
    }

    private ParentTree AddToParentTree(string objectPath)
    {
        if (!_objectsByPath.TryGetValue(objectPath, out var folder))
        {
            var folderPathWithoutEndSlash = objectPath.TrimEnd('/');
            var lastSlashPosition = folderPathWithoutEndSlash.LastIndexOf("/");
            List<ParentTree> folders;
            string folderName;
            if (lastSlashPosition < 0) // it's a first level folder
            {
                folderName = folderPathWithoutEndSlash;
                folders = _tree;
            }
            else
            {
                var parentFolderPath = objectPath.Substring(0, lastSlashPosition + 1);
                folders = _objectsByPath[parentFolderPath].Folders;
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
        string fileIndentation = folderIndentation + "  ";
        Debug.Log($"{folderIndentation}-{parentTree.Name}");

        foreach (var subfolder in parentTree.Folders)
        {
            ShowFolder(subfolder, indentation + 2);
        }
    }
}

public class ParentTree
{
    public string Name { get; set; }
    public List<ParentTree> Folders { get; set; } = new List<ParentTree>();
    public List<GameObject> Files { get; set; } = new List<GameObject>();
}

public class GameObjectPath
{
    public int Id;
    public GameObject GameObject;
    public string Path;

    public GameObjectPath(GameObject gameObject, string path)
    {
        if (gameObject != null)
            Id = gameObject.GetInstanceID();
        GameObject = gameObject;
        Path = path;
    }
}