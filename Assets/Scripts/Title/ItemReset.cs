using UnityEngine;

public class ItemReset : Button
{
    /// <summary>
    /// コレクトアイテムの進捗をリセットする
    /// </summary>

    public override void OnClick()
    {
        Debug.Log("アイテムをリセットした"); // 本当は確認画面をここに挟む
    }
}
