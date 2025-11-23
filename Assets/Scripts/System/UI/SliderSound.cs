using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SliderSound : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Slider mySlider;
    private bool isDragging = false; // 今マウスで掴んでいるか？

    void Awake()
    {
        mySlider = GetComponent<Slider>();
    }

    void OnEnable()
    {
        // 値が変わった時のイベントに登録
        mySlider.onValueChanged.AddListener(OnValueChanged);
    }

    void OnDisable()
    {
        // イベント解除（エラー防止）
        mySlider.onValueChanged.RemoveListener(OnValueChanged);
    }

    // -------------------------------------------------------
    // ■ 値が変わった時の処理
    // -------------------------------------------------------
    private void OnValueChanged(float value)
    {
        if (isDragging)
        {
            return; 
        }

        PlaySound();
    }

    // -------------------------------------------------------
    // ■ マウス操作の検知
    // -------------------------------------------------------

    // マウスで掴んだ瞬間
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        PlaySound();
    }

    // マウスを離した瞬間（ご希望の機能はここ！）
    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;

        PlaySound();
    }

    // -------------------------------------------------------
    // ■ 音再生処理
    // -------------------------------------------------------
    private void PlaySound()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(14); 
        }
    }
}
