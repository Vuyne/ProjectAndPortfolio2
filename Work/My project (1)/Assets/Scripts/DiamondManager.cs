using UnityEngine;
using TMPro;

public class DiamondManager : MonoBehaviour 
{
    public TMP_Text counterText;
    int start, remaining;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start = GameObject.FindGameObjectsWithTag("Diamond").Length; ///all diamonds taged in the game
        remaining = start; ///first all diamonds are remaining
        updateHelpermenu(); 
    }

   public void PickUpone()
    {
        remaining = Mathf.Max(remaining - 1, 0); /// not less than 0;
        updateHelpermenu();
    } 

    void updateHelpermenu() 
    {
        counterText.text = $"{remaining}/{start}";
    }
}
