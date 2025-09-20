using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] gunStats gun;
    private void OnTriggerEnter(Collider collision)
    {
        IPickup pickupable = collision.GetComponent<IPickup>();
        if (pickupable != null)
        {
            gun.ammoCur = gun.ammoMax;
            pickupable.getGunStats(gun);
            Destroy(gameObject);
        }
    }
}
