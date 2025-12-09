using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TitleEscAction : MonoBehaviour
{
    private bool isLoading = false;

    // Escキーに関する処理
    [Header("UIをEscキーで消せるようにするために必要なGameObjectの注入")]
    [SerializeField] private GameObject playMenu;
    [SerializeField] private GameObject itemInventory;
    [SerializeField] private GameObject itemResetConfirmation;
    [SerializeField] private GameObject saves;
    [SerializeField] private GameObject saveConfirmation;
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject itemDescroptionButtons;

    [Header("Escキーを押した時のReturnボタンのスクリプトの注入")]
    [SerializeField] private GameObject toTitleReturn;
    [SerializeField] private GameObject itemReturn;
    [SerializeField] private GameObject itemResetReturn;
    [SerializeField] private GameObject savesReturn;
    [SerializeField] private GameObject saveConfirmationReturn;
    [SerializeField] private GameObject settingsReturn;
    [SerializeField] private GameObject itemDescriptionReturn;

    private UnityEngine.UI.Button targetButton;

    public void OnEsc(InputAction.CallbackContext context)
    {
        // ボタンが押された瞬間だけ実行する
        if (context.performed)
        {
            if (isLoading) return;

            if (playMenu.activeSelf)
            {
                targetButton = toTitleReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
            else if (itemInventory.activeSelf)
            {
                if (itemDescroptionButtons.activeSelf)
                {
                    targetButton = itemDescriptionReturn.GetComponent<UnityEngine.UI.Button>();
                    targetButton.onClick.Invoke();
                    return;
                }
                else if (itemResetConfirmation.activeSelf)
                {
                    targetButton = itemResetReturn.GetComponent<UnityEngine.UI.Button>();
                    targetButton.onClick.Invoke();
                    return;
                }

                targetButton = itemReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
            else if (saves.activeSelf)
            {
                if (saveConfirmation.activeSelf)
                {
                    targetButton = saveConfirmationReturn.GetComponent<UnityEngine.UI.Button>();
                    targetButton.onClick.Invoke();
                    return;
                }

                targetButton = savesReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
            else if (settings.activeSelf)
            {
                targetButton = settingsReturn.GetComponent<UnityEngine.UI.Button>();
                targetButton.onClick.Invoke();
                return;
            }
        }
    }

    public void ActiveIsLoading()
    {
        isLoading = true;
    }
}
