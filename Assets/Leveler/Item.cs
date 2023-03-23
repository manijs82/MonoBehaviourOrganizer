using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private float floatVar;
    [SerializeField] private string stringVar;
    [SerializeField] private Vector2 vector2Var;

    [LevelerProperty] public float FloatVar
    {
        get => floatVar;
        set => floatVar = value;
    }
    
    [LevelerMethod] public void SetFloat(float value)
    {
        floatVar = value;
    }

    [LevelerMethod] public void PrintFloat()
    {
        print(floatVar);
    }
    
    [LevelerMethod] public void SetString(string value)
    {
        stringVar = value;
    }
    
    [LevelerMethod] public void SetVector2(Vector2 value)
    {
        vector2Var = value;
    }
}