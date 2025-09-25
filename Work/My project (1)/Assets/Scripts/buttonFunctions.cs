using UnityEngine;
using UnityEngine.SceneManagement;
public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.stateResume();
    }
    public void restart()
    {
        //bad version of restart for time's sake
        Debug.Log("Clicked Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.stateResume();
    }
    public void quit()
    {
        //doesnt close down unity 
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        //this closes unity down, not good for testing
        Application.Quit();
#endif
    }

    public void respawn()
    {
        gameManager.instance.playerScript.spawnPlayer(); ///check player movement 
        gameManager.instance.stateResume(); 
    }
}
