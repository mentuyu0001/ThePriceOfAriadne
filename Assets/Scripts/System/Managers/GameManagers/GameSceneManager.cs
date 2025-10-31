using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public void LoadStage2()
    {
        SceneManager.LoadScene("Stage2New");
    }
    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
