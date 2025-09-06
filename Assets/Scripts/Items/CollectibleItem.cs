using UnityEngine;
using VContainer;
public class CollectibleItem : MonoBehaviour
{
    /// <summary>
    /// コレクター要素のあるアイテムの取得に関するスクリプト
    /// </summary>
    
    //アイテムのID(外部キーの役割)
    [SerializeField] private int itemID;
    
    // ItemManagerの参照
    [Inject] private ItemManager itemManager;
    // ItemDataの参照 
    [Inject] private ItemData itemData;

    // アイテム名
    private string itemName;
    // アイテムの説明文
    private string itemDescription;
    // アイテムのスプライト
    private string itemText;

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

    // アイテムを取得情報をインベントリに保存するメソッド
    public void CollectItem()
    {
        itemManager.ObtainItem(itemID);
    }

    // アイテム名を取得するメソッド
    public string GetItemName()
    {
        return itemData.GetItemNameByID(itemID);
    }
    // アイテムのテキストを表示するメソッド
    public string GetItemText()
    {
        return itemData.GetItemTextByID(itemID);
    }
    /*
    // アイテムの説明文を取得するメソッド
    public ItemDescriptions GetItemDescription()
    {
        return itemData.GetItemDescriptionsByID(itemID);
    }
    */

    // プレイヤーが触れた時の処理など
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // アイテム情報をログに出力
            Debug.Log($"取得したアイテム: {GetItemName()}");
            Debug.Log($"アイテムテキスト: {GetItemText()}");

            // アイテム取得処理をItemManagerに依頼
            CollectItem();

            // GameManager.Instance.AddItem(itemData.itemID);
            Destroy(gameObject);
        }
    }
}
