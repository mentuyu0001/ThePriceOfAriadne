using System.Diagnostics;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("BGM音源")]
    [SerializeField]private AudioSource bgmSource;
    [SerializeField] private AudioClip[] bgmClips;

    [Header("SE音源")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip[] seClips;

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

    // BGM停止
    public void StopBGM()
    {
        if (bgmSource != null)
        {
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
                seSource.clip = seClips[index];
                seSource.loop = true;
                seSource.volume = VolumeData.Instance.seVolume;
                seSource.Play();
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
            seSource.Stop();
            seSource.loop = false;
            seSource.clip = null;
        }
    }

    // AudioClip直接指定でSE再生（ループ指定可能）
    public void PlaySE(AudioClip clip, bool loop = false)
    {
        if (clip != null && seSource != null)
        {
            if (loop)
            {
                seSource.clip = clip;
                seSource.loop = true;
                seSource.volume = VolumeData.Instance.seVolume;
                seSource.Play();
            }
            else
            {
                seSource.loop = false;
                seSource.volume = VolumeData.Instance.seVolume;
                seSource.PlayOneShot(clip);
            }
        }
    }
}
