using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;

    private async UniTask LoadStageWithBGMFadeOut(string sceneName)
    {
        soundManager.StopBGMFadeOut(1.0f).Forget();
        await UniTask.Delay(3000);
        SceneManager.LoadScene(sceneName);
    }

    public void LoadStage2()
    {
        LoadStageWithBGMFadeOut("Stage2");
    }

    // 指定したステージに遷移
    public void LoadStage(int stageNumber)
    {
        string sceneName = $"{"Stage"}{stageNumber}";
        UnityEngine.Debug.Log(sceneName + "に遷移します");
        LoadStageWithBGMFadeOut(sceneName);
    }
    public void LoadTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void LoadSaveScene(int currentStage)
    {
        if (currentStage < 1 || currentStage > 5)
        {
            UnityEngine.Debug.LogError("現在のステージ番号が不正です: " + currentStage);
            return;
        }
        string sceneName = $"Save{currentStage}to{currentStage + 1}";
        SceneManager.LoadScene(sceneName);
    }

    public void LoadPlorogueToStage1()
    {
        SceneManager.LoadScene("Stage1");
    }
    public void LoadPlorogue()
    {
        SceneManager.LoadScene("Prologue");
    }
    
    public void LoadEpilogue()
    {
        SceneManager.LoadScene("Epilogue");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
