using UnityEngine;
using UnityEngine.SceneManagement;

public class ValbuttonFunctions : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void resume()
    {
        ValGameManager.instance.stateUnpause();
    }

    public void restart()
    {
        ///not efficient in big projects. Is slow
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ValGameManager.instance.stateUnpause();
    }

    public void quit()
    {
        #if UNITY_EDITOR

            UnityEditor.EditorApplication.isPlaying = false; ///play in unity automatic
        #else
            Application.Quit();

        #endif
    }
}
