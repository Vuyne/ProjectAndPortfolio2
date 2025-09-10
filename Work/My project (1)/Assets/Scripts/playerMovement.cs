using UnityEngine;
using System.Collections;

public class playerMovement : MonoBehaviour, IDamage
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] Rigidbody rb;

    [SerializeField] int HP;
    [SerializeField] float speed;
    [SerializeField] float sprintMod;
    [SerializeField] float jumpForce;
    [SerializeField] int jumpMax;

    [SerializeField] int shootDamage;
    [SerializeField] float shootRate;
    [SerializeField] float shootDist;

    float shootTimer;
    int jumpCount;
    int HPOrig;

    bool isSprinting;
    [Header("Wall Run")]
    public LayerMask maskWall;

    public float wallRunForce = 5f;
    public float maxWallRunTime = 1.5f;
    public float wallCheckDistance = 1f;
    public float minJumpHeight = 1.5f;

    //things to check wall existence
    private bool wallLeft;
    private bool wallRight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private float wallRunTimer;


    void Start()
    {
        HPOrig = HP;
        updatePlayerUI();
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        movement();
        sprint();

        if (Input.GetButton("Fire1") && shootTimer >= shootRate)
            shoot();
    }

    void movement()
    {
        shootTimer += Time.deltaTime;

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * h + transform.forward * v).normalized;


        Vector3 newVel = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = newVel;

        jump();

        CheckForWall();

        if ((wallLeft || wallRight) && CanWallRun())

        {
            WallRunMovement();
        }
        else
        {
            wallRunTimer = 0;
        }
    }

    void jump()
    {

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void sprint()
    {
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

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashDamage());
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }

    IEnumerator flashDamage()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }
    private bool CanWallRun()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight);
    }

    private void CheckForWall()
    {
        Vector3 camRight = new Vector3(Camera.main.transform.right.x, 0f, Camera.main.transform.right.z).normalized;
        wallRight = Physics.Raycast(transform.position, transform.right, out rightWallHit, wallCheckDistance, maskWall);
        wallLeft = Physics.Raycast(transform.position, -transform.right, out leftWallHit, wallCheckDistance, maskWall);

        Debug.DrawRay(transform.position, transform.right * wallCheckDistance, wallRight ? Color.green : Color.blue);
        Debug.DrawRay(transform.position, -transform.right * wallCheckDistance, wallLeft ? Color.green : Color.blue);
    }

    private void WallRunMovement()
    {
        // tăng timer
        wallRunTimer += Time.deltaTime;

        if (wallRunTimer > maxWallRunTime)
        {
            wallRunTimer = 0;
            return;
        }

        // giữ tốc độ rơi chậm lại
        Vector3 velocity = rb.linearVelocity;
        velocity.y = Mathf.Max(velocity.y, -2f);

        // lấy normal của tường
        Vector3 wallNormal = wallLeft ? leftWallHit.normal : rightWallHit.normal;

        // tính hướng chạy dọc tường
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);
        if (Vector3.Dot(wallForward, transform.forward) < 0)
            wallForward = -wallForward;

        // nếu có input thì chạy theo tường
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            rb.linearVelocity = wallForward * speed + new Vector3(0, velocity.y, 0);
        }
        else
        {
            // tụt chậm xuống thay vì treo
            rb.linearVelocity = new Vector3(0, -1f, 0);
        }

        // wall jump
        if (Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = wallForward * speed
                              + wallNormal * wallRunForce
                              + Vector3.up * jumpForce;
            jumpCount = 1; // reset lại nhảy trên tường, tránh vô hạn jump
            wallRunTimer = 0;
        }
    }
}
