using System.Buffers.Text;
using System.Collections;
using UnityEngine;

public class chasingFlyingEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] Transform shootPos;

    [SerializeField] int HP = 5;
    [SerializeField] int faceTargetSpeed = 5;
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate = 2f;
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float hoverDistance = 6f; // stops this far from player

    [SerializeField] float hoverAmplitude = 1f; // vertical range
    [SerializeField] float hoverSpeed = 2f; // speed of vertical oscillation

    Color colorOrig;
    float shootTimer;
    bool playerInTrigger;
    Vector3 playerDir;
    private float baseY;

    void Start()
    {
        colorOrig = model.material.color;
        shootTimer = 0;
        baseY = transform.position.y; // starting height
        gameManager.instance.updateGameGoal(1);
    }

    void Update()
    {
        if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
            return;

        shootTimer += Time.deltaTime;

        // Get horizontal direction only
        Vector3 targetPos = gameManager.instance.player.transform.position;
        Vector3 flatTargetDir = targetPos - transform.position;
        flatTargetDir.y = 0; // ignore vertical difference

        float distance = flatTargetDir.magnitude;

        // Move only if farther than hover distance
        if (distance > hoverDistance)
        {
            Vector3 moveDir = flatTargetDir.normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }
        // Vertical hovering
        float newY = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        // Face the player horizontally
        faceTarget();

        // Shoot
        if (shootTimer >= shootRate)
            shoot();
    }


    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, faceTargetSpeed * Time.deltaTime);
    }

    void shoot()
    {
      
        Vector3 dir = (gameManager.instance.player.transform.position - shootPos.position).normalized;
        Quaternion rot = Quaternion.LookRotation(dir);

        Instantiate(bullet, shootPos.position, rot);
        shootTimer = 0;
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

    public void takeDamage(int amount)
    {
        HP -= amount;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            Destroy(gameObject);
            /*gameManager.instance.updateGameGoal(-1);*/
           
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
