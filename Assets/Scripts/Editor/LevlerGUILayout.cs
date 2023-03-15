using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class LevlerGUILayout
{
    public static void MethodGui(Component component, MethodInfo method)
    {
        var attribute = Attribute.GetCustomAttribute(method, typeof(LevelerMethodAttribute));
        if (attribute == null) return;

        var parameters = method.GetParameters();
        if (parameters.Length == 0 && method.ReturnType == typeof(void))
        {
            if (GUILayout.Button(method.Name))
                method.Invoke(component, null);
            return;
        }

        if (parameters.Length != 1) return;

        var param = parameters[0];
        if (param.ParameterType == typeof(float) && !method.IsSpecialName)
        {
            EditorGUILayout.LabelField(method.Name);
            method.Invoke(component, new object[] { EditorGUILayout.Slider(1, 0, 1) });
        }
    }

    public static void PropertyGui(Component component, PropertyInfo property)
    {
        var attribute = Attribute.GetCustomAttribute(property, typeof(LevelerPropertyAttribute));
        if (attribute == null) return;

        if (!property.CanRead && !property.CanWrite) return;

        if (property.PropertyType == typeof(float))
        {
            EditorGUI.BeginChangeCheck();
            property.SetValue(component,
                EditorGUILayout.FloatField(property.Name, (float)property.GetValue(component)));
            if(EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(component);
            }
        }
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