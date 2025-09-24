using System.Collections;
using UnityEngine;

public class chasingFlyingEnemyAI : EnemyBase
{
    [Header("Movement")]
    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float hoverDistance = 6f; // stops this far from player
    [SerializeField] float hoverAmplitude = 1f; // vertical range
    [SerializeField] float hoverSpeed = 2f;     // speed of vertical oscillation
    [SerializeField] Rigidbody rb;

    private float baseY;

    [Header("Roaming")]
    [SerializeField] protected int roamDist = 10;
    [SerializeField] protected int roamPauseTime = 3;
    protected float roamTimer;
    protected Vector3 startingPos;
    protected Vector3 roamTarget;

    protected override void Start()
    {
        base.Start();
        baseY = transform.position.y;
        startingPos = transform.position;
        PickNewRoamTarget();
    }

    protected override void Update()
    {
        if (gameManager.instance == null || gameManager.instance.player == null)
            return;

        shootTimer += Time.deltaTime;

        if (playerInTrigger)
        {
            HandleChase();
        }
        else
        {
            HandleRoam();
        }
    }

    void HandleChase()
    {
        // Direction toward player (flattened)
        Vector3 targetPos = gameManager.instance.player.transform.position;
        Vector3 flatDir = targetPos - transform.position;
        flatDir.y = 0;

        float distance = flatDir.magnitude;

        // Move toward player until hover distance
        if (distance > hoverDistance)
        {
            Vector3 moveDir = flatDir.normalized;
            transform.position += moveDir * moveSpeed * Time.deltaTime;
        }

        // Hover vertically
        float newY = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        // Face player horizontally
        base.FaceTarget(flatDir);

        // Shoot
        if (shootTimer >= shootRate)
            base.Shoot();
    }

    void HandleRoam()
    {
        roamTimer += Time.deltaTime;

        // reached roam target
        Vector3 flatDir = roamTarget - transform.position;
        flatDir.y = 0;
        if (flatDir.magnitude <= 0.5f)
        {
            if (roamTimer >= roamPauseTime)
            {
                PickNewRoamTarget();
                roamTimer = 0;
            }
        }
        else
        {
            // move toward roam target
            Vector3 moveDir = flatDir.normalized;
            transform.position += moveDir * (moveSpeed * 0.5f) * Time.deltaTime; // slower roam
        }

        // hover motion even while roaming
        float newY = baseY + Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void PickNewRoamTarget()
    {
        Vector3 randPos = Random.insideUnitSphere * roamDist;
        randPos.y = 0;
        roamTarget = startingPos + randPos;
    }

    public override void takeDamage(int amount)
    {
        base.takeDamage(amount);
        // If hit, immediately aggro
        playerInTrigger = true;
    }
}
