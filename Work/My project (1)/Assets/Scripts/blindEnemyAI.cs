using UnityEngine;
using UnityEngine.AI;

public class blindEnemyAI : EnemyBase
{
    [SerializeField] Rigidbody rb;
    [SerializeField] NavMeshAgent enemyAI;
    [SerializeField] Transform handPos;
    [SerializeField] Transform headPos;
    [SerializeField] Transform noisePos; 
    [SerializeField] Transform modelRoot;
    [SerializeField] int enemySpeed;

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
        hearingRadius = 10f;
        hearingLevel = 5e-7f;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
            { return; }
        Vector3 targetPos = gameManager.instance.player.transform.position - headPos.position;
        Debug.DrawRay(headPos.position, targetPos);
        RaycastHit hit;
        if (Physics.Raycast(headPos.position, targetPos, out hit))
        {
            if (hit.collider.CompareTag("Player"))
            {
                if (AudioPeer.currentAMP > hearingLevel)
                {
                    test = AudioPeer.currentAMP;
                    enemyAI.SetDestination(gameManager.instance.player.transform.position);
                    base.FaceTarget(targetPos);
                }
                else
                {
                    enemyAI.isStopped = true;
                }
            }
        }
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
}
