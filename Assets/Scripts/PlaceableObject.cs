using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private float range;

    [LevelerProperty] public float Range
    {
        get => range;
        set => range = value;
    }

    [LevelerMethod]
    public void SetRangeToFive()
    {
        Range = 5;
    }
}