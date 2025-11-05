using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public void LoadStage2()
    {
        SceneManager.LoadScene("Stage2");
    }

    // 指定したステージに遷移
    public void LoadStage(int stageNumber)
    {
        string sceneName = $"{"Stage"}{stageNumber}";
        UnityEngine.Debug.Log(sceneName + "に遷移します");
        SceneManager.LoadScene(sceneName);
    }
    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void LoadSaveScene(int currentStage)
    {
        if(currentStage < 1 || currentStage > 4)
        {
            UnityEngine.Debug.LogError("現在のステージ番号が不正です: " + currentStage);
            return;
        }
        string sceneName = $"Save{currentStage}to{currentStage + 1}";
        SceneManager.LoadScene(sceneName);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
