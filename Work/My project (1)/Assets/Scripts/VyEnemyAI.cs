using System.Collections;
using UnityEngine;

public class VyenemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;
    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    Color colorOrig;
    float shootTimer;
    bool playerInTrigger;
    Vector3 playerDir;

    void Start()
    {
        colorOrig = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    void Update()
    {
        shootTimer += Time.deltaTime;

        if (playerInTrigger)
        {
            playerDir = gameManager.instance.player.transform.position - transform.position;
            faceTarget();
            if (shootTimer >= shootRate)
            {
                shoot();
            }
        }
    }

    void faceTarget()
    {
        Vector3 lookDir = new Vector3(playerDir.x, playerDir.y, playerDir.z);
        if (lookDir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(lookDir);
            transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = false;
    }

    void shoot()
    {
        shootTimer = 0;
        Instantiate(bullet, shootPos.position, transform.rotation);
    

        Debug.Log("Enemy Shooting!");
    }

    public void takeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(flashRed());
        }
        if (HP <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}