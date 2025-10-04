using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UIボタンの抽象クラス
/// </summary>
public abstract class Button : MonoBehaviour, ISelectHandler, IPointerEnterHandler 
{
    /// <summary>
    /// UIが選択されるたびに「カチカチ」と音を流したりする際の親オブジェクト
    /// </summary>

    // 選択された際の音以外の処理を施す抽象メソッド
    protected virtual void TriggerSelectionEffects() 
    {
        return;
    }

    // 決定時の処理を施す抽象メソッド
    public abstract void OnClick();

    // 選択された際に音を鳴らす処理
    private void TriggerSelectionSounds()
    {
        // 効果音を鳴らす
        Debug.Log("カチっと音が鳴った");
    }

    // キーボードで選択された場合
    public void OnSelect(BaseEventData eventData)
    {
        TriggerSelectionSounds();
        TriggerSelectionEffects();
    }

    // マウスで選択された場合
    public void OnPointerEnter(PointerEventData eventData)
    {
        TriggerSelectionSounds();
        TriggerSelectionEffects();
    }
}
