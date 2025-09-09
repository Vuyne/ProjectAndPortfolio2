using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Collections;

public class RJPlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [SerializeField] int speed;
    [SerializeField] int sprint;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] int gravity;
    [SerializeField] int HP;


    AudioSource footsteps;
    Vector3 playerVel;
    Vector3 moveDir;
    bool isSprinting;
    bool isMoving;
    float noiseLevel;
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
        noiseUpdate();
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
        if (moveDir == Vector3.zero)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
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
    void noiseUpdate()
    {
        if (isMoving == true && isSprinting == false)
        {
            noiseLevel += Time.deltaTime;
        }
        else if (isSprinting == true)
        {
            noiseLevel += Time.deltaTime * 2;
        }
        else if (noiseLevel > 0 && isMoving == false && isSprinting == false)
        {
            noiseLevel -= Time.deltaTime;
        }
    }
}
