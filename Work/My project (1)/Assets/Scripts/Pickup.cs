using UnityEngine;

public class pickUp : MonoBehaviour
{
    [SerializeField] gunStats gun;
    private void OnTriggerEnterPickup(Collider collision)
    {
        IPickup pickupable = collision.GetComponent<IPickup>();
        if (pickupable != null)
        {
            gun.ammoCur = gun.ammoMax;
            pickupable.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}
