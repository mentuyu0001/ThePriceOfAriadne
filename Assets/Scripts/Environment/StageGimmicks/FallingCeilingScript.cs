using UnityEngine;
using Cysharp.Threading.Tasks;
using VContainer;

public class FallingCeilingScript : MonoBehaviour
{
    // プレイヤーの参照
    [Inject] 
    private GameObject player;
    // Groundの参照
    [Inject] 
    private GameObject ground;
    // GameOverManagerの参照
    [Inject] 
    private GameOverManager gameOverManager;
    
    // 天井が落ちる速さ
    [SerializeField] private float fallingSpeed = 1f;
    // 子オブジェクトの参照
    private GameObject childObject;
    private Rigidbody2D fallingCeilingRigidbody;
    // プレイヤーの名前
    private string playerName;
    // グラウンドの名前
    private string groundName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // プレイヤーの名前を取得
        if (player != null)
        {
            playerName = player.name;
            Debug.Log($"VContainerでプレイヤーが正常に取得されました: {player.name}");
        }
        else
        {
            Debug.LogError("VContainerでプレイヤーの注入に失敗しました");
            return;
        }

        // グラウンドの名前を取得
        if (ground != null)
        {
            groundName = ground.name;
            Debug.Log($"VContainerでGroundが正常に取得されました: {ground.name}");
        }
        else
        {
            Debug.LogError("VContainerでGroundの注入に失敗しました");
            return;
        }

        // GameOverManagerの確認
        if (gameOverManager != null)
        {
            Debug.Log("VContainerでGameOverManagerが正常に取得されました");
        }
        else
        {
            Debug.LogError("VContainerでGameOverManagerの注入に失敗しました");
            return;
        }

        // 最初の子オブジェクトを取得
        if (transform.childCount > 0)
        {
            childObject = transform.GetChild(0).gameObject;
        }
        else
        {
            Debug.LogWarning("FallingCeilingScript: No child object found.");
        }

        // Rigidbody2Dコンポーネントを取得
        fallingCeilingRigidbody = GetComponent<Rigidbody2D>();
        if (fallingCeilingRigidbody == null)
        {
            Debug.LogError("FallingCeilingScript: Rigidbody2D component not found on the GameObject.");
        }
        else
        {
            fallingCeilingRigidbody.gravityScale = 0; // 初期状態では重力を無効にする
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 子オブジェクトの当たり判定内にプレイヤーが入った時
        if (childObject != null && collision.gameObject.name == playerName)
        {
            childObject.SetActive(false); // 子オブジェクトを非アクティブにする
            fallingCeilingRigidbody.gravityScale = fallingSpeed; // 重力を有効にする
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == playerName) // プレイヤーと接触した時
        {
            gameOverManager.GameOver(); // GameOverManagerのGameOverメソッドを呼び出す
        }

        if (collision.gameObject.name == groundName) // 地面と接触した時
        {
            this.gameObject.SetActive(false); // 天井オブジェクトを非アクティブにする
        }
    }
}
