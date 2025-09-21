using UnityEngine;
using TMPro;

public class DiamondManager : MonoBehaviour 
{
    public TMP_Text counterText;
    [SerializeField] gameManager gm; //new

    int start, remaining;

    void Awake() //new
    {
        if (gm == null)
        { 
            gm = gameManager.instance;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start = GameObject.FindGameObjectsWithTag("Diamond").Length; ///all diamonds taged in the game
        remaining = start; ///first all diamonds are remaining
        updateCountDiamond(); 
    }

   public void PickUpone()
    {
        remaining = Mathf.Max(remaining - 1, 0); /// not less than 0;
        updateCountDiamond();

        //new
        if( remaining == 0 )
        {
            gameManager.instance?.CheckWinCondition(); //get win menu here  
        }
    } 

    void updateCountDiamond() 
    {
        counterText.text = $"{remaining}/{start}"; 
    }

    
}
