using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ValEnemyAI : MonoBehaviour, ValIDamage
{
    [SerializeField] Renderer model;

    [SerializeField] int HP;

    Color colorOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color; 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(flasRed());
        }

        if (HP <= 0)
        {
            //GameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flasRed()
    {
        model.material.color = Color.red; //color of the model flash
        yield return new WaitForSeconds(0.1f); //time of the flash
        model.material.color = colorOrig;
    }

}
