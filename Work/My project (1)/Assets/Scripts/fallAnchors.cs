using UnityEngine;
using System;

public class fallAnchors : MonoBehaviour
{
    [SerializeField] float fallDelay = 3f;       
    [SerializeField] float resetDelay = 5f;      
    private Rigidbody rb;
    private bool isGrabbed = false;
    private float fallTimer;
    private float resetTimer;

    private Vector3 startPos;    
    private Quaternion startRot;

    public Action OnAnchorFall;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();

        rb.isKinematic = true;
        rb.useGravity = false;

        startPos = transform.position;
        startRot = transform.rotation;
    }

    public void OnGrabbed()
    {
        isGrabbed = true;
        fallTimer = fallDelay;
    }

    void Update()
    {
        if (isGrabbed)
        {
            fallTimer -= Time.deltaTime;
            if (fallTimer <= 0f)
            {
                rb.isKinematic = false;
                rb.useGravity = true;

                isGrabbed = false;
                resetTimer = resetDelay;

                OnAnchorFall?.Invoke();
            }
        }
        else if (!rb.isKinematic) 
        {
            resetTimer -= Time.deltaTime;
            if (resetTimer <= 0f)
            {
                ResetAnchor();
            }
        }
    }

    void ResetAnchor()
    {
        rb.isKinematic = true;
        rb.useGravity = false;

        transform.position = startPos;
        transform.rotation = startRot;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
