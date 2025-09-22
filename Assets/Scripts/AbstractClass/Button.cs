using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UIボタンの抽象クラス
/// </summary>
public abstract class Button : MonoBehaviour, IPointerClickHandler
{
    // クリック処理の抽象メソッド
    protected abstract void OnClick();

    // UIボタンがクリックされた時に呼ばれる
    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
}
