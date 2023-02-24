using System.Linq;
using UnityEditor;
using UnityEngine;

public class LevelWindow : EditorWindow
{
    private LevelWindowData _windowData;

    private SerializedObject _so;

    private SceneViewHandel _sceneViewHandel;
    private GuiHandel _guiHandel;
    private PlaceableGroup _placeableGroup;
    private Transform _currentParent;
    private Vector2 _scrollPos;

    [MenuItem("Tools/Leveler")]
    private static void CreateWindow() =>
        GetWindow<LevelWindow>("Leveler").Show();

    private void OnEnable()
    {
        if (GetWindowData()) return;

        InitSo();
        InitHandel();
    }

    private void InitHandel()
    {
        _sceneViewHandel = new SceneViewHandel();
        _guiHandel = new GuiHandel(_windowData, _placeableGroup, _currentParent, Repaint, _so);
        _sceneViewHandel.OnInputDown += PlacePrefab;
        _guiHandel.OnInputDown += TryPlacePrefab;
    }

    private void OnDisable()
    {
        _guiHandel.OnInputDown -= TryPlacePrefab;
        _sceneViewHandel.OnInputDown -= PlacePrefab;
        _sceneViewHandel.Disable();
    }

    private void InitSo()
    {
        _so = new SerializedObject(_windowData);

        InitGroups();
    }

    private bool GetWindowData()
    {
        if (_windowData == null)
        {
            _windowData = AssetDatabase.LoadAssetAtPath<LevelWindowData>("Assets/WindowData.asset");
            if (_windowData != null)
            {
                InitSo();
                InitHandel();
                return true;
            }

            _windowData = CreateInstance<LevelWindowData>();
            AssetDatabase.CreateAsset(_windowData, "Assets/WindowData.asset");
            AssetDatabase.Refresh();
        }

        return false;
    }

    private void InitGroups()
    {
        var objs = FindObjectsOfType<PlaceableObject>().ToList();
        _placeableGroup = new PlaceableGroup();
        foreach (var placeableObject in objs)
            _placeableGroup.AddObject(placeableObject);
    }

    private void PlacePrefab(RaycastHit hit)
    {
        if (_windowData.prefabs == null) return;
        var prefab = _windowData.prefabs[_windowData.selectedPrefabIndex];
        if (prefab == null) return;

        var go = PlacementUtility.PlacePrefab(prefab, _currentParent, hit);
        PlacementUtility.OrientObject(_windowData, go.transform, hit.normal);
        _placeableGroup.AddObject(go);
    }
    
    private void TryPlacePrefab()
    {
        if(!PlacementUtility.Raycast(_sceneViewHandel.SceneCam, out RaycastHit hit)) return;
        
        if (_windowData.prefabs == null) return;
        var prefab = _windowData.prefabs[_windowData.selectedPrefabIndex];
        if (prefab == null) return;

        var go = PlacementUtility.PlacePrefab(prefab, _currentParent, hit);
        PlacementUtility.OrientObject(_windowData, go.transform, hit.normal);
        _placeableGroup.AddObject(go);
    }

    private void OnGUI()
    {
        _guiHandel.OnGUI();
    }
}