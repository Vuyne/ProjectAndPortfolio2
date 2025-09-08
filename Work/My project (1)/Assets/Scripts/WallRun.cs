using UnityEngine;

public class WallRun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public LayerMask maskWall;
    public LayerMask maskGround;
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    private float horizontalInput;
    private float verticalInput;

    public float wallCheckDistance;
    public float minJumpHeight;
    private RaycastHit leftWallhit;
    private RaycastHit rightWallhit;
    private bool wallLeft;
    private bool wallRight;

    public Transform orientation;
    private playerController pm;
    private Rigidbody rb;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<playerController>();
    }
    void Update()
    {
        CheckForWall();

    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallhit, wallCheckDistance, maskWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallhit, wallCheckDistance, maskWall);

    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, maskGround);
    }

    private void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("vertical");

        //WallRun
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround())
        {

        }
        
    }

    // Update is called once per frame
    
}
