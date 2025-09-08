using UnityEngine;

public class itemTrigger : MonoBehaviour
{
    public GameObject knight;           
    public playerController player;    

    private bool activated = false;

    void OnMouseDown()
    {
        if(Input.GetButton("Fire1"))
        if (!activated)
        {
            activated = true;

            knight.SetActive(true);

            // Bắt đầu combat / Gravity Rush
            player.StartFlying();

            Debug.Log("Knight spawned, Flying");
        }
    }
}
