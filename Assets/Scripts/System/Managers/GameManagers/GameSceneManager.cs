using System.Diagnostics;
using System.Threading;
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

    private CancellationToken dct; // DestroyCancellationToken

    void Start()
    {
        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
    }

    private async UniTask LoadStageAsync(string sceneName, CancellationToken token)
    {
        if (isLoading) return;

        isLoading = true;
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        var bgmFadeTask = soundManager.StopBGMFadeOut(fadeDuration, token);
        var visualFadeTask = fadeController.FadeOut(fadeDuration);

        await UniTask.WhenAll(bgmFadeTask, visualFadeTask);

        await UniTask.WaitUntil(() => op.progress >= 0.9f, cancellationToken: token);

        op.allowSceneActivation = true;
        isLoading = false;
    }

    public void LoadStage2()
    {
        LoadStageAsync("Stage2", dct).Forget();
    }

    // 指定したステージに遷移
    public void LoadStage(int stageNumber)
    {
        string sceneName = $"Stage{stageNumber}";
        UnityEngine.Debug.Log(sceneName + "に遷移します");
        LoadStageAsync(sceneName, dct).Forget();
    }
    public void LoadTitle()
    {
        LoadStageAsync("TitleScene", dct).Forget();
    }

    public void LoadSaveScene(int currentStage)
    {
        if (currentStage < 1 || currentStage > 5)
        {
            UnityEngine.Debug.LogError("現在のステージ番号が不正です: " + currentStage);
            return;
        }
        string sceneName = $"Save{currentStage}to{currentStage + 1}";
        LoadStageAsync(sceneName, dct).Forget();
    }

    public void LoadPlorogueToStage1()
    {
        LoadStageAsync("Stage1", dct).Forget();
    }
    public void LoadPlorogue()
    {
        LoadStageAsync("Prologue", dct).Forget();
    }
    
    public void LoadEpilogue()
    {
        LoadStageAsync("Epilogue", dct).Forget();
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
