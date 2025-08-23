using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] private float throwForce = 10f; // ナイフの速度
    private Rigidbody2D rb;

    void Start()
    {
        rb = this.gameObject.GetComponent<Rigidbody2D>();
        // ナイフの飛ぶ方向（右向き）に力を加える
        rb.AddForce(Vector2.right * throwForce, ForceMode2D.Impulse);
    }

    // 何かに当たった時の処理
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("PlayerDetection"))
        {
        return; // プレイヤー検知用の当たり判定と衝突した場合は消滅を回避
        }

        Destroy(gameObject); // 自分自身（ナイフ）を消す
    }
}
