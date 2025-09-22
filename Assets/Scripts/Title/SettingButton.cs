using UnityEngine;

public class SettingButton : Button
{
    public GameObject titlePanel;   // Title パネルを参照するための変数
    public GameObject settingPanel; // Setting パネルを参照するための変数
    protected override void OnClick()
    {
        titlePanel.SetActive(false);
        settingPanel.SetActive(true);
        Debug.Log("Setting パネルを表示し、Title パネルを非表示にしました");
    }
}
