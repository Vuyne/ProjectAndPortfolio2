using UnityEngine;
using TMPro;

public class DiamondManager : MonoBehaviour 
{
    public TMP_Text counterText;
    [SerializeField] ValGameManager gm; //new

    int start, remaining;

    void Awake() //new
    {
        if (gm == null)
        { 
            gm = ValGameManager.instance;
        }
    }
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

        //new
        if( remaining == 0 )
        {
            gameManager.instance?.CheckWinCondition(); //get win menu here  
        }
    } 

    void updateHelpermenu() 
    {
        counterText.text = $"{remaining}/{start}";
    }

    
}
