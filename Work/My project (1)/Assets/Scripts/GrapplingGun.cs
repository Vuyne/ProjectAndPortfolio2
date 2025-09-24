using UnityEngine;

public class GrapplingGun : MonoBehaviour
{
    private LineRenderer lineRender;
    private Vector3 grabPoint;
    public LayerMask maskGrappingItem;
    private Transform guntip, myCamera, player;
    private Rigidbody grabbedRb;

    [SerializeField] float maxDistance = 100f;
    [SerializeField] float ropeStrength = 45f;
    [SerializeField] float dampValue = 7f;
    private SpringJoint joint;
    private Vector3 currentGrabPosition;

   // [SerializeField] float fallDelay = 2f;  
  //  private float fallTimer;
   // private bool isAnchorBreaking;

    void Awake()
    {
        enabled = false;
        guntip = GameObject.FindWithTag("GunTip").transform;
       myCamera = Camera.main.transform;
        player = GameObject.FindWithTag("Player").transform;

        lineRender = GetComponent<LineRenderer>();
    }

    void Update()
    {
        /*if (isAnchorBreaking)
        {
            fallTimer -= Time.deltaTime;

            if (fallTimer <= 0f)
            {
                if (grabbedRb != null)
                {
                    grabbedRb.isKinematic = false;
                    grabbedRb.useGravity = true;   
                }
                StopGrab();
                isAnchorBreaking = false;
            }
        }*/



        if (Input.GetButtonDown("Hook")) 
          {
            StartGrab();
          }
        else if(Input.GetButtonUp("Hook"))
        {
            StopGrab();
        }
    }

    void StartGrab()
    {
        RaycastHit hit;
        if (Physics.Raycast(myCamera.position, myCamera.forward, out hit, maxDistance, maskGrappingItem))
        {
            grabPoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grabPoint;

            float distanceFromPoint = Vector3.Distance(player.position, grabPoint);

            //Physics!!!!!!!!!!!!
            joint.maxDistance = 10f;
            joint.minDistance = distanceFromPoint * 0.25f;
            joint.spring = ropeStrength;
            joint.damper = dampValue;
            joint.massScale = 4.5f;

            lineRender.positionCount = 2;
            currentGrabPosition = guntip.position;
            fallAnchors anchor = hit.collider.GetComponent<fallAnchors>();
            if (anchor != null)
            {
                anchor.OnGrabbed();
                anchor.OnAnchorFall += StopGrab;
            }

            /*if (hit.collider.CompareTag("SpecialAnchor"))
            {
                grabbedRb = hit.collider.attachedRigidbody;
                if (grabbedRb != null)
                {
                    grabbedRb.isKinematic = true; 
                }
                fallTimer = fallDelay;
                isAnchorBreaking = true;
            }*/

        }
    }
        

    

    void LateUpdate()
    {


        Drawrope();

    }


    void StopGrab()
    {
        lineRender.positionCount = 0;
        Destroy(joint);

    }

    void Drawrope()
    {
        if (!joint) return;

        currentGrabPosition = Vector3.Lerp(currentGrabPosition, grabPoint, Time.deltaTime * 8f);


        lineRender.SetPosition(0, guntip.position);
        lineRender.SetPosition(1, grabPoint);
    }

   /* public bool IsGrappling()
    {
        return joint != null;
    }

    public Vector3 GetGrapplePoint()
    {
        return grabPoint;
    }*/
}
