using UnityEngine;
using VContainer;

/// <summary>
/// 鍵のかかったドアを開ける鍵のスクリプト
/// </summary>
public class DoorKey : MonoBehaviour
{
    // 鍵のID，これと同じIDのドアならば開けられる
    [SerializeField] int keyID;

    // プレイヤーの参照
    [Inject]
    private GameObject player;
    
    // プレイヤーの名前
    private string playerName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // プレイヤーの名前を取得
        if (player != null)
        {
            playerName = player.name;
        }
        else
        {
            Debug.LogError("VContainerでプレイヤーの注入に失敗しました");
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーが鍵に触れたかどうかを確認
        if (collision.gameObject.name == playerName)
        {
            // 鍵を取得
            KeyManager.GetKey(keyID);
            // 鍵のオブジェクトを非表示にする
            this.gameObject.SetActive(false);
        }
    }
}
