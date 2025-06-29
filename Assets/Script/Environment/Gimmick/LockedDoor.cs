using UnityEngine;

/// <summary>
/// 鍵のかかったドアのスクリプト
/// </summary>

public class LockedDoor : MonoBehaviour
{
    // ドアのID，これと同じIDの鍵ならば開けられる
    [SerializeField] int doorID;

    // プレイヤーの参照
    [SerializeField] private GameObject player;

    // プレイヤーステータスを参照
    [SerializeField] private PlayerStatus playerStatus;

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
            Debug.LogError("LockedDoor: Player GameObject is not assigned.");
        }
    }

    // ドアを開く関数
    void OpenDoor()
    {
        // ドアを非表示にする
        this.gameObject.SetActive(false);
    }

    // ドアを開けるのを試みる関数
    public void TryToOpen()
    {
        // 対応する鍵を持っていれば開く
        if (KeyManager.TryToUseKey(doorID) || playerStatus.CanUnlock)
        {
            OpenDoor();
        }
        else
        {
            Debug.Log("鍵が必要です。");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // プレイヤーが扉に触れたら開けるのを試みる
        if (collision.gameObject.name == playerName)
        {
            TryToOpen();
        }
    }
}
