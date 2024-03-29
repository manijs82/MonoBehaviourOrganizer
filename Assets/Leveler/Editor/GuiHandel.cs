using System;
using UnityEditor;
using UnityEngine;

namespace Leveler
{
    public class GuiHandel
    {
        public static event Action OnSwitchTabs;
        public event Action OnInputDown; 

        private LevelWindowData _windowData;
        private Action _repaintAction;
        private SerializedObject _so;
        private SerializedProperty _propOrientToNormal;

        private TabMode _tabMode;
        private PlacerTab _placerTab;
        private PrefabsTab _prefabsTab;

        public GuiHandel(LevelWindowData windowData, Action repaintAction, SerializedObject so)
        {
            _windowData = windowData;
            _repaintAction = repaintAction;
            _so = so;
            _propOrientToNormal = _so.FindProperty(nameof(_windowData.orientToNormal));
            _tabMode = (TabMode) EditorPrefs.GetInt("SelectedTab", 0);

            _placerTab = new PlacerTab(_windowData, _propOrientToNormal);
            _prefabsTab = new PrefabsTab(_windowData, _so);
        }

        public void OnGUI()
        {
            _so.Update();

            DrawTab();

            EditorGUI.BeginChangeCheck();
            _tabMode = (TabMode) EditorGUI.EnumPopup(new Rect(0, Screen.height - 50, Screen.width / 3f, 22), _tabMode);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("SelectedTab", (int) _tabMode);
                OnSwitchTabs?.Invoke();
            }

            _so.ApplyModifiedProperties();
            _repaintAction();

            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.C)
            {
                Event.current.Use();
                OnInputDown?.Invoke();
            }
        }

        private void DrawTab()
        {
            switch (_tabMode)
            {
                case TabMode.PrefabTab:
                    _prefabsTab.OnGui();
                    break;
                case TabMode.PlacerTab:
                    _placerTab.OnGui();
                    break;
            }
        }
    }

    public enum TabMode
    {
        PrefabTab,
        PlacerTab
    }
}