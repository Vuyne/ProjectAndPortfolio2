using System.Collections;
using UnityEngine;
using UnityEngine.AI; //for navMesh

public class ZombieEnemy : EnemyBase
{
    [SerializeField] NavMeshAgent agent; //to walk 
    [SerializeField] Rigidbody rb;


    // Update is called once per frame
    void Update()
    {
        if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
            return;

        if (playerInTrigger)
        {
            agent.SetDestination(gameManager.instance.player.transform.position); //enemy follow player
        }
    }

    
}
