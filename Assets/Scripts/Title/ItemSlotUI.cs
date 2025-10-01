using UnityEngine;

public class ItemSlotUI : MonoBehaviour
{
    [Header("アイテム番号")]
    [SerializeField] private int itemNum;

    private bool isHand = true; // アイテムを取得してるかどうか。実際はboolじゃなくて、ScriptablObjectだと思う

    private void Awake()
    {
        if (!isHand)
        {
            // アイテム持ってなかったときの処理
        }
    }

    public void LoadItem()
    {
        Debug.Log("アイテム番号" + itemNum + "を読み込んだ");
    }
}
