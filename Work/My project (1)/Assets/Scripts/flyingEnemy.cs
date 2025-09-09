using System.Buffers.Text;
using System.Collections;
using UnityEngine;

public class chasingFlyingEnemyAI : EnemyBase
{

    [SerializeField] float moveSpeed = 3f;
    [SerializeField] float hoverDistance = 6f; // stops this far from player
    [SerializeField] Rigidbody rb;
    [SerializeField] float hoverAmplitude = 1f; // vertical range
    [SerializeField] float hoverSpeed = 2f; // speed of vertical oscillation

    private float baseY;

   protected override void Start()
    {
        base.Start();
        baseY = transform.position.y; // starting height
      
    }

    protected override void Update()
    {
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
            base.FaceTarget(playerDir);

            // Shoot
            if (shootTimer >= shootRate)
                base.Shoot();
        }

    }



}
