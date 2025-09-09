using UnityEngine;

public class Pickup : MonoBehaviour
{
    //new
    [SerializeField] DiamondManager dm;
    void Awake()
    {
        if (dm == null)
        {
            dm = FindFirstObjectByType<DiamondManager>();
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player")) return;

        dm?.PickUpone();
        Destroy(gameObject);  
        
    }

}
