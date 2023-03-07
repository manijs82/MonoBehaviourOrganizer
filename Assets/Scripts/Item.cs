using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;

    [LevelerMethod]
    public void SetNameToFoo()
    {
        itemName = "Foo";
    }
}