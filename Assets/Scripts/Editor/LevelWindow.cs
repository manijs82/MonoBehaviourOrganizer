using System;
using UnityEditor;
using UnityEngine;

public class LevelWindow : EditorWindow
{
    public static event Action HierarchyChange;
    public static Transform Parent;
    
    private LevelWindowData _windowData;
    private SerializedObject _so;
    private SceneViewHandel _sceneViewHandel;
    private GuiHandel _guiHandel;


    [MenuItem("Tools/Leveler")]
    private static void CreateWindow() =>
        GetWindow<LevelWindow>("Leveler").Show();

    private void OnEnable()
    {
        if (GetWindowData()) return;

        InitSo();
        InitHandel();
    }

    private void InitSo()
    {
        _so = new SerializedObject(_windowData);
    }

    private void InitHandel()
    {
        _sceneViewHandel = new SceneViewHandel();
        _guiHandel = new GuiHandel(_windowData, Repaint, _so);
        _sceneViewHandel.OnInputDown += PlacePrefab;
        _guiHandel.OnInputDown += TryPlacePrefab;
    }

    private void OnDisable()
    {
        _guiHandel.OnInputDown -= TryPlacePrefab;
        _sceneViewHandel.OnInputDown -= PlacePrefab;
        _sceneViewHandel.Disable();
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

    private void PlacePrefab(RaycastHit hit)
    {
        if (_windowData.prefabs == null) return;
        var prefab = _windowData.prefabs[_windowData.selectedPrefabIndex];
        if (prefab == null) return;

        var go = PlacementUtility.PlacePrefab(prefab, Parent, hit);
        PlacementUtility.OrientObject(_windowData, go.transform, hit.normal);
    }

    private void TryPlacePrefab()
    {
        SceneView.lastActiveSceneView.Focus();
        _sceneViewHandel.ReceiveInputFromOtherWindow();
    }

    private void OnGUI()
    {
        _guiHandel.OnGUI();
    }
    
    void OnHierarchyChange()
    {
        HierarchyChange?.Invoke();
        Repaint();
    }
}