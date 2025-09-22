using UnityEngine;

public class PlayButton : Button
{
    public GameObject titlePanel;   // Title パネルを参照するための変数
    public GameObject playMenuPanel; // Setting パネルを参照するための変数
    protected override void OnClick()
    {
        titlePanel.SetActive(false);
        playMenuPanel.SetActive(true);
        Debug.Log("PlayMenu パネルを表示し、Title パネルを非表示にしました");
    }
}
