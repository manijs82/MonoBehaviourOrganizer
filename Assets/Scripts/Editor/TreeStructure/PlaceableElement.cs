using UnityEditor.TreeViewExamples;
using UnityEngine;


[System.Serializable]
public class PlaceableElement : TreeElement
{
    public GameObject gameObject;
    public bool isRootGo;

    public PlaceableElement(string name, int depth, int id, GameObject gameObject, bool isRootGo) :
        base(name, depth, id)
    {
        this.gameObject = gameObject;
        this.isRootGo = isRootGo;
    }
}