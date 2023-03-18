using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private float floatVar;
    [SerializeField] private int intVar;
    [SerializeField] private string stringVar;
    [SerializeField] private Color colorVar;
    [SerializeField] private Vector2 v2Var;
    [SerializeField] private Vector3 v3Var;

    [LevelerProperty] public float FloatVar
    {
        get => floatVar;
        set => floatVar = value;
    }
    
    [LevelerProperty] public int IntVar
    {
        get => intVar;
        set => intVar = value;
    }
    
    [LevelerProperty] public string StringVar
    {
        get => stringVar;
        set => stringVar = value;
    }
    
    [LevelerProperty] public Color ColorVar
    {
        get => colorVar;
        set => colorVar = value;
    }
    
    [LevelerProperty] public Vector2 V2Var
    {
        get => v2Var;
        set => v2Var = value;
    }
    
    [LevelerProperty] public Vector3 V3Var
    {
        get => v3Var;
        set => v3Var = value;
    }
}