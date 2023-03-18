using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private string itemName;

    [LevelerProperty] public string ItemName
    {
        get => itemName;
        set => itemName = value;
    }

    [LevelerMethod]
    public void SetNameToFoo()
    {
        itemName = "Foo";
    }
}