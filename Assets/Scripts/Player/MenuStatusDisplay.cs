using UnityEngine;
using VContainer;
using Parts.Types;
using UnityEngine.UI;

public class MenuStatusDisplay : MonoBehaviour
{
    // プレイヤーのパーツ情報を保持するシングルトン
    [SerializeField] private PlayerParts playerParts;
    [Header("表示する画像（子オブジェクトのものをアタッチする）")]
    [SerializeField] private Image ImageLeftArm;
    [SerializeField] private Image ImageRightArm;
    [SerializeField] private Image ImageLeftLeg;
    [SerializeField] private Image ImageRightLeg;

    // 各パーツの種類を保存する変数
    private PartsChara LeftArm;
    private PartsChara RightArm;
    private PartsChara LeftLeg;
    private PartsChara RightLeg;


    void Start()
    {
        // コンポーネントの確認
        if (playerParts == null)
        {
            Debug.LogError("MenuStatusDisplay: playerParts connot found.");
        }
        if (ImageLeftArm == null)
        {
            Debug.LogError("MenuStatusDisplay: ImageLeftArm connot found.");
        }
        if (ImageRightArm == null)
        {
            Debug.LogError("MenuStatusDisplay: ImageRightArm connot found.");
        }
        if (ImageLeftLeg == null)
        {
            Debug.LogError("MenuStatusDisplay: ImageLeftLeg connot found.");
        }
        if (ImageRightLeg == null)
        {
            Debug.LogError("MenuStatusDisplay: ImageRightLeg connot found.");
        }

    }

    // オブジェクトがアクティブになったら実行される関数
    void OnEnable()
    {
        if (playerParts != null)
        {
            // プレイヤーのパーツを取得
            LeftArm = playerParts.LeftArm;
            RightArm = playerParts.RightArm;
            LeftLeg = playerParts.LeftLeg;
            RightLeg = playerParts.RightLeg;
            DisplayStatus();
        }
    }

    // プレイヤーのパーツを表示する関数
    void DisplayStatus()
    {
        // Debug.Log(playerParts.LeftLeg);
        // 左腕の画像を変更
        if (ImageLeftArm != null)
        {
            switch (playerParts.LeftArm)
            {
                // 主人公パーツ
                case PartsChara.Normal:
                    ImageLeftArm.color = Color.green;
                    break;
                // 泥棒パーツ
                case PartsChara.Theif:
                    ImageLeftArm.color = new Color(0.8f, 0.0f, 1.0f);
                    break;
                // 軍人パーツ
                case PartsChara.Muscle:
                    ImageLeftArm.color = Color.red;
                    break;
                // アサシンパーツ
                case PartsChara.Assassin:
                    ImageLeftArm.color = Color.yellow;
                    break;
                // 消防士パーツ
                case PartsChara.Fire:
                    ImageLeftArm.color = Color.cyan;
                    break;
                case PartsChara.None:
                    ImageLeftArm.color = Color.white;
                    Debug.Log("MenuStatusDisplay: LeftArm is None.");
                    break;

            }
        }

        // 左足の画像を変更
        if (ImageLeftLeg != null)
        {
            switch (playerParts.LeftLeg)
            {
                // 主人公パーツ
                case PartsChara.Normal:
                    ImageLeftLeg.color = Color.green;
                    break;
                // 泥棒パーツ
                case PartsChara.Theif:
                    ImageLeftLeg.color = new Color(0.8f, 0.0f, 1.0f);
                    break;
                // 軍人パーツ
                case PartsChara.Muscle:
                    ImageLeftLeg.color = Color.red;
                    break;
                // アサシンパーツ
                case PartsChara.Assassin:
                    ImageLeftLeg.color = Color.yellow;
                    break;
                // 消防士パーツ
                case PartsChara.Fire:
                    ImageLeftLeg.color = Color.cyan;
                    break;
                case PartsChara.None:
                    ImageLeftLeg.color = Color.white;
                    Debug.Log("MenuStatusDisplay: LeftLeg is None.");
                    break;

            }
        }

        // 右腕の画像を変更
        if (ImageRightArm != null)
        {
            switch (playerParts.RightArm)
            {
                // 主人公パーツ
                case PartsChara.Normal:
                    ImageRightArm.color = Color.green;
                    break;
                // 泥棒パーツ
                case PartsChara.Theif:
                    ImageRightArm.color = new Color(0.8f, 0.0f, 1.0f);
                    break;
                // 軍人パーツ
                case PartsChara.Muscle:
                    ImageRightArm.color = Color.red;
                    break;
                // アサシンパーツ
                case PartsChara.Assassin:
                    ImageRightArm.color = Color.yellow;
                    break;
                // 消防士パーツ
                case PartsChara.Fire:
                    ImageRightArm.color = Color.cyan;
                    break;
                case PartsChara.None:
                    ImageRightArm.color = Color.white;
                    Debug.Log("MenuStatusDisplay: RightArm is None.");
                    break;

            }
        }
        
        // 右足の画像を変更
        if (ImageRightLeg !=null)
        {
            switch (playerParts.RightLeg)
            {
                // 主人公パーツ
                case PartsChara.Normal:
                    ImageRightLeg.color = Color.green;
                    break;
                // 泥棒パーツ
                case PartsChara.Theif:
                    ImageRightLeg.color = new Color(0.8f, 0.0f, 1.0f);
                    break;
                // 軍人パーツ
                case PartsChara.Muscle:
                    ImageRightLeg.color = Color.red;
                    break;
                // アサシンパーツ
                case PartsChara.Assassin:
                    ImageRightLeg.color = Color.yellow;
                    break;
                // 消防士パーツ
                case PartsChara.Fire:
                    ImageRightLeg.color = Color.cyan;
                    break;
                case PartsChara.None:
                    ImageRightLeg.color = Color.white;
                    Debug.Log("MenuStatusDisplay: RightLeg is None.");
                    break;

            }
        }
    }

}
