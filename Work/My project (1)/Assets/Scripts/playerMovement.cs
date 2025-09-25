using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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
    [SerializeField] GameObject arrow;
    [SerializeField] int arrowSpeed;
    Vector3 move;

    float shootTimer;
    int jumpCount;
    int HPOrig;

    bool isMoving;

    bool isSprinting;
    bool isPlayingSteps;

    

    [Header("Footstep")]
    
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
    [Header("Audio")]
    AudioSource footsteps;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [UnityEngine.Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audHurt;
    [UnityEngine.Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audJump;
    [UnityEngine.Range(0, 1)][SerializeField] float audJumpVol;
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

         move = (transform.right * h + transform.forward * v).normalized;


        Vector3 newVel = new Vector3(move.x * speed, rb.linearVelocity.y, move.z * speed);
        rb.linearVelocity = newVel;

        jump();

       
        if (Input.GetButton("Fire1") && gunList.Count > 0 && gunList[gunListPos].ammoCur > 0 && shootTimer >= shootRate)
            shoot();

        selectGun();
        reload();
    }

    void jump()
    {

        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax)
        {
           aud.PlayOneShot(audJump[UnityEngine.Random.Range(0, audJump.Length)], audJumpVol);

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
            aud.PlayOneShot(gunList[gunListPos].shootSound[UnityEngine.Random.Range(0, gunList[gunListPos].shootSound.Length)], gunList[gunListPos].shootSoundVol);
            if (gunList[gunListPos].isCrossbow)
            {
                GameObject newArrow = Instantiate(arrow, weaponPos.position, weaponPos.rotation);

                Rigidbody rbArrow = newArrow.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 Hitdir = (hit.point - weaponPos.position).normalized;
                    rbArrow.linearVelocity = Hitdir * arrowSpeed;
                    newArrow.transform.forward = Hitdir;
                    
                }
                Destroy(newArrow, 5f);
            }
            else
            {
                Instantiate(gunList[gunListPos].hitEffect, hit.point, quaternion.identity);
            }

            Debug.Log(hit.collider.name);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if (dmg != null)
            {
                dmg.takeDamage(shootDamage);
              //  cam.FireKick();
            }
            gunList[gunListPos].ammoCur--;
            updatePlayerUI();
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
        aud.PlayOneShot(audHurt[UnityEngine.Random.Range(0, audHurt.Length)], audHurtVol);

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
        if (gunList.Count > 0)
        {
            gameManager.instance.ammoCur.text = gunList[gunListPos].ammoCur.ToString("F0");
            gameManager.instance.ammoMax.text = gunList[gunListPos].ammoMax.ToString("F0");
        }
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
            if (move.normalized.magnitude > 0.3 && !isPlayingSteps)
            {
                StartCoroutine(playStep());
            }
            jumpCount = 0;
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
            GrapplingGun grappling = Hook.GetComponent<GrapplingGun>();
            if (grappling != null)
            {
                grappling.enabled = true;
            }

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
        updatePlayerUI();

        //gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[gunListPos].gunModel.GetComponent<MeshFilter>().sharedMesh;
        // gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[gunListPos].gunModel.GetComponent<MeshRenderer>().sharedMaterial;


    }

    public void spawnPlayer()
    {
        transform.position = gameManager.instance.playerSpawnPos.transform.position; 
        HP = HPOrig;
        updatePlayerUI();
    }
    public void teleporter()
    {
        transform.position = gameManager.instance.playerSpawnPos2.transform.position;
        HP = HPOrig;
        updatePlayerUI();
    }

    IEnumerator playStep()
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[UnityEngine.Random.Range(0, audSteps.Length)], audStepsVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);

        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isPlayingSteps = false;
    }


}
