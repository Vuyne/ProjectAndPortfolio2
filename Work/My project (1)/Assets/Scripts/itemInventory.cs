using System.Collections.Generic;
using UnityEngine;

public class keyInventory : MonoBehaviour
{
    private HashSet<item> keys = new HashSet<item>();

    public void AddKey(item keyID)
    {
        keys.Add(keyID);
    }

    public bool HasKey(item keyID)
    {
        return keys.Contains(keyID);
    }

    public bool UseKey(item keyID)
    {
        return keys.Remove(keyID);
    }

    public List<item> GetAllKeys()
    {
        return new List<item>(keys);
    }
}
