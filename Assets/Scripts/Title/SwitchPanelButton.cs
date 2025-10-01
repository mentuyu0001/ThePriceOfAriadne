using UnityEngine;

public class SwitchPanelButton : MonoBehaviour
{
    [SerializeField] private GameObject currentPanel; // 今いるパネルを参照するための変数
    [SerializeField] private GameObject returnPanel;   // 遷移するパネルを参照するための変数
    public void ChangePanel()
    {
        currentPanel.SetActive(false);
        returnPanel.SetActive(true);
    }
}