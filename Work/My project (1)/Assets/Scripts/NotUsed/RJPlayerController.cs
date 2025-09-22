using System.Collections;
using UnityEngine;

public class RJPlayerController : MonoBehaviour, IDamage
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
    public float noiseLevel;
    public float noiseRadius;
    float maxWalkingNoiseLvl;
    float noiseRadiusOrig;
    int HPOrig;
    int jumpCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        maxWalkingNoiseLvl = 5f;
        noiseRadius = 5f;
        noiseRadiusOrig = noiseRadius;
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
        if (isMoving && !isSprinting)
        {
            if (noiseLevel == maxWalkingNoiseLvl)
            {

            }
            else if (noiseLevel < maxWalkingNoiseLvl)
                noiseLevel += Time.deltaTime;
            else if (noiseLevel > maxWalkingNoiseLvl)
                noiseLevel -= Time.deltaTime;
        }
        else if (isSprinting)
        {
            noiseLevel += Time.deltaTime * 2;
        }
        else if (noiseLevel > 0 && !isMoving && !isSprinting)
        {
            noiseLevel -= Time.deltaTime * 2;
        }
        noiseRadiusUpdate();
    }
    void noiseRadiusUpdate()
    {
        if (noiseLevel > 15f && isSprinting)
        {
            noiseRadius += Time.deltaTime;
        }
        else if (isMoving && !isSprinting)
        {
            if (noiseRadius > noiseRadiusOrig)
                noiseRadius -= Time.deltaTime;
            
        }
        else if (!isMoving && !isSprinting)
        {
            if (noiseRadius > noiseRadiusOrig)
                noiseRadius -= Time.deltaTime * 2;
        }
    }

    public void takeDamage(int damage)
    {
       HP -= damage;
       updatePlayerUI();
       StartCoroutine(playerFlashDamage());
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }
    public void updatePlayerUI()
    {
        gameManager.instance.playerHPBar.fillAmount = (float)HP / HPOrig;
    }
    IEnumerator playerFlashDamage()
    {
        gameManager.instance.playerDamageFlash.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageFlash.SetActive(false);
    }
}
