using System.Buffers.Text;
using System.Collections;
using UnityEngine;
using UnityEngine.AI; //for navMesh


public class ZombieEnemy : EnemyBase
{
    [SerializeField] NavMeshAgent agent; //to walk 
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody rb;
    [SerializeField] Transform headPos;

    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamPauseTime; //pause between destinations

    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    Vector3 startingPos;
    Vector3 playerDirection;

    protected override void Start()
    {
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

    }

    // Update is called once per frame
    protected override void Update()
    {

         setAnimLocomotion();

        if (agent.remainingDistance < 0.01f)
        {
            roamTimer += Time.deltaTime;

        }

        if (playerInTrigger && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInTrigger)
        {
            checkRoam();
        }


        if (playerInTrigger)
        {
            canSeePlayer();
        }

    }

    //bool canSeePlayer()
    //{
    //    if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
    //        return false;

    //    if (playerInTrigger)
    //    {
    //        agent.SetDestination(gameManager.instance.player.transform.position); //enemy follow player
    //    }

    //    return true;
    //}



    void setAnimLocomotion()
    {
       anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
    }

    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f) //time is possible to get there and he close to the destination
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;

        agent.stoppingDistance = 0;

        Vector3 randomPos = Random.insideUnitSphere * roamDist; //create a sphere where he will select random position
        randomPos += startingPos; //to go back to the original position while roaming

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

    }

    bool canSeePlayer()
    {

        playerDirection = gameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);
        Debug.DrawRay(headPos.position, playerDirection);

        RaycastHit hit;

        if (Physics.Raycast(headPos.position, playerDirection, out hit))
        {

            if (angleToPlayer <= FOV && hit.collider.transform.CompareTag("Player"))
            {

                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }
                agent.stoppingDistance = stoppingDistOrig;

                return true;
            }

        }
        agent.stoppingDistance = 0;
        return false;

    }

    void faceTarget()
    {

        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDirection.x, transform.position.y, playerDirection.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    public override void takeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(FlashRed());
            agent.SetDestination(gameManager.instance.player.transform.position);
        }

        if (HP <= 0)
        {
           // gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }
}
