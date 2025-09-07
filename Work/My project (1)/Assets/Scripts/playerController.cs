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
    [SerializeField] float smoothY ;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        movement();
        
    }
    void movement()
    {
        shootTimer += Time.deltaTime;

        //movedir
        moveDir = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

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

        //Flying
        if (!isFlyingActive)
        {
            if (controller.isGrounded && playerVel.y < 0)
                playerVel.y = -2f;

            if (Input.GetButtonDown("Jump") && controller.isGrounded)
                playerVel.y = jumpSpeed;

            playerVel.y -= gravity * Time.deltaTime;
        }
        else
        {
            float targetY = Input.GetButton("Jump") ? jumpSpeed * gravityRush : 0f;
            playerVel.y = Mathf.Lerp(playerVel.y, targetY, smoothY * Time.deltaTime);
            controller.Move((moveDir * speed + playerVel) * Time.deltaTime);
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
/*            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }*/
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
}