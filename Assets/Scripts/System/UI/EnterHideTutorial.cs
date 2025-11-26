using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// チュートリアル用の文章を出す
/// </summary>
public class EnterHideTutorial : MonoBehaviour
{
    [SerializeField] private GameTutorialTextDisplay gameTextDisplay;

    private bool isText = false;

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
            }
        }
    }

    public async UniTask HideText(CancellationToken token)
    {
        using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
        CancellationToken linkedToken = linkedCts.Token;

        if (isText)
        {
            gameTextDisplay.HideText(linkedToken).Forget();
        }
    }
}
