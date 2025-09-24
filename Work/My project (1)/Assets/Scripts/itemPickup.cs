using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    [Header("Key Settings")]
    [SerializeField] private item keyID; 

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player has a keyInventory
        keyInventory playerInv = other.GetComponent<keyInventory>();
        if (playerInv != null)
        {
            // Add key to player's inventory
            playerInv.AddKey(keyID);
            Debug.Log($"Picked up key: {keyID}");

            // Remove key object from the scene
            Destroy(gameObject);
        }
    }
}
