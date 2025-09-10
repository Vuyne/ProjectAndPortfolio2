using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] TMP_Text gameCountText;

    public Image playerHPBar;
    public GameObject playerDamageFlash;
    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    int gameGoalCount;
    bool playerAtEnd;

    float timeScaleOrig;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
    }

    // Update is called once per frame
    void Update()
    {
        //use list to manage multiple menus

        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateResume();
            }
        }
    }
    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
    public void stateResume()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }
    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;
        gameCountText.text = gameGoalCount.ToString("F0");
        if (gameGoalCount < 0)
            gameGoalCount = 0;

        CheckWinCondition();
    }

    // Call this when player reaches the end trigger
    public void PlayerReachedEnd()
    {
        playerAtEnd = true;
        CheckWinCondition();
    }
    public void PlayerLeftEnd()
    {
        playerAtEnd = false;
    }

    // Check if all enemies killed and player reached end
    public void CheckWinCondition() 
    {
        if (gameGoalCount == 0 && playerAtEnd)
        {
            // You win: pause and show win menu
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }

    }


    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
}
