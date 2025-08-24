using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    /// <summary>
    /// コレクター要素のあるアイテムの取得に関するスクリプト
    /// </summary>
    
    /*

    // このアイテムがどの「設計図」を持つかを設定する
    [SerializeField] private ItemData itemData;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        // 自分のSpriteRendererコンポーネントを取得
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // ItemDataに基づいて自分の見た目を設定する
        ApplyItemData();
    }

    // 設計図の情報を自分に反映させるメソッド
    public void ApplyItemData()
    {
        if (itemData == null)
        {
            Debug.LogError("ItemDataが設定されていません！");
            return;
        }

        // 設計図からスプライトを取得し、自分の見た目に設定
        spriteRenderer.sprite = itemData.itemSprite;
        
        // 名前も設計図に合わせて変更しておく（デバッグに便利）
        this.gameObject.name = itemData.itemName;
    }
    */

    // プレイヤーが触れた時の処理など
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Debug.Log(itemData.itemName + " を手に入れた！");
            // ここで「取得済みリスト」に追加するなどの処理
            // GameManager.Instance.AddItem(itemData.itemID);
            Destroy(gameObject);
        }
    }
}
