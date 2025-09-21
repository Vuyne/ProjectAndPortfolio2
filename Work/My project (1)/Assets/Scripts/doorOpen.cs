/*using UnityEngine;

public class Door : MonoBehaviour
{
   [SerializeField] private Key requiredKey;
    [SerializeField] private KeyInventory keyInventory;

    [Header("Door Parts")]
    [SerializeField] private Animator anim;
    [SerializeField] private Collider doorCollider;

    public void TryOpen()
    {
        if (keyInventory.HasKey(requiredKey))
        {
            anim.SetTrigger("Open");
            doorCollider.enabled = false;
            Debug.Log("Door opened with key: " + requiredKey.KeyName);
        }
        else
        {
            Debug.Log("You need the " + requiredKey.KeyName + " to open this door.");
        }
    }
}
*/