using UnityEngine;
using UnityEngine.AI;

public class blindEnemyAI : EnemyBase
{
    [SerializeField] Rigidbody rb;
    [SerializeField] NavMeshAgent enemyAI;
    [SerializeField] Transform headPos;
    [SerializeField] Transform noisePos; 
    [SerializeField] Transform modelRoot;
    [SerializeField] int enemySpeed;
    [SerializeField] int enemyDamage;
    [SerializeField] float enemyAttackCD;
    [SerializeField] float enemyAttackRange;

    float enemyAttackCDOrig;
    float hearingRadius;
    float hearingLevel;
    float test;
    Animator animator;
    bool isMoving;
    bool isCrouching;
    bool isJumping;
    bool isRoaring;
    bool isSniffing;
    bool enemyAnimation;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        if(!modelRoot) { modelRoot = transform; }
        enemyAttackCDOrig = enemyAttackCD;
        hearingRadius = 10f;
        hearingLevel = 5e-7f;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
            { return; }
        attackPlayer();
        //animator.SetBool("isWalking", true);

        //animator.SetBool("FindingPlayer", true);

        //animator.SetTrigger("BiteAttack");
    }
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = true;
    }
    protected override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = false;
    }
    void attackPlayer()
    {
        attackCD();
        Vector3 targetPos = gameManager.instance.player.transform.position - headPos.position;
        Debug.DrawRay(headPos.position, targetPos);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, targetPos, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (AudioPeer.currentAMP > hearingLevel)
                {
                    enemyAI.SetDestination(gameManager.instance.player.transform.position);
                    //isMoving = true;
                    //animator.SetBool("isWalking", isMoving);
                    if (enemyAI.remainingDistance <= enemyAI.stoppingDistance)
                    {
                        base.FaceTarget(targetPos);
                        IDamage dmg = hit.collider.GetComponent<IDamage>();
                        if (dmg != null && enemyAttackCD <= 0f && enemyAttackRange <= enemyAI.remainingDistance)
                        {
                            //animator.SetTrigger("punchAttack");
                            dmg.takeDamage(enemyDamage);
                            enemyAttackCD = enemyAttackCDOrig;
                        }
                    }
                }
                else
                {
                    enemyAI.isStopped = true;
                }
                //isMoving = false;
                enemyAI.isStopped = false;
            }
        }

    }
    public override void takeDamage(int amount)
    {
        base.HP -= 0;
    }
    void attackCD()
    {
        if (enemyAttackCD > 0f)
        {
            enemyAttackCD -= Time.deltaTime;
        }
        else if (enemyAttackCD <= 0f)
        {

        }
    }
}
