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

    public static bool Raycast(Camera cam, out RaycastHit hit)
    {
        var ray = cam.ScreenPointToRay(MousePos());
        return Physics.Raycast(ray, out hit, 25);
    }

    public static Vector3 MousePos()
    {
        return new Vector3(Event.current.mousePosition.x, Screen.height - Event.current.mousePosition.y);
    }
}