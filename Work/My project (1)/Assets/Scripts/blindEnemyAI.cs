using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class blindEnemyAI : EnemyBase
{
    [SerializeField] Rigidbody rb;
    [SerializeField] NavMeshAgent enemyAI;
    [SerializeField] Transform headPos;
    [SerializeField] Transform noisePos; 
    [SerializeField] Transform modelRoot;
    [SerializeField] Animator animator;
    [SerializeField] int animTransSpeed;
    [SerializeField] int roamDistance;
    [SerializeField] int roamPauseTimer;
    [SerializeField] int enemySpeed;
    [SerializeField] int enemyDamage;
    [SerializeField] float enemyAttackCD;
    [SerializeField] float enemyAttackRange;

    float enemyAttackCDOrig;
    float hearingRadius;
    float hearingLevel;
    float roamTimer;
    Vector3 startingPos;
    
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        if(!modelRoot) { modelRoot = transform; }
        enemyAttackCDOrig = enemyAttackCD;
        hearingRadius = 10f;
        hearingLevel = 5e-7f;
        animator = GetComponent<Animator>();
        startingPos = transform.position;
    }

    // Update is called once per frame
    protected override void Update()
    {
        //if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
        //    { return; }
        //animationLocation();
        //attackPlayer();
        if (enemyAI.stoppingDistance < 0.01f)
            roamTimer += Time.deltaTime;
        if (playerInTrigger && AudioPeer.currentAMP < hearingLevel)
        {
            checkRoam();
        }
        else if(!playerInTrigger)
        {
            checkRoam();
        }
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
                    if (enemyAI.remainingDistance <= enemyAI.stoppingDistance)
                    {
                        base.FaceTarget(targetPos);
                        IDamage dmg = hit.collider.GetComponent<IDamage>();
                        if (dmg != null && enemyAttackCD <= 0.0f && enemyAttackRange <= enemyAI.remainingDistance)
                        {
                            animator.SetTrigger("Punch");
                            dmg.takeDamage(enemyDamage);
                            enemyAttackCD = enemyAttackCDOrig;
                        }
                    }
                }
                else
                {
                    enemyAI.isStopped = true;
                }
                enemyAI.isStopped = false;
            }
        }

    }
    public override void takeDamage(int amount)
    {
        base.HP -= amount;
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
    void animationLocation()
    {
        float enemyCurrSpeed = enemyAI.velocity.normalized.magnitude;
        float animCurrSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animCurrSpeed, enemyCurrSpeed, Time.deltaTime * animTransSpeed));
    }
    void checkRoam()
    {
        if (roamTimer >= roamPauseTimer && enemyAI.stoppingDistance < 0.01f)
        {
            roam();
        }
    }
    void roam()
    {
        roamTimer = 0;
        enemyAI.stoppingDistance = 0;
        Vector3 randomPos = Random.insideUnitSphere * roamDistance;
        randomPos += startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDistance, 1);
        enemyAI.SetDestination(hit.position);
    }
}
