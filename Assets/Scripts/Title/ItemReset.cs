using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemReset : Button
{
    /// <summary>
    /// コレクトアイテムの進捗をリセットする
    /// </summary>

    // 確認画面の取得
    [SerializeField] private ItemResetConfirmation resetConfirm;
    [SerializeField] private ItemSlot[] itemSlots;  // ItemSlotの配列を追加

    void Start()
    {
        // ItemSlotが設定されていない場合は自動で探す
        if (itemSlots == null || itemSlots.Length == 0)
        {
            itemSlots = FindObjectsOfType<ItemSlot>();
            if (itemSlots.Length == 0)
            {
                UnityEngine.Debug.LogWarning("ItemSlotが見つかりません");
            }
        }
    }

    public override void OnClick()
    {
        base.OnClick(); // 決定音を鳴らす
        UnityEngine.Debug.Log("アイテムリセットの確認画面を表示します。");
        resetConfirm.ShowConfirmationDialog();
        
        // 全てのItemSlotのアイコンを更新
        foreach (var slot in itemSlots)
        {
            if (slot != null)
            {
                slot.UpdateIcon();
                UnityEngine.Debug.Log($"ItemSlot {slot.name} のアイコン{slot.iconImage.enabled}");
            }
        }
    }
}
