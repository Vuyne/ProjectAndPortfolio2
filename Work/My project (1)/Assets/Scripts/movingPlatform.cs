using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    [SerializeField] CharacterController player;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            player.transform.parent = transform;
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            player.transform.parent = null;
    }
}
