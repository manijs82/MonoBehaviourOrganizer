using System;
using UnityEditor;
using UnityEngine;

public class SceneViewHandel
{
    public event Action<RaycastHit> OnInputDown;
    
    private bool _canInvokeInput;

    public Camera SceneCam
    {
        get;
        private set;
    }

    public SceneViewHandel()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    public void Disable() => SceneView.duringSceneGui -= DuringSceneGUI;

    private void DuringSceneGUI(SceneView scene)
    {
        if (Event.current.type == EventType.MouseMove)
            scene.Repaint();

        SceneCam = scene.camera;
        if (PlacementUtility.Raycast(SceneCam, out RaycastHit hitInfo))
        {
            DrawDisc(hitInfo);
            _canInvokeInput = CheckInput();

            if (_canInvokeInput)
            {
                _canInvokeInput = false;
                OnInputDown?.Invoke(hitInfo);
            }
        }
    }

    private bool CheckInput()
    {
        if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
        {
            Event.current.Use();
            return true;
        }

        return false;
    }

    private static void DrawDisc(RaycastHit hitInfo)
    {
        Handles.color = Color.black;
        Handles.DrawSolidDisc(hitInfo.point, hitInfo.normal, .2f);
    }
}