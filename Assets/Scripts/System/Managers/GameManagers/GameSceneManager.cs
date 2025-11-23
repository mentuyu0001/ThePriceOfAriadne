using System.Diagnostics;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private FadeController fadeController;

    float fadeDuration = 3.0f;

    private static bool isLoading = false;

    private async UniTask LoadStageAsync(string sceneName)
    {
        if (isLoading) return;

        isLoading = true;
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        var bgmFadeTask = soundManager.StopBGMFadeOut(fadeDuration);
        var visualFadeTask = fadeController.FadeOut(fadeDuration);

        await UniTask.WhenAll(bgmFadeTask, visualFadeTask);

        await UniTask.WaitUntil(() => op.progress >= 0.9f);

        op.allowSceneActivation = true;
        isLoading = false;
    }

    public void LoadStage2()
    {
        LoadStageAsync("Stage2").Forget();
    }

    // 指定したステージに遷移
    public void LoadStage(int stageNumber)
    {
        string sceneName = $"Stage{stageNumber}";
        UnityEngine.Debug.Log(sceneName + "に遷移します");
        LoadStageAsync(sceneName).Forget();
    }
    public void LoadTitle()
    {
        LoadStageAsync("TitleScene").Forget();
    }

    public void LoadSaveScene(int currentStage)
    {
        if (currentStage < 1 || currentStage > 5)
        {
            UnityEngine.Debug.LogError("現在のステージ番号が不正です: " + currentStage);
            return;
        }
        string sceneName = $"Save{currentStage}to{currentStage + 1}";
        LoadStageAsync(sceneName).Forget();
    }

    public void LoadPlorogueToStage1()
    {
        LoadStageAsync("Stage1").Forget();
    }
    public void LoadPlorogue()
    {
        LoadStageAsync("Prologue").Forget();
    }
    
    public void LoadEpilogue()
    {
        LoadStageAsync("Epilogue").Forget();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
