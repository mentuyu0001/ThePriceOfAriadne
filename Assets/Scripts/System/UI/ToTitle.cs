using UnityEngine;

public class ToTitle : Button
{
    [SerializeField] private ToTitleConfirmation confirmation;
    public override void OnClick()
    {
        base.OnClick(); // 決定音を鳴らす
        confirmation.ShowConfirmationDialog();
    }
}
