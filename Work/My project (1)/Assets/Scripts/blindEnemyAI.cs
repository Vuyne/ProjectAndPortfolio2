using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class blindEnemyAI : EnemyBase
{
    //[SerializeField] Rigidbody rb;
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
    float hearingLevel;
    float roamTimer;
    float stoppingDistanceOrig;
    float distanceFromPlayer;
    int HPOrig;
    Vector3 startingPos;
    
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        if(!modelRoot) { modelRoot = transform; }
        enemyAttackCDOrig = enemyAttackCD;
        hearingLevel = 5e-7f;
        animator = GetComponent<Animator>();
        startingPos = transform.position;
        stoppingDistanceOrig = enemyAI.stoppingDistance;
    }

    // Update is called once per frame
    protected override void Update()
    {
        animationLocation();
        if (enemyAI.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;
        if (playerInTrigger && AudioPeer.currentAMP < hearingLevel)
        {
            checkRoam();
        }
        else if (!playerInTrigger)
        {
            checkRoam();
        }
        attackPlayer();
        
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
                noiseLevel();
                if (AudioPeer.currentAMP > hearingLevel && playerInTrigger)
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
                    enemyAI.stoppingDistance = stoppingDistanceOrig;
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
        HP -= amount;
        if (HP <= HPOrig / 2)
        {
            animator.SetTrigger("Stunned");
        }
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
        if (roamTimer >= roamPauseTimer && enemyAI.remainingDistance < 0.01f)
        {
            roam();
        }
    }
    void roam()
    {
        roamTimer = 0;
        enemyAI.stoppingDistance = 0;
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * roamDistance;
        randomPos += startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDistance, 1);
        enemyAI.SetDestination(hit.position);
    }
    void noiseLevel()
    {
        float xDistance = gameManager.instance.player.transform.position.x - transform.position.x;
        float yDistance = gameManager.instance.player.transform.position.x - transform.position.x;
        distanceFromPlayer = Mathf.Sqrt((xDistance * xDistance) + (yDistance * yDistance));
        AudioPeer.currentAMP = AudioPeer.currentAMP / distanceFromPlayer;
    }
}
