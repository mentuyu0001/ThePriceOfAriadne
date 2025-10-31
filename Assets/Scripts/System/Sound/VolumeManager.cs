using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider seSlider;

    void Start()
    {
        // 初期値をVolumeDataから取得
        if (VolumeData.Instance != null)
        {
            bgmSlider.value = VolumeData.Instance.bgmVolume;
            seSlider.value = VolumeData.Instance.seVolume;
        }

        // スライダーの値変更時に音量を更新
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        seSlider.onValueChanged.AddListener(OnSeSliderChanged);
    }

    private void OnBgmSliderChanged(float value)
    {
        if (VolumeData.Instance != null)
        {
            VolumeData.Instance.bgmVolume = value;
        }
    }

    private void OnSeSliderChanged(float value)
    {
        if (VolumeData.Instance != null)
        {
            VolumeData.Instance.seVolume = value;
        }
    }
}
