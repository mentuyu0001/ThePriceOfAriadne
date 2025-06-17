using UnityEngine;

/// <summary>
/// 装備品ごとに設定される能力値を保持するスクリプタブルオブジェクト
/// </summary>

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "ScriptableObject/Player/Status")]
public class PlayerStatusData : ScriptableObject
{
    // プレイヤーの元状態のステータスデータ
    [SerializeField] private float normalMoveSpeed = 10.0f;
    [SerializeField] private bool normalCanUnlock = false;
    [SerializeField] private float normalJumpForce = 10.0f;
    [SerializeField] private bool normalCanPushHeavyObject = false;

    // 泥棒状態のステータスデータ
    [SerializeField] private float thiefMoveSpeed = 20.0f;
    [SerializeField] private bool thiefCanUnlock = true; 
    // マッチョのステータスデータ
    [SerializeField] private float muscleJumpForce = 20.0f;
    [SerializeField] private bool muscleCanPushHeavyObject = true;

    // --- プロパティ ---
    public float NormalMoveSpeed => normalMoveSpeed;
    public bool NormalCanUnlock => normalCanUnlock;
    public float NormalJumpForce => normalJumpForce;
    public bool NormalCanPushHeavyObject => normalCanPushHeavyObject;

    public float ThiefMoveSpeed => thiefMoveSpeed;
    public bool ThiefCanUnlock => thiefCanUnlock;

    public float MuscleJumpForce => muscleJumpForce;
    public bool MuscleCanPushHeavyObject => muscleCanPushHeavyObject;
}
