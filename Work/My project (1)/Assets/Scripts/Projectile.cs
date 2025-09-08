using UnityEngine;

public class Projectile : MonoBehaviour
{
    public Rigidbody rb;
    public GameObject explosion;
    public LayerMask Ennemy;

    //Stats
    [Range(0f, 1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int explosionDamage;
    public float explosionRange;

    //Lifetime
    public int maxCollions;
    public float maxLifetime;
    public bool explodeOntouch = true;

    int collisions;
    PhysicsMaterial physics_mat;

    private void Setup()
    {
        //Create 
    }

   
}
