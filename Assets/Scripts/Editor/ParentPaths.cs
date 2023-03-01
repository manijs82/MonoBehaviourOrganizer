using System;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;

public class ParentPaths
{
    private Tree<GameObjectPath> _tree;

    public ParentPaths()
    {
        var rootItem = new GameObjectPath(null, "");
        _tree = new Tree<GameObjectPath>(new Node<GameObjectPath>(rootItem));
    }

    public void AddPath(GameObject go)
    {
        var path = SearchUtils.GetHierarchyPath(go, false);
        path = path.Remove(0, 1);
        GetFoldersFormStrings(new List<string>() { path });
    }

    static List<Folder> GetFoldersFormStrings(List<string> strings)
    {
        var folders = new List<Folder>();
        strings.Sort(StringComparer.InvariantCultureIgnoreCase);
        var folderByPath = new Dictionary<string, Folder>();
        foreach (var str in strings)
        {
            var paths = str.Split("/");
            var lastPath = paths[0];
            lastPath += "/";
            for (int i = 0; i < paths.Length; i++)
            {
                if (lastPath.EndsWith("/")) // we have a folder
                {
                    EnsureFolder(folders, folderByPath, lastPath);
                }

                if (i < paths.Length - 2)
                    lastPath += paths[i + 1] + "/";
            }
        }

        ShowFolder(folders[0], 1);
        return folders;
    }

    private static Folder EnsureFolder(List<Folder> rootFolders, Dictionary<string, Folder> folderByPath,
        string folderPath)
    {
        if (!folderByPath.TryGetValue(folderPath, out var folder))
        {
            var folderPathWithoutEndSlash = folderPath.TrimEnd('/');
            var lastSlashPosition = folderPathWithoutEndSlash.LastIndexOf("/");
            List<Folder> folders;
            string folderName;
            if (lastSlashPosition < 0) // it's a first level folder
            {
                folderName = folderPathWithoutEndSlash;
                folders = rootFolders;
            }
            else
            {
                var parentFolderPath = folderPath.Substring(0, lastSlashPosition + 1);
                folders = folderByPath[parentFolderPath].Folders;
                folderName = folderPathWithoutEndSlash.Substring(lastSlashPosition + 1);
            }

            folder = new Folder
            {
                Name = folderName
            };
            folders.Add(folder);
            folderByPath.Add(folderPath, folder);
        }

        return folder;
    }

    private static void ShowFolders(List<Folder> folders)
    {
        foreach (var folder in folders)
        {
            ShowFolder(folder, 0);
        }
    }

    private static void ShowFolder(Folder folder, int indentation)
    {
        string folderIndentation = new string(' ', indentation);
        string fileIndentation = folderIndentation + "  ";
        Debug.Log($"{folderIndentation}-{folder.Name}");
        foreach (var file in folder.Files)
        {
            Debug.Log($"{fileIndentation}-{file.Name}");
        }

        foreach (var subfolder in folder.Folders)
        {
            ShowFolder(subfolder, indentation + 2);
        }
    }
}

public class Folder
{
    public string Name { get; set; }
    public List<Folder> Folders { get; set; } = new List<Folder>();
    public List<File> Files { get; set; } = new List<File>();
}

public class File
{
    public string Name { get; set; }
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