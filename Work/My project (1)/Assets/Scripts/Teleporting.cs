using UnityEngine;
using System.Collections;
using UnityEngine.VFX;

public class Teleporting : MonoBehaviour
{
    [SerializeField] Renderer model;

    Color colorOrig;

    private void Start()
    {
        colorOrig = model.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gameManager.instance.playerSpawnPos2.transform.position != transform.position) 
        {
            gameManager.instance.playerScript.teleporter(); //spawn player once it hits checkpoint back to beginning
            StartCoroutine(checkpointFeedback());
        }
    }

    IEnumerator checkpointFeedback()
    {
        gameManager.instance.Teleporterpopup.SetActive(true); //turn on
        model.material.color = Color.blueViolet;
        yield return new WaitForSeconds(1.25f); //how long we wait
        model.material.color = colorOrig;
        gameManager.instance.Teleporterpopup.SetActive(false); //turn it off
    }

}
