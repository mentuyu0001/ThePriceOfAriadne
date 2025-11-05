using UnityEngine;

public class StageNumber : MonoBehaviour
{
    // シングルトンインスタンス
    public static StageNumber Instance { get; private set; }

    // 現在のステージ番号
    [SerializeField] private int currentStage = 1;

    // ステージ番号を取得
    public int GetCurrentStage()
    {
        return currentStage;
    }

    // ステージ番号を設定
    public void SetCurrentStage(int stage)
    {
        currentStage = stage;
    }
}
