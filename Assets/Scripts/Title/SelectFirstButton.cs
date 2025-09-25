using UnityEngine;
using UnityEngine.EventSystems;

public class SelectFirstButton : MonoBehaviour
{
    // 最初に選択状態にしたいボタンをセット
    [SerializeField] private GameObject firstSelectedButton;

    // このUIが表示されたときに呼ばれる関数
    void OnEnable()
    {
        // EventSystemに最初に選択するオブジェクトをEventSystemに教える
        EventSystem.current.SetSelectedGameObject(firstSelectedButton);
    }
}
