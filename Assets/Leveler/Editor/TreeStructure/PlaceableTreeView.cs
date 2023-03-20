using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Leveler
{
    public class PlaceableTreeView : TreeView
    {
        private ParentPaths _paths;
        private List<Type> _validTypes;
        private List<GameObject> _currentList;
        private bool[] _objectListFoldouts;
        private Dictionary<GameObject, Component[]> _objectComponents;
        private Dictionary<GameObject, bool[]> _objectComponentFoldouts;
        private IList<int> _selectedIds;
        private Vector2 _componentScrollPos;
        private Vector2 _hierarchyScrollPos;

        public PlaceableTreeView(TreeViewState state, List<string> validTypeNames) : base(state)
        {
            showBorder = true;
            var monos = new ReflectedTypes<MonoBehaviour>();
            _validTypes = new List<Type>();
            foreach (var typeName in validTypeNames)
                _validTypes.Add(monos.GetTypeFromString(typeName));
            _currentList = new List<GameObject>();
            _objectComponents = new Dictionary<GameObject, Component[]>();
            _objectComponentFoldouts = new Dictionary<GameObject, bool[]>();
            Reload();

            LevelWindow.HierarchyChange += Reload;
            GuiHandel.OnSwitchTabs += () =>
            {
                _validTypes.Clear();
                foreach (var typeName in validTypeNames)
                    _validTypes.Add(monos.GetTypeFromString(typeName));
                Reload();
            };
        }

        protected override bool CanMultiSelect(TreeViewItem item) => false;

        protected override TreeViewItem BuildRoot()
        {
            return new TreeViewItem { id = 0, depth = -1 };
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            var rows = GetRows() ?? new List<TreeViewItem>(200);

            var objs = GetSceneObjects();
            _paths ??= new ParentPaths();
            if (!_paths.AddObjects(objs))
                return rows;

            rows.Clear();
            SetupTree(_paths.Tree, root, rows);
            SetupDepthsFromParentsAndChildren(root);

            if (_selectedIds != null)
                SetGameObjectList();

            return rows;
        }

        private List<Object> GetSceneObjects()
        {
            var o = new List<Object>();
                
            Scene scene = SceneManager.GetSceneAt (0);
            var gameObjectRoots = scene.GetRootGameObjects ();
            foreach (var gameObject in gameObjectRoots)
            {
                FindObjectsOfTypeInOrder(gameObject.transform, o);
            }

            return o;
        }

        private void FindObjectsOfTypeInOrder(Transform root, List<Object> objs)
        {
            for (int i = 0; i < root.childCount; i++)
            {
                var go = root.GetChild(i);
                foreach (var type in _validTypes)
                {
                    var component = go.GetComponent(type);
                    if(component != null)
                    {
                        objs.Add(component);
                        break;
                    }
                }

                FindObjectsOfTypeInOrder(root.GetChild(i), objs);
            }
        }

        private void SetupTree(List<ParentTree> parents, TreeViewItem root, IList<TreeViewItem> rows)
        {
            foreach (var tree in parents)
            {
                var treeViewItem = new TreeViewItem { id = tree.id, displayName = tree.name };
                root.AddChild(treeViewItem);
                rows.Add(treeViewItem);
                SetupTree(tree.parents, treeViewItem, rows);
            }
        }

        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            _selectedIds = selectedIds;
            SetGameObjectList();
        }

        private void SetGameObjectList()
        {
            _currentList = _paths.GetParentTreeById(_selectedIds[0], _paths.Tree).children;
            _objectListFoldouts = new bool[_currentList.Count];
        }

        public override void OnGUI(Rect rect)
        {
            var treeRect = new Rect(rect.x, rect.y, rect.width / 3, rect.height);

            using (new GUILayout.HorizontalScope())
            {
                base.OnGUI(treeRect);
                
                GUILayout.Space(rect.width / 3 + 3);
                using (new GUILayout.VerticalScope())
                {
                    _componentScrollPos = EditorGUILayout.BeginScrollView(_componentScrollPos, GUILayout.ExpandWidth(true));
                    DrawList();
                    EditorGUILayout.EndScrollView();
                }
            }
        }

        private void DrawList()
        {
            var objectsToRemove = new List<GameObject>();

            int i = 0;
            foreach (var gameObject in _currentList)
            {
                if (gameObject == null || !HasValidType(gameObject))
                {
                    objectsToRemove.Add(gameObject);
                    continue;
                }

                using (new GUILayout.HorizontalScope())
                {
                    DrawGameObject(gameObject, i);
                }

                if (_objectListFoldouts[i] && _objectComponents.ContainsKey(gameObject))
                {
                    EditorGUI.indentLevel++;
                    DrawInsideGameObject(gameObject);

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();
                }

                i++;
            }

            foreach (var gameObject in objectsToRemove)
            {
                if (_objectComponents.ContainsKey(gameObject)) _objectComponents.Remove(gameObject);
                if (_objectComponentFoldouts.ContainsKey(gameObject)) _objectComponentFoldouts.Remove(gameObject);
                _currentList.Remove(gameObject);
            }
        }

        private void DrawInsideGameObject(GameObject gameObject)
        {
            int compIndex = 0;
            foreach (var component in _objectComponents[gameObject])
            {
                _objectComponentFoldouts[gameObject][compIndex] =
                    EditorGUILayout.Foldout(_objectComponentFoldouts[gameObject][compIndex],
                        component.GetPrettyName());

                if (_objectComponentFoldouts[gameObject][compIndex])
                {
                    DrawInsideComponent(component);
                }

                compIndex++;
            }
        }

        private static void DrawInsideComponent(Component component)
        {
            var props = LevelerGUILayout.GetValidProperties(component);
            var methods = LevelerGUILayout.GetValidMethods(component);
            
            if (props.Count != 0)
            {
                GUILayout.BeginVertical($"Properties ({component.GetPrettyName()})", "window",
                    GUILayout.MaxHeight(40));
                foreach (var prop in props) LevelerGUILayout.PropertyGui(component, prop);
                EditorGUILayout.Space();
                GUILayout.EndVertical();
            }

            EditorGUILayout.Space();
            
            if (methods.Count != 0)
            {
                GUILayout.BeginVertical($"Methods ({component.GetPrettyName()})", "window",
                    GUILayout.MaxHeight(40));
                foreach (var method in methods) LevelerGUILayout.MethodGui(component, method);
                EditorGUILayout.Space();
                GUILayout.EndVertical();
            }
        }

        private bool HasValidType(GameObject gameObject)
        {
            foreach (var type in _validTypes)
            {
                if (gameObject.GetComponent(type) != null)
                    return true;
            }

            return false;
        }

        private void DrawGameObject(GameObject gameObject, int index)
        {
            GUI.enabled = false;

            EditorGUI.BeginChangeCheck();
            _objectListFoldouts[index] = EditorGUILayout.Foldout(_objectListFoldouts[index], gameObject.name);
            if (EditorGUI.EndChangeCheck())
            {
                if (_objectListFoldouts[index])
                {
                    var components = gameObject.GetComponents<MonoBehaviour>();
                    var validComponents = new List<Component>();

                    foreach (var component in components)
                    {
                        var methods = component.GetMethods();
                        var props = component.GetProperties();
                        if (methods.HasAttribute() || props.HasAttribute())
                            validComponents.Add(component);
                    }

                    var componentArray = validComponents.ToArray();
                    _objectComponents.TryAdd(gameObject, componentArray);
                    _objectComponentFoldouts.TryAdd(gameObject, new bool[componentArray.Length]);
                }
                else
                {
                    if (_objectComponents.ContainsKey(gameObject)) _objectComponents.Remove(gameObject);
                    if (_objectComponentFoldouts.ContainsKey(gameObject)) _objectComponentFoldouts.Remove(gameObject);
                }
            }

            GUI.enabled = true;
            if (GUILayout.Button("Delete"))
            {
                if (_objectComponents.ContainsKey(gameObject)) _objectComponents.Remove(gameObject);
                if (_objectComponentFoldouts.ContainsKey(gameObject)) _objectComponentFoldouts.Remove(gameObject);
                Undo.DestroyObjectImmediate(gameObject);
            }

            if (GUILayout.Button("Select")) Selection.activeGameObject = gameObject;
            if (GUILayout.Button("Focus"))
            {
                Selection.activeGameObject = gameObject;
                SceneView.FrameLastActiveSceneView();
            }
        }
    }
}