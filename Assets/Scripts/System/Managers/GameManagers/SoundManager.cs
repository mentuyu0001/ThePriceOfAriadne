using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM音源")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioClip[] bgmClips;

    [Header("SE音源")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioSource seSourceLoop;
    [SerializeField] private AudioSource seSourceLoop2;
    [SerializeField] private AudioClip[] seClips;

    public float SEVolume => seSource != null ? seSource.volume : 0f;

    private CancellationToken dct; // DestroyCancellationToken

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // DestroyCancellationTokenの取得 このオブジェクトが破棄されるとキャンセルされる
        dct = this.GetCancellationTokenOnDestroy();
    }

    // BGM再生（ループ指定可能）
    public void PlayBGM(int index, bool loop = true)
    {
        if (bgmClips != null && index >= 0 && index < bgmClips.Length)
        {
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = loop;
            bgmSource.volume = VolumeData.Instance.bgmVolume;
            bgmSource.Play();
        }
    }

    // フェードインしながらBGM再生（ループ指定可能）
    public async UniTask PlayBGMFadeIn(int index, float duration, CancellationToken token, bool loop = true)
    {
        if (bgmClips != null && index >= 0 && index < bgmClips.Length)
        {
            bgmSource.clip = bgmClips[index];
            bgmSource.loop = loop;
            bgmSource.volume = 0f;
            bgmSource.Play();

            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
            CancellationToken linkedToken = linkedCts.Token;

            float dt = 0.01f;
            float volumeStep = VolumeData.Instance.bgmVolume / duration * dt;
            while (bgmSource.volume < VolumeData.Instance.bgmVolume && !linkedToken.IsCancellationRequested)
            {
                bgmSource.volume += volumeStep;
                await UniTask.Delay(TimeSpan.FromSeconds(dt), cancellationToken: linkedToken);
            }
        }
    }

    // BGM停止
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    // フェードアウトしながらBGM停止
    public async UniTask StopBGMFadeOut(float duration, CancellationToken token)
    {
        if (bgmSource != null && bgmSource.isPlaying)
        {
            using CancellationTokenSource linkedCts = CancellationTokenSource.CreateLinkedTokenSource(token, dct);
            CancellationToken linkedToken = linkedCts.Token;

            float dt = 0.01f;
            float volumeStep = bgmSource.volume / duration * dt;
            while (bgmSource.volume > 0f && !linkedToken.IsCancellationRequested)
            {
                bgmSource.volume -= volumeStep;
                await UniTask.Delay(TimeSpan.FromSeconds(dt), cancellationToken: linkedToken, ignoreTimeScale: true);
            }

            bgmSource.Stop();
        }
    }

    // SE再生（ループ指定可能）
    public void PlaySE(int index, bool loop = false)
    {
        UnityEngine.Debug.Log("PlaySE called with index: " + index);
        if (seClips != null && index >= 0 && index < seClips.Length)
        {
            if (loop)
            {
                seSourceLoop.clip = seClips[index];
                seSourceLoop.loop = true;
                seSourceLoop.volume = VolumeData.Instance.seVolume;
                seSourceLoop.Play();
            }
            else
            {
                seSource.loop = false;
                seSource.volume = VolumeData.Instance.seVolume;
                seSource.PlayOneShot(seClips[index]);
            }
            UnityEngine.Debug.Log("Played SE: " + seClips[index].name);
        }
    }

    // SE停止（ループSE用）
    public void StopSE()
    {
        if (seSource != null)
        {
            seSourceLoop.Stop();
            seSourceLoop.clip = null;
        }
    }

    // AudioClip直接指定でSE再生（ループ指定可能）
    public void PlaySE(AudioClip clip, bool loop = false)
    {
        if (clip != null && seSource != null)
        {
            if (loop)
            {
                seSourceLoop.clip = clip;
                seSourceLoop.loop = true;
                seSourceLoop.volume = VolumeData.Instance.seVolume;
                seSourceLoop.Play();
            }
            else
            {
                seSource.loop = false;
                seSource.volume = VolumeData.Instance.seVolume;
                seSource.PlayOneShot(clip);
            }
        }
    }

    public void PlayLoopSE2(int index)
    {
        UnityEngine.Debug.Log("PlaySE called with index: " + index);
        if (seClips != null && index >= 0 && index < seClips.Length) {
            seSourceLoop2.clip = seClips[index];
            seSourceLoop2.loop = true;
            seSourceLoop2.volume = VolumeData.Instance.seVolume;
            seSourceLoop2.Play();
        }
    }

    public void StopLoopSE2()
    {
        if (seSource != null)
        {
            seSourceLoop2.Stop();
            seSourceLoop2.clip = null;
        }
    }
}
