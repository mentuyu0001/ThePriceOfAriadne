using UnityEngine;
using UnityEngine.UI; // Textを使うため

public class SaveSlotUI : MonoBehaviour
{
    // セーブデータを読み込む変数が必要！！！
    private bool saveData = false;

    [Header("データがある場合に表示するUI")]
    [SerializeField] private GameObject dataGroup;

    [Header("データがない場合に表示するUI")]
    [SerializeField] private GameObject noDataGroup;

    private void Start() {
        UpdateSlotUI();
    }

    // このセーブ欄のUIを更新するための公開関数
    private void UpdateSlotUI()
    {
        // もしセーブデータが存在するなら...
        if (saveData != null)
        {
            dataGroup.SetActive(true);
            noDataGroup.SetActive(false);
        }
        // もしセーブデータが存在しない (null) なら...
        else
        {
            dataGroup.SetActive(false);
            noDataGroup.SetActive(true);
        }
    }
}