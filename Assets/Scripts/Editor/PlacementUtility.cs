using UnityEditor;
using UnityEngine;

public static class PlacementUtility
{
    public static PlaceableObject PlacePrefab(PlaceableObject prefab, Transform parent, RaycastHit hit)
    {
        var go = (PlaceableObject)PrefabUtility.InstantiatePrefab(prefab);
        go.transform.position = hit.point;
        if (parent != null)
            go.transform.SetParent(parent);

        Undo.RegisterCreatedObjectUndo(go.gameObject, "spawn prefab");
        return go;
    }

    public static void OrientObject(LevelWindowData settings, Transform transform, Vector3 normal)
    {
        if (settings.orientToNormal)
            transform.rotation = Quaternion.LookRotation(normal);
    }

    public static bool Raycast(out RaycastHit hit)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        return Physics.Raycast(ray, out hit, 25);
    }
}