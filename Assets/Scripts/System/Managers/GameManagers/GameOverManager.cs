using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverEffect;
    [SerializeField] private GameObject player;
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
    }
}
