using UnityEngine;

public class FallCheck : MonoBehaviour
{
    [SerializeField] private GameOverManager gameOverManager;

    // 落下判定を行うメソッド
    void OnTriggerEnter2D(Collider2D collider) {
        gameOverManager.GameOver();
    }
}
