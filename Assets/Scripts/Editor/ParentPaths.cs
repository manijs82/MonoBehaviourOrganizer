using System.Collections.Generic;
using System.Linq;
using UnityEditor.Search;
using UnityEngine;

public class ParentPaths
{
    public List<ParentTree> Tree;
    private Dictionary<string, ParentTree> _objectsByPath;
    private int _lastId = 1;

    public ParentPaths()
    {
        Tree = new List<ParentTree>();
        _objectsByPath = new Dictionary<string, ParentTree>();
    }

    public ParentTree GetParentTreeById(int id, List<ParentTree> roots)
    {
        foreach (var parentTree in roots)
        {
            if (parentTree.id == id)
                return parentTree;

            var tree = GetParentTreeById(id, parentTree.parents);
            if(tree != null)
                return tree;
        }

        return null;
    }

    public void AddObjects(List<PlaceableObject> gos)
    {
        gos = gos.OrderBy(g => g.transform.parent.gameObject.name).ToList();
        foreach (var go in gos)
            AddObject(go.gameObject);
    }

    private void AddObject(GameObject go)
    {
        var path = SearchUtils.GetHierarchyPath(go, false);
        path = path.Remove(0, 1);
        var paths = path.Split("/");
        var lastPath = paths[0];
        lastPath += "/";
        for (int i = 0; i < paths.Length; i++)
        {
            if(i == paths.Length - 1)
            {
                AddObjectToParent(go, lastPath);
            }
            if (lastPath.EndsWith("/"))
            {
                AddParentTree(lastPath);
            }
            

            if (i < paths.Length - 2)
                lastPath += paths[i + 1] + "/";
        }
    }

    private void AddObjectToParent(GameObject go, string objectPath)
    {
        if (_objectsByPath.TryGetValue(objectPath, out var parent))
            parent.children.Add(go);
    }

    private void AddParentTree(string objectPath)
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
                folders = _objectsByPath[parentFolderPath].parents;
                folderName = folderPathWithoutEndSlash.Substring(lastSlashPosition + 1);
            }

            _lastId++;
            folder = new ParentTree
            {
                name = folderName,
                id = _lastId
            };
            folders.Add(folder);
            _objectsByPath.Add(objectPath, folder);
        }
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
        Debug.Log($"{folderIndentation}-{parentTree.name}");

        foreach (var subfolder in parentTree.parents)
        {
            ShowFolder(subfolder, indentation + 2);
        }
    }
}

public class ParentTree
{
    public string name { get; set; }
    public int id { get; set; }
    public List<ParentTree> parents { get; set; } = new();
    public List<GameObject> children { get; set; } = new();
}