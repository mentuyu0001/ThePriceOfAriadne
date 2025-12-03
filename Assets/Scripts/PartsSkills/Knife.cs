using UnityEngine;

public class Knife : MonoBehaviour
{
    /// <summary>
    /// ナイフのが当たったらオブジェクトの消去を行うスクリプト
    /// </summary>

    // 何かに当たった時の処理
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other);

        if (other.CompareTag("PlayerDetection") || other.CompareTag("StopButton") || other.CompareTag("BurningFire") || other.CompareTag("WaterTank"))
        {
        return; // プレイヤー検知用の当たり判定と衝突した場合は消滅を回避
        }

        Destroy(gameObject); // 自分自身（ナイフ）を消す
    }
}
