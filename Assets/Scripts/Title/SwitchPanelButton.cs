using UnityEngine;

public class SwitchPanelButton : Button
{
    public GameObject currentPanel; // 今いるパネルを参照するための変数
    public GameObject returnPanel;   // 遷移するパネルを参照するための変数
    protected override void OnClick()
    {
        currentPanel.SetActive(false);
        returnPanel.SetActive(true);
    }
}