using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Leveler
{
    [System.Serializable]
    public class PlacerTab : WindowTab
    {
        [SerializeField] TreeViewState treeViewState;
        private PlaceableTreeView _treeView;
        private LevelWindowData _windowData;
        private SerializedProperty _propOrientToNormal;

        public PlacerTab(LevelWindowData windowData, SerializedProperty propOrientToNormal)
        {
            _windowData = windowData;
            _propOrientToNormal = propOrientToNormal;
        
            InitTreeView();
        }

        private void InitTreeView()
        {
            treeViewState ??= new TreeViewState();
            _treeView ??= new PlaceableTreeView(treeViewState, _windowData.validTypes);
        }

        public override void OnGui()
        {
            EditorGUILayout.PropertyField(_propOrientToNormal);
            LevelWindow.Parent = (Transform)EditorGUILayout.ObjectField("Parent", LevelWindow.Parent, typeof(Transform), true);
            EditorGUILayout.LabelField("Choose a Prefab from the List and Press C to Place");
            DrawPrefabSelectors();
        
            _treeView.OnGUI(new Rect(0, GUILayoutUtility.GetLastRect().y + 22, Screen.width, Screen.height));
        }
    
        private void DrawPrefabSelectors()
        {
            EditorGUILayout.Space();
            if (_windowData.prefabs == null) return;
            using (new EditorGUILayout.HorizontalScope())
            {
                foreach (var prefab in _windowData.prefabs)
                {
                    if (prefab == null) continue;
                    GUIStyle style = new(GUI.skin.GetStyle("Button"));
                    if (_windowData.selectedPrefabIndex == _windowData.prefabs.IndexOf(prefab))
                        style.fontStyle = FontStyle.BoldAndItalic;

                    if (GUILayout.Button(prefab.name, style))
                        _windowData.selectedPrefabIndex = _windowData.prefabs.IndexOf(prefab);
                }
            }
        }
    }
}