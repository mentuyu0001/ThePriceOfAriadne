using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

/// <summary>
/// チュートリアル用の文章を出す
/// </summary>
public class ShowTutorial : MonoBehaviour
{
    [SerializeField] private GameTutorialTextDisplay gameTextDisplay;

    [SerializeField] private string tutorialText;

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
                gameTextDisplay.ShowText(tutorialText, token: dct).Forget();   
            }
        }
    }
}
