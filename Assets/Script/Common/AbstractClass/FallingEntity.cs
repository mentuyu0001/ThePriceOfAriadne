using UnityEngine;

///  <summary>
/// 落下した際の処理を行うインターフェース
/// </summary>

public abstract class FallingEntity : MonoBehaviour
{
    // 共通の落下処理（中身は派生クラスに任せる）
    protected abstract void FallAction();
}
