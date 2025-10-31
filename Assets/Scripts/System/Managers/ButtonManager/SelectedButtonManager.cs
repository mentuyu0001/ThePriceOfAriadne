using UnityEngine;
using UnityEngine.EventSystems;

public class SelectedButtonManager : MonoBehaviour
{
    private GameObject lastSelectedButton;

    public void SetLastSelectedButton(GameObject button)
    {
        lastSelectedButton = button;
    }

    public void ReturnSelectedButton()
    {
        Debug.Log("ReturnSelectedButton lastSelectedButton: " + lastSelectedButton);
        //  記憶しておいたボタンにフォーカスを戻す
        EventSystem.current.SetSelectedGameObject(lastSelectedButton);
    }
}   
