using UnityEngine;

public class InisialItemText : MonoBehaviour
{
    [SerializeField] private StageItemSlot itemSlot;

    // このUIが表示された時に呼ばれる関数
    public void OnEnable()
    {
        itemSlot.ShowFlervorText();
    }
}
