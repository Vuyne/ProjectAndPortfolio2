using UnityEngine;

public class RJPlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] int speed;
    [SerializeField] int sprint;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;

    AudioSource footsteps;
    Vector3 playerVel;
    Vector3 moveDir;
    bool isSprinting;
    int jumpCount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprinting();
    }
    void movement()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }
        moveDir = (Input.GetAxis("Horizontal") * transform.right) + (Input.GetAxis("Vertical") * transform.forward);
        controller.Move(moveDir * speed * Time.deltaTime);
        jump();
        controller.Move(playerVel * Time.deltaTime);

    }
   void sprinting()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprint;
            isSprinting = true;
            footsteps = GetComponent<AudioSource>();
            footsteps.Play();
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprint;
            isSprinting = false;
            footsteps.Stop();
        }
        
    }
    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
            jumpCount++;
            playerVel.y = jumpSpeed;
        }
    }
}
