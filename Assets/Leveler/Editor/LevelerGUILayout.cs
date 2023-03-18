using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Leveler
{
    public static class LevelerGUILayout
    {
        private static readonly Type[] ValidTypes =
        {
            typeof(float),
            typeof(int),
            typeof(Vector2),
            typeof(Vector3),
            typeof(Color),
            typeof(string)
        };

        private static Dictionary<PropertyInfo, object> _registeredProperties;

        private static void RegisterProperty(PropertyInfo propertyInfo)
        {
            if (_registeredProperties == null)
            {
                Debug.LogError("You are trying to add to a closed registry");
                return;
            }

            _registeredProperties.Add(propertyInfo, default);
        }

        public static void OpenRegistry()
        {
            _registeredProperties = new Dictionary<PropertyInfo, object>();
        }

        public static void CloseRegistry()
        {
            _registeredProperties = null;
        }

        public static void MethodGui(Component component, MethodInfo method)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0 && method.ReturnType == typeof(void))
            {
                if (GUILayout.Button(method.Name))
                {
                    Undo.RecordObject(component, $"Invoke {method.Name}");
                    method.Invoke(component, null);
                }
                return;
            }

            if (parameters.Length != 1) return;

            var param = parameters[0];
            if (param.ParameterType == typeof(float))
            {
                EditorGUILayout.LabelField(method.Name);
                method.Invoke(component, new object[] { EditorGUILayout.Slider(1, 0, 1) });
            }
        }

        public static void PropertyGui(Component component, PropertyInfo property)
        {
            if (!_registeredProperties.ContainsKey(property))
            {
                RegisterProperty(property);
            }
            else
            {
                EditorGUI.BeginChangeCheck();
                _registeredProperties[property] = DisplayProperty(component, property);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(component, $"Change {property.Name}");
                    property.SetValue(component, _registeredProperties[property]);
                }
            }
        }

        private static object DisplayProperty(Component component, PropertyInfo property)
        {
            var type = property.PropertyType;
            switch(type.Name) {
                case "Single":
                    return EditorGUILayout.FloatField(property.Name, (float)property.GetValue(component));
                case "Int32":
                    return EditorGUILayout.IntField(property.Name, (int)property.GetValue(component));
                case "Color":
                    return EditorGUILayout.ColorField(property.Name, (Color)property.GetValue(component));
                case "Vector2":
                    return EditorGUILayout.Vector2Field(property.Name, (Vector2)property.GetValue(component));
                case "Vector3":
                    return EditorGUILayout.Vector3Field(property.Name, (Vector3)property.GetValue(component));
                case "String":
                    return EditorGUILayout.TextField(property.Name, (string)property.GetValue(component));
            }

            return default;
        }

        public static List<MethodInfo> GetValidMethods(Component component)
        {
            var o = new List<MethodInfo>();
            foreach (var method in component.GetMethods())
            {
                var attribute = Attribute.GetCustomAttribute(method, typeof(LevelerMethodAttribute));
                if (attribute == null || method.IsSpecialName) continue;
                
                o.Add(method);
            }

            return o;
        }
        
        public static List<PropertyInfo> GetValidProperties(Component component)
        {
            var o = new List<PropertyInfo>();
            foreach (var property in component.GetProperties())
            {
                var attribute = Attribute.GetCustomAttribute(property, typeof(LevelerPropertyAttribute));
                if (attribute == null) continue;
                if (!property.CanRead && !property.CanWrite) continue;
                if (!ValidTypes.Contains(property.PropertyType)) continue;
                
                o.Add(property);
            }

            return o;
        }

        public static bool HasAttribute(this MemberInfo[] memberInfos)
        {
            foreach (var memberInfo in memberInfos)
            {
                if (HasAttribute(memberInfo))
                    return true;
            }

            return false;
        }

        public static bool HasAttribute(this MemberInfo memberInfo)
        {
            var propAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(LevelerPropertyAttribute));
            var methodAttribute = Attribute.GetCustomAttribute(memberInfo, typeof(LevelerMethodAttribute));
            return propAttribute != null || methodAttribute != null;
        }

        public static MethodInfo[] GetMethods(this Component component) =>
            component.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        public static PropertyInfo[] GetProperties(this Component component) =>
            component.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

        public static string GetPrettyName(this Component component) =>
            ObjectNames.NicifyVariableName(component.GetType().Name);
    }
}