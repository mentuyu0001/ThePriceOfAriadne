using UnityEngine;

/// <summary>
/// 装備品ごとに設定される能力値を保持するスクリプタブルオブジェクト
/// </summary>

[CreateAssetMenu(fileName = "PlayerStatus", menuName = "ScriptableObject/Player/Status")]
public class PlayerStatusData : ScriptableObject
{
    // プレイヤーの元状態のステータスデータ
    [SerializeField] private float normalFriction = 3.5f; // 地面との摩擦
    [SerializeField] private float normalAirResistance = 10.0f; // 空中移動時の抵抗（実際の値と割り算して使う）
    [SerializeField] private float normalMoveSpeed = 10.0f;
    [SerializeField] private bool normalCanUnlock = false;
    [SerializeField] private float normalJumpForce = 10.0f;
    [SerializeField] private bool normalCanPushHeavyObject = false;
    [SerializeField] private bool normalCanWalkOnFire = false;
    [SerializeField] private bool normalCanShootWater = false;
    [SerializeField] private bool normalCanDoubleJump = false;
    [SerializeField] private bool normalCanThrowKnife = false;

    // 泥棒状態のステータスデータ
    [SerializeField] private float theifFriction = 5.0f;
    [SerializeField] private float theifAirResistance = 30.0f;
    [SerializeField] private float thiefMoveSpeed = 20.0f;
    [SerializeField] private bool thiefCanUnlock = true; 
    // マッチョのステータスデータ
    [SerializeField] private float muscleJumpForce = 20.0f;
    [SerializeField] private bool muscleCanPushHeavyObject = true;
    // 消防士のステータスデータ
    [SerializeField] private bool fireCanWalkOnFire = true;
    [SerializeField] private bool fireCanShootWater = true;
    // アサシンのステータスデータ
    [SerializeField] private bool assassinCanDoubleJump = true;
    [SerializeField] private bool assassinCanThrowKnife = true;

    // --- プロパティ ---
    public float NormalFriction => normalFriction;
    public float NormalAirResistance => normalAirResistance;
    public float NormalMoveSpeed => normalMoveSpeed;
    public bool NormalCanUnlock => normalCanUnlock;
    public float NormalJumpForce => normalJumpForce;
    public bool NormalCanPushHeavyObject => normalCanPushHeavyObject;
    public bool NormalCanWalkOnFire => normalCanWalkOnFire;
    public bool NormalCanShootWater => normalCanShootWater;
    public bool NormalCanDoubleJump => normalCanDoubleJump;
    public bool NormalCanThrowKnife => normalCanThrowKnife;

    public float TheifFriction => theifFriction;
    public float TheifAirResistance => theifAirResistance;
    public float ThiefMoveSpeed => thiefMoveSpeed;
    public bool ThiefCanUnlock => thiefCanUnlock;

    public float MuscleJumpForce => muscleJumpForce;
    public bool MuscleCanPushHeavyObject => muscleCanPushHeavyObject;

    public bool FireCanWalkOnFire => fireCanWalkOnFire;
    public bool FireCanShootWater => fireCanShootWater;

    public bool AssassinCanDoubleJump => assassinCanDoubleJump;
    public bool AssassinCanThrowKnife => assassinCanThrowKnife;
}
