namespace Parts.Types
{
    /// <summary>
    /// プレイヤーが装備する部品の型を保存する
    /// </summary>

    // パーツの場所を保存する型
    public enum PartsSlot {
        None,
        LeftArm,
        RightArm,
        LeftLeg,
        RightLeg,
    }

    // どのキャラかを保存する型
    public enum PartsChara {
        None,
        Normal,
        Thief,
        Muscle,
        Assassin,
        Fire,
    }
}
