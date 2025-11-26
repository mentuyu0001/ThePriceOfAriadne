using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using System.Threading;

public class FirstShowTutorial : MonoBehaviour
{
    [SerializeField] private GameTutorialTextDisplay gameTextDisplay;
    [SerializeField] private string tutorialText1;
    [SerializeField] private string tutorialText2;

    private bool isText = false;

    float secondsToWait = 4.0f;

    private CancellationToken dct; // DestroyCancellationToken

    void Start()
    {
        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isText)
        {
            if (other.gameObject.tag == "Player")
            {
                isText = true;
                ShowAndHideTexts(dct).Forget();
            }
        }
    }
    
    private async UniTask ShowAndHideTexts(CancellationToken token)
    {
        await gameTextDisplay.ShowText(tutorialText1, token: dct);

        await UniTask.Delay(TimeSpan.FromSeconds(secondsToWait), cancellationToken: token);

        gameTextDisplay.HideText(dct).Forget();

        await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: token);

        await gameTextDisplay.ShowText(tutorialText2, token: dct);
    }
}
