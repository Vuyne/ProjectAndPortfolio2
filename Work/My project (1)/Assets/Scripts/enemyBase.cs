using System.Collections;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamage
{
    [SerializeField] protected Renderer model;
    [SerializeField] protected Transform shootPos;
    [SerializeField] protected GameObject bullet;
    [SerializeField] protected int HP = 5;
    [SerializeField] protected float shootRate = 2f;
    [SerializeField] protected int faceTargetSpeed = 5;
    protected float shootTimer;
    protected Color colorOrig;
    protected bool playerInTrigger;
    protected Vector3 playerDir;

    protected virtual void Start()
    {
        colorOrig = model.material.color;
        shootTimer = 0;
        //gameManager.instance?.updateGameGoal(1);
    }

    protected virtual void Update()
    {
       
        if (!playerInTrigger || gameManager.instance.player == null)
            return;

        shootTimer += Time.deltaTime;
        playerDir = gameManager.instance.player.transform.position - transform.position;
        FaceTarget(playerDir);
        if (shootTimer >= shootRate)
            Shoot();
    }

    protected virtual void FaceTarget(Vector3 dir)//how quick enemy will rotate to face us
    {
        if (dir == Vector3.zero) return;
        Quaternion rot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    protected virtual void Shoot()
    {
        if (bullet == null || shootPos == null) return;
        Vector3 dir = (gameManager.instance.player.transform.position - shootPos.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);
        Instantiate(bullet, shootPos.position, rot);
        shootTimer = 0;
    }

    public virtual void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(FlashRed());
        if (HP <= 0)
        {
            gameManager.instance?.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    protected IEnumerator FlashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = true;
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = false;
    }
}
