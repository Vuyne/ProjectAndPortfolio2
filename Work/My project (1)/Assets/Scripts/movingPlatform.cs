using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    [SerializeField] Rigidbody player;
    private void OnTriggerStay(Collider other)
    {
        player = other.GetComponent<Rigidbody>();
        if (player.CompareTag("Player"))
        {
            player.transform.parent = transform;
            player.isKinematic = true;
        }
        //if (player.transform.parent != null)
        //{
        //    player.isKinematic = false;
        //}
    }
    private void OnTriggerExit(Collider other)
    {
        player = other.GetComponent<Rigidbody>();
        if (player.CompareTag("Player"))
        {
            player.transform.parent = null;
            player.isKinematic = false;
        }
    }
}
