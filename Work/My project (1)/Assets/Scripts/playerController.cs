using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask ignoreLayer;

    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;

    Vector3 moveDir;
    Vector3 playerVel;

    float shootTimer;
    int jumpCount;

    bool isSprinting;

    //Flying mechanics
    bool isFlyingActive = false;
    [SerializeField] float gravityRush;
    [SerializeField] float gravityFall;
    [SerializeField] float verticalSmoothTime;
    [SerializeField] float horizontalSmoothTime = 0.1f;
    private float verticalVelocitySmooth;
    [SerializeField] float smoothY;

    //this: Wallrun settings
    [Header("Wall Run")]
    public LayerMask maskWall;
    public LayerMask maskGround;
    public float wallRunForce = 5f;
    public float maxWallRunTime = 1.5f;
    public float wallCheckDistance = 1f;
    public float minJumpHeight = 1.5f;
    private bool wallLeft;
    private bool wallRight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private float wallRunTimer;
    [SerializeField] private Transform orientation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (orientation == null)
            orientation = Camera.main.transform;

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        movement();

    }
    void movement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        shootTimer += Time.deltaTime;

        //movedir
        moveDir = transform.right * horizontalInput + transform.forward * verticalInput;

        //Sprint
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }

        // this: Grounded check 
        bool grounded = controller.isGrounded;
        if (grounded && playerVel.y < 0)
            playerVel.y = -2f;

        //Flying
        if (!isFlyingActive)
        {
            if (controller.isGrounded && playerVel.y < 0)
                playerVel.y = 0;

            if (Input.GetButtonDown("Jump") && controller.isGrounded)
                playerVel.y = jumpSpeed;

            playerVel.y -= gravity * Time.deltaTime;
        }
        else
        {
            //jumpin
            float targetY = Input.GetButton("Jump") ? jumpSpeed * gravityRush : 0f;
            playerVel.y = Mathf.Lerp(playerVel.y, targetY, smoothY * Time.deltaTime);
           
        }
        //this: wall run detection
        CheckForWall();

        if ((wallLeft || wallRight) && verticalInput > 0 && !grounded && AboveGround())
        {
            WallRunMovement();
        }
        else
        {
            wallRunTimer = 0; 
        }



        //Move
        controller.Move((moveDir * speed + playerVel) * Time.deltaTime);

        //Shooting
        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
            shoot();
    }
    void shoot()
    {
        shootTimer = 0;
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }
    public void StartFlying()
    {
        isFlyingActive = true;
        jumpCount = 0;
        playerVel.y = jumpSpeed * gravityRush;
    }

    public void StopFlying()
    {
        isFlyingActive = false;
    }

    //Wallrun functions

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, maskWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, maskWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, maskGround);
    }

    private void WallRunMovement()
    {
        wallRunTimer += Time.deltaTime;

        // 1️⃣ Reduce gravity while wall running
        playerVel.y = -2f; // nhẹ hơn so với rơi bình thường

        // 2️⃣ Calculate forward direction along wall
        Vector3 wallForward = Vector3.Cross(wallLeft ? leftWallHit.normal : rightWallHit.normal, Vector3.up);

        if (Vector3.Dot(transform.forward, wallForward) < 0)
            wallForward = -wallForward;

        // 3️⃣ Apply movement along wall
        controller.Move(wallForward * speed * Time.deltaTime);

        // 4️⃣ Wall jump
        if (Input.GetButtonDown("Jump"))
        {
            Vector3 wallNormal = wallLeft ? leftWallHit.normal : rightWallHit.normal;
            playerVel.y = jumpSpeed; // vertical jump
            moveDir += wallNormal * speed; // push away from wall
            wallRunTimer = 0;
        }

        // 5️⃣ Limit wall run time
        if (wallRunTimer > maxWallRunTime)
        {
            wallRunTimer = 0;
        }
    }
}