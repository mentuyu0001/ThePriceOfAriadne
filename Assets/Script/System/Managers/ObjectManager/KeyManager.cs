using System.Collections.Generic;

/// <summary>
/// どの鍵を持っているか管理するスクリプト
/// </summary>

public class KeyManager
{
    // 持っている鍵のIDを格納する配列
    static private List<int> keys;

    // 静的コンストラクタで初期化&インスタンス作成
    static KeyManager()
    {
        keys = new List<int>();
    }

    // 鍵を取得
    static public void GetKey(int keyID)
    {
        keys.Add(keyID);
    }

    // 鍵を持っていれば使用
    static public bool TryToUseKey(int keyID)
    {
        if (keys.Contains(keyID))
        {
            keys.Remove(keyID);
            return true; // 鍵を使用できた
        }
        return false; // 鍵がなかった
    }
}
