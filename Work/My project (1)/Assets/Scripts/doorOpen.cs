using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private item requiredKey;
    [SerializeField] private keyInventory keyInventory;

    [Header("Door Parts")]
    [SerializeField] private Animator anim;
    [SerializeField] private Collider doorCollider;

    public void TryOpen()
    {
        if (keyInventory.HasKey(requiredKey))
        {
            anim.SetTrigger("Open");
            doorCollider.enabled = false;
            Debug.Log("Door opened with key: " + requiredKey.itemName);
        }
        else
        {
            Debug.Log("You need the " + requiredKey.itemName + " to open this door.");
        }
    }
}
