using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Key Inventory")]
public class keyInventory : ScriptableObject
{
    private List<item> collectedKeys = new List<item>();

    public void AddKey(item key)
    {
        if (!collectedKeys.Contains(key))
            collectedKeys.Add(key);
    }

    public bool HasKey(item key)
    {
        return collectedKeys.Contains(key);
    }

    public void ResetKeys()
    {
        collectedKeys.Clear();
    }
}
