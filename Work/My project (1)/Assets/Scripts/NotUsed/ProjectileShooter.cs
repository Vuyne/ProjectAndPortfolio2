using System.Collections;
using UnityEngine;

public class ProjectileShooter : MonoBehaviour, IDamage
{
    [Header("References")]
    public Transform player;             
    public GameObject projectilePrefab;  
    public Transform shootPoint;         

    [Header("Stats")]
    public float health = 50f;
    public float shootRate = 2f;
    public float projectileSpeed = 20f;

    private float shootTimer = 0f;
    private Renderer model;
    private Color colorOrig;

    void Start()
    {
        if (player == null)
            player = GameObject.FindWithTag("Player")?.transform;

        model = GetComponent<Renderer>();
        if (model != null)
            colorOrig = model.material.color;
    }

    void Update()
    {
        if (player == null) return;

        //
        Vector3 dir = (player.position - transform.position).normalized;
        dir.y = 0; //
        transform.rotation = Quaternion.LookRotation(dir);

        //
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            ShootAtPlayer();
            shootTimer = 0f;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || shootPoint == null) return;

        // spawn projectile
        GameObject proj = Instantiate(projectilePrefab, shootPoint.position, Quaternion.identity);
        Rigidbody rb = proj.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 dir = (player.position - shootPoint.position).normalized;
            rb.linearVelocity = dir * projectileSpeed;
        }
    }

    public void takeDamage(int amount)
    {
        health -= amount;

        if (model != null)
            StartCoroutine(FlashRed());

        if (health <= 0)
            Destroy(gameObject);
    }

    IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
