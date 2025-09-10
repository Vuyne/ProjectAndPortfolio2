using UnityEngine;
using UnityEngine.AI; // for navmeshagent

public class EnemyGolem : EnemyBase
{
    [SerializeField] NavMeshAgent agent;

    [SerializeField] Transform modelRoot;

    [SerializeField] float moveSpeed = 3f;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        if (!modelRoot) modelRoot = transform; 
    }

    // Update is called once per frame
    protected override void Update()
    {
        if (!playerInTrigger || gameManager.instance == null || gameManager.instance.player == null)
            return;

        if (playerInTrigger)
        {
            agent.SetDestination(gameManager.instance.player.transform.position); //enemy follow player
        }


    }

}
