using UnityEngine;

public class GunPickup : MonoBehaviour, IPickup
{
    [SerializeField] private gunStats gun;

    public void OnPickup(GameObject player)
    {
        Debug.Log("Triggered");
        gun.ammoCur = gun.ammoMax; // refill ammo
        player.GetComponent<playerMovement>().getGunStats(gun);
        Debug.Log("Picked up gun: " + gun.name);
        Destroy(gameObject);
    }
}
