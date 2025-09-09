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
}
