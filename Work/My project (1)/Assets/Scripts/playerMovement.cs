using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    bool isMoving;

    bool isSprinting;
   /*[Header("Wall Run")]
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
    private float wallRunTimer;*/

    [Header("Footstep")]
    AudioSource footsteps;
    public float noiseLevel;
    public float noiseRadius;
    float maxWalkingNoiseLvl;
    float noiseRadiusOrig;

    private cameraController cam;

    [Header("Gun")]
    [SerializeField] GameObject gunModel;
    GameObject currentGun;
    [SerializeField] List<gunStats> gunList = new List<gunStats>();
    List<GameObject> gunInstances = new List<GameObject>();
    public Transform weaponPos;
    public Transform grabPos;
    int gunListPos;

    void Start()
    {

        HPOrig = HP;
        spawnPlayer();
        //updatePlayerUI();
    }

    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);

        if (!(gameManager.instance.isPaused))
         {
            movement();
        }

        sprint();

        noiseUpdate();
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

        //CheckForWall();

        /*if ((wallLeft || wallRight) && CanWallRun())

        {
            WallRunMovement();
        }
        else
        {
            wallRunTimer = 0;
        }
        if (move == Vector3.zero)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }*/
        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();

        selectGun();
        reload();
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
            footsteps = GetComponent<AudioSource>();
            footsteps.Play();
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
            footsteps.Stop();
        }
    }

    void shoot()
    {
        shootTimer = 0;
      // GetComponent<CameraFOV>().FireKick();

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
              //  cam.FireKick();
            }
        }
    }
    void reload()
    {
        if (Input.GetButtonDown("Reload"))
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        updatePlayerUI();
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
    /*private bool CanWallRun()
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
        // timer
        wallRunTimer += Time.deltaTime;

        if (wallRunTimer > maxWallRunTime)
        {
            wallRunTimer = 0;
            return;
        }

        // Reduce gravity
        Vector3 velocity = rb.linearVelocity;
        velocity.y = Mathf.Max(velocity.y, -2f);

        // Normal Wall
        Vector3 wallNormal = wallLeft ? leftWallHit.normal : rightWallHit.normal;

        // Wall Forward
        Vector3 wallForward = Vector3.Cross(wallNormal, Vector3.up);
        if (Vector3.Dot(wallForward, transform.forward) < 0)
            wallForward = -wallForward;

        // Goforward to wall
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
            rb.linearVelocity = wallForward * speed + new Vector3(0, velocity.y, 0);
        }
        else
        {
            // or fall a bit
            rb.linearVelocity = new Vector3(0, -1f, 0);
        }

        // wall jump
        if (Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = wallForward * speed
                              + wallNormal * wallRunForce
                              + Vector3.up * jumpForce;
            jumpCount = 1; 
            wallRunTimer = 0;
        }
    }*/
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
    void selectGun()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < gunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }
    /*void reload()
    {
        if (Input.GetButtonDown("Reload"))
            gunList[gunListPos].ammoCur = gunList[gunListPos].ammoMax;
        updatePlayerUI();
    }*/
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player collided with: " + other.name);
        if (other.TryGetComponent<IPickup>(out var pickup))
        {
            pickup.OnPickup(gameObject);
        }
    }
    public void getGunStats(gunStats gun)
    {
        if (gun.Grappable == true)
        {
            Debug.Log("Grab!!");
            GameObject Hook = Instantiate(gun.gunModel, grabPos);

        }
        else
        {
            gunList.Add(gun);

            GameObject newGun = Instantiate(gun.gunModel, weaponPos);
            newGun.SetActive(false);
            gunInstances.Add(newGun);
            gunListPos = gunList.Count - 1;

            changeGun();
        }
    }

    void changeGun()
    {
        for (int i = 0; i < gunInstances.Count; i++)
        {
            gunInstances[i].SetActive(false);
        }

      
        gunInstances[gunListPos].SetActive(true);
        currentGun = gunInstances[gunListPos];

        shootDamage = gunList[gunListPos].shootDamage;
        shootDist = gunList[gunListPos].shootDist;
        shootRate = gunList[gunListPos].shootRate;

        //gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
       // gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;


    }

    public void spawnPlayer()
    {
        transform.position = gameManager.instance.playerSpawnPos.transform.position; 
        HP = HPOrig;
        updatePlayerUI();
    }

}
