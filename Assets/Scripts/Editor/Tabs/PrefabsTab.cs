using UnityEditor;

public class PrefabsTab : WindowTab
{
    private SerializedProperty _propPrefabs;

    public PrefabsTab(SerializedProperty propPrefabs)
    {
        _propPrefabs = propPrefabs;
    }

    public override void OnGui()
    {
        EditorGUILayout.PropertyField(_propPrefabs);
    }
}