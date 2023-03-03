using UnityEngine;

public class PlaceableObject : MonoBehaviour
{
    [Range(0,1)][SerializeField] private float range;

    public void SetRange(float range)
    {
        this.range = range;
    }
}
