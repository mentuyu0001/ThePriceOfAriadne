using Cysharp.Threading.Tasks;
using UnityEngine;
using System.Threading;

public class BGMStarter : MonoBehaviour
{
    [SerializeField] private SoundManager soundManager;

    [SerializeField] private int bgmIndex;

    [SerializeField] private bool doFadein;

    [SerializeField] private float fadeInDuration;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CancellationToken dct = this.GetCancellationTokenOnDestroy();

        if (doFadein)
        {
            soundManager.PlayBGMFadeIn(bgmIndex, fadeInDuration, dct).Forget();
        }
        else
        {
            soundManager.PlayBGM(bgmIndex);
        }
    }
}
