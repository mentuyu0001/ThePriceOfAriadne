using UnityEngine;
using VContainer;
using UnityEngine.SceneManagement;
public class PlayerFallAction : FallingEntity
{
    [Inject] private GameOverManager gameOverManager;
    
    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "TitleScene") return;

        // デバッグ用：依存性注入の確認
        if (gameOverManager != null)
        {
            Debug.Log($"✅ PlayerFallAction: GameOverManager注入成功 - {gameOverManager.name}");
        }
        else
        {
            gameOverManager = GameObject.Find("GameOverManager").GetComponent<GameOverManager>();
            if (gameOverManager != null) Debug.LogError("❌ PlayerFallAction: GameOverManager注入失敗 - nullです");
        }
    }
    protected override void FallAction()
    {
        gameOverManager.GameOver();
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "FallObject") {
            FallAction();
        }
    }
}