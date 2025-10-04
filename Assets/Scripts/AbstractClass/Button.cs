using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// UIボタンの抽象クラス
/// </summary>
public abstract class Button : MonoBehaviour, ISelectHandler, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// UIが選択されるたびに「カチカチ」と音を流したりする際の親オブジェクト
    /// </summary>

    // カーソルが入ってるか出ているか判定する変数
    private bool isPointer = false;

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
        if (isPointer)
        {
            return;
        }

        TriggerSelectionSounds();
        TriggerSelectionEffects();
        Debug.Log("キーボード");
    }

    // マウスで選択された場合
    public void OnPointerEnter(PointerEventData eventData)
    {
        // もしこのオブジェクトが既に選択されているなら、何もしない
        if (EventSystem.current.currentSelectedGameObject == this.gameObject)
        {
            return;
        }
        isPointer = true;
        TriggerSelectionSounds();
        TriggerSelectionEffects();
        Debug.Log("マウス");
    }

    // マウスカーソルがこのUI要素の範囲から「出た」瞬間に呼ばれる
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointer = false;
    }
}
