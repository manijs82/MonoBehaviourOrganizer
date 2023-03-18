using System;
using UnityEditor;
using UnityEngine;

namespace Leveler
{
    public class SceneViewHandel
    {
        public event Action<RaycastHit> OnInputDown;
    
        private bool _canInvokeInput;

        public SceneViewHandel()
        {
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        public void Disable() => SceneView.duringSceneGui -= DuringSceneGUI;

        public void ReceiveInputFromOtherWindow()
        {
            _canInvokeInput = true;
        }

        private void DuringSceneGUI(SceneView scene)
        {
            if (Event.current.type == EventType.MouseMove)
                scene.Repaint();

            if (PlacementUtility.Raycast(out RaycastHit hitInfo))
            {
                DrawDisc(hitInfo);

                if (_canInvokeInput)
                {
                    _canInvokeInput = false;
                    OnInputDown?.Invoke(hitInfo);
                }
            
                _canInvokeInput = CheckInput();
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
}