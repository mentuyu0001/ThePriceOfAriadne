using UnityEngine;

/// <summary>
/// SEとBGMデータを保持するシングルトン
/// </summary>
public class VolumeData : MonoBehaviour
{
    public static VolumeData Instance { get; private set; }

    [Header("BGM音量")]
    [Range(0f, 1f)]
    public float bgmVolume = 1f;

    [Header("SE音量")]
    [Range(0f, 1f)]
    public float seVolume = 1f;

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
}
