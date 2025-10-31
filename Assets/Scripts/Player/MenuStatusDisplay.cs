using UnityEngine;
using Parts.Types;
using VContainer;
using TMPro;

/// <summary>
/// メニュー画面でプレイヤーのパーツ状態を表示するスクリプト
/// </summary>

public class MenuStatusDisplay : MonoBehaviour
{
    // パーツデータの参照
    [SerializeField] private PartsData partsData;

    // プレイヤーのパーツ情報の参照
    [Inject] private PlayerParts playerParts;

    // パーツの情報を表示するテキストの参照
    [Header("パーツの名前を表示するテキスト")]
    [SerializeField] private TextMeshProUGUI leftArmName;
    [SerializeField] private TextMeshProUGUI rightArmName;
    [SerializeField] private TextMeshProUGUI leftLegName;
    [SerializeField] private TextMeshProUGUI rightLegName;

    [Header("パーツの説明を表示するテキスト")]
    [SerializeField] private TextMeshProUGUI leftArmDescription;
    [SerializeField] private TextMeshProUGUI rightArmDescription;
    [SerializeField] private TextMeshProUGUI leftLegDescription;
    [SerializeField] private TextMeshProUGUI rightLegDescription;

    void Start()
    {
        // コンポーネントの確認
        if (partsData == null)
        {
            Debug.LogError("MenuStatusDisplay: PartsData is not assigned.");
        }
        if (leftArmName == null || rightArmName == null || leftLegName == null || rightLegName == null)
        {
            Debug.LogError("MenuStatusDisplay: One or more adjective GameObjects are not assigned.");
        }
        if (leftArmDescription == null || rightArmDescription == null || leftLegDescription == null || rightLegDescription == null)
        {
            Debug.LogError("MenuStatusDisplay: One or more description GameObjects are not assigned.");
        }
    }

    // プレイヤーのパーツを表示する関数
    void DisplayStatus()
    {
        // 左腕の形容詞と説明を表示
        PartsInfo leftArmInfo = partsData.GetPartsInfoByPartsChara(playerParts.LeftArm);
        leftArmName.text = leftArmInfo.adjective + "左腕";
        leftArmDescription.text = leftArmInfo.descriptionArm;

        // 右腕の形容詞と説明を表示
        PartsInfo rightArmInfo = partsData.GetPartsInfoByPartsChara(playerParts.RightArm);
        rightArmName.text = rightArmInfo.adjective + "右腕";
        rightArmDescription.text = rightArmInfo.descriptionArm;

        // 左脚の形容詞と説明を表示
        PartsInfo leftLegInfo = partsData.GetPartsInfoByPartsChara(playerParts.LeftLeg);
        leftLegName.text = leftLegInfo.adjective + "左足";
        leftLegDescription.text = leftLegInfo.descriptionLeg;   
    
        // 右脚の形容詞と説明を表示
        PartsInfo rightLegInfo = partsData.GetPartsInfoByPartsChara(playerParts.RightLeg);
        rightLegName.text = rightLegInfo.adjective + "右足";
        rightLegDescription.text = rightLegInfo.descriptionLeg;
    }

    // VContainerの注入完了時に呼ばれるメソッド
    [Inject]
    public void Construct(PlayerParts injectedPlayerParts)
    {
        this.playerParts = injectedPlayerParts;
        // 注入完了後に表示を更新
        DisplayStatus();

        Debug.Log("MenuStatusDisplay: PlayerParts injected successfully.");
    }

    // オブジェクトがアクティブになったら実行される関数
    void OnEnable()
    {   
        if (playerParts == null)
        {
            // 注入前にOnEnableが呼ばれた場合はここに入るが、Constructで表示されるのでログだけにする
            Debug.LogWarning("MenuStatusDisplay: PlayerParts is not yet injected. Waiting for injection.");
        }
        else
        {
            DisplayStatus();
        }
    }

}
