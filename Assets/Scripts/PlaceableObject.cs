using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [SerializeField] private float range;

    public float Range
    {
        get => range;
        set => range = value;
    }

    public void SetRangeToFive()
    {
        Range = 5;
    }
}
