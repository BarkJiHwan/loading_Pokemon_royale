using UnityEngine;
using UnityEngine.SceneManagement;

public class StartAndExitButtonConttroler : MonoBehaviour
{
    public void LoadGameStart()
    {
        SceneManager.LoadScene("MainScene");
    }
    public void MainLoad()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void LoadExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
