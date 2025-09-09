using UnityEngine;

public class EndofLevelTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Tell gameManager the player reached the end
            gameManager.instance.PlayerReachedEnd();
        }
    }
}
