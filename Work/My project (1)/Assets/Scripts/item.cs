using UnityEngine;

[CreateAssetMenu(menuName = "key")]
public class item : ScriptableObject
{
    [SerializeField] GameObject itemModel;
    public string itemName;
}
