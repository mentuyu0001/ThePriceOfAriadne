using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class SoundFadeButton : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;
    [SerializeField] private float duration = 2.5f;

    private CancellationToken dct; // DestroyCancellationToken

    void Start()
    {
        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
    }

    public void OnButtonClicked()
    {
        soundManager.StopBGMFadeOut(duration, dct).Forget();
    }
}
