using System.Collections;
using UnityEngine;
using UnityEngine.AI; //for navMesh

public class ZombieEnemy : EnemyBase
{
    [SerializeField] NavMeshAgent agent; //to walk 
    [SerializeField] Animator anim;
    [SerializeField] Rigidbody rb;


    // Update is called once per frame
    void Update()
    {

        setAnimLocomotion();

        if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
            return;

        if (playerInTrigger)
        {
            agent.SetDestination(gameManager.instance.player.transform.position); //enemy follow player
        }
    }

    void setAnimLocomotion()
    {
        //float agentSpeedCurr = agent.velocity.normalized.magnitude;
        //float animSpeedCurr = anim.GetFloat("Speed");

        // anim.SetFloat("Speed", Mathf.Lerp(animSpeedCurr, agentSpeedCurr, Time.deltaTime * animTransSpeed));

        anim.SetFloat("Speed", agent.velocity.normalized.magnitude);
    }
    
}
