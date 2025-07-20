using UnityEngine;

public class PlayerFallAction : FallingEntity
{
    [SerializeField] private GameOverManager gameOverManager;

    protected override void FallAction()
    {
        gameOverManager.GameOver();
        Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.gameObject.tag == "FallObject") {
            FallAction();
        }
    }
}