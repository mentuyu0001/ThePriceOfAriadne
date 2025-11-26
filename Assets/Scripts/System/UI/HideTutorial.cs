using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// チュートリアル用の文章を出す
/// </summary>
public class HideTutorial : MonoBehaviour
{
    [SerializeField] private GameTutorialTextDisplay gameTextDisplay;

    private bool isText = false;

    private CancellationToken dct; // DestroyCancellationToken

    void Start()
    {
        dct = this.GetCancellationTokenOnDestroy();
    }

    async void OnTriggerEnter2D(Collider2D other)
    {
        if (!isText)
        {
            if (other.gameObject.tag == "Player")
            {
                isText = true;
                gameTextDisplay.HideText(dct).Forget();
            }
        }
    }
}
