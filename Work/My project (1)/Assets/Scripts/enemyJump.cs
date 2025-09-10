using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class enemyJump : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Rigidbody rb;
    [SerializeField] float jumpPower;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        agent.autoTraverseOffMeshLink = false;
    }

    // Update is called once per frame
    void Update()
    {
       if (agent.isOnOffMeshLink)
        {
            StartCoroutine(jumpToTarget());
        }
    }
    IEnumerator jumpToTarget()
    {
        OffMeshLinkData data = agent.currentOffMeshLinkData;
        Vector3 startPos = agent.transform.position;
        Vector3 endPos = data.endPos;

        float timer = 0f;
        float jumpDuration = 1f;

        agent.enabled = false;

        while (timer <= jumpDuration)
        {
            float t = timer / jumpDuration;
            float yOffset = jumpPower * (t - t * t);
            transform.position = Vector3.Lerp(startPos, endPos, t) + new Vector3(0, yOffset, 0);

            timer += Time.deltaTime;
            yield return null;
        }
        agent.enabled = true;
        agent.CompleteOffMeshLink();
    }
}
