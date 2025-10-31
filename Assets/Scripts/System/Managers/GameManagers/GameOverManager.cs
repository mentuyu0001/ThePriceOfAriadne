using Unity.VisualScripting;
using UnityEngine;
using VContainer;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverEffect;
    [SerializeField] private GameObject player;
    [SerializeField] private GameSceneManager gameSceneManager;

    public void GameOver()
    {
        Debug.Log("Game Over");
        if (gameOverEffect != null && player != null)
        {
            // X軸を-90度回転
            Quaternion effectRotation = Quaternion.Euler(-90, 0, 0);
            Instantiate(gameOverEffect, player.transform.position, effectRotation);
            Debug.Log("Game Over Effect Triggered");
        }

        // ゲームオーバー時のサウンドを再生（Singletonのみでアクセス）
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySE(0); // 0はゲームオーバーサウンドのインデックス
        }
        Destroy(player);
        gameSceneManager.LoadStage2();
    }
}
