using UnityEngine;
using System.Collections;

public class damageZombie : Damage
{

    [SerializeField] int contactDmg = 1;
    [SerializeField] float contactCooldown_ = 0.6f;
    bool canHit = true;
    Transform ownerRoot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        ownerRoot = transform.root;

        //makes sure it triggers
        var collider_ = GetComponent<Collider>();
        if (collider_) collider_.isTrigger = true;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Hit by: {other.name} (root: {other.transform.root.name})", other);

        if (!canHit) return;
        if (other.isTrigger) return;
        if (!other.CompareTag("Player")) return;
        if (other.transform.root == ownerRoot) return;
       
        IDamage dmg = other.attachedRigidbody ? other.attachedRigidbody.GetComponent<IDamage>() 
            : other.GetComponentInParent<IDamage>(); 

        if(dmg != null)
        {
            dmg.takeDamage(contactDmg);
            StartCoroutine(ContactCooldown());
        }

    }

    IEnumerator ContactCooldown()
    {
        canHit = false;
        yield return new WaitForSeconds(contactCooldown_);
        canHit = true;
    }

}
