using UnityEngine;

public class Pickup : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            FindFirstObjectByType<DiamondManager>().PickUpone(); 
            Destroy(gameObject);  
        }
    }

}
