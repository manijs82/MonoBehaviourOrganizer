using UnityEditor.Search;
using UnityEngine;

public class ParentPaths
{
    private Tree<GameObjectPath> _tree;

    public ParentPaths()
    {
        var rootItem = new GameObjectPath(null, "Level");
        _tree = new Tree<GameObjectPath>(new Node<GameObjectPath>(rootItem));
    }

    public void AddPath(GameObject go)
    {
        var path = SearchUtils.GetHierarchyPath(go);
        var subPaths = path.Split("/");
        for (int i = 0; i < subPaths.Length - 1; i++)
        {
            
        }
    }
}

public class GameObjectPath
{
    public int Id;
    public GameObject GameObject;
    public string Path;

    public GameObjectPath(GameObject gameObject, string path)
    {
        Id = gameObject.GetInstanceID();
        GameObject = gameObject;
        Path = path;
    }
}