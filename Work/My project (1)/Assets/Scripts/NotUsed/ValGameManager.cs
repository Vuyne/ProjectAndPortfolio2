using UnityEngine;
using TMPro;

public class ValGameManager : MonoBehaviour
{
    public static ValGameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    float timeScaleOrig;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake() 
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player"); 
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel")) ///cancel is for escape in unity
        {
            if (menuActive == null) //if theres nothing on the screen we can pause
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause) //if the screen is paused we can unpaused
            {
                stateUnpause();
            }
        }
    }


    public void statePause() ///to pause the game
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig; 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        ///turn off the menu of pause when we are unpaused
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void winMenu() 
    {
        statePause();
        menuActive = menuWin; 
        menuActive.SetActive(true);
    }

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
