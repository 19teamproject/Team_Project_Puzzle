using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class InGameUI : MonoBehaviour
{
    [Header("Button")]
    [SerializeField] private GameObject ingameUI;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private bool isEnabled = false;

    private void Start()
    {
        ingameUI.SetActive(isEnabled);
        ButtonSetting();
    }

    public void OnInGameUI(InputAction.CallbackContext context)
    {
        isEnabled = !isEnabled;
        ingameUI.SetActive(isEnabled);
        Cursor.lockState = isEnabled ? CursorLockMode.None : CursorLockMode.Locked;
        CharacterManager.Instance.Player.Controller.cursorInputForLook = !isEnabled;
        CharacterManager.Instance.Player.Controller.look = isEnabled ? Vector2.zero : CharacterManager.Instance.Player.Controller.look;
        Time.timeScale = isEnabled ? 0f : 1f;
    }

    private void ButtonSetting()
    {
        restartButton.onClick.AddListener(RestarButton);
        exitButton.onClick.AddListener(ExitButton);
    }

    private void RestarButton()
    {
        SceneLoader.Instance.LoadScene(SceneManager.GetActiveScene().name);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void ExitButton()
    {
        SceneLoader.Instance.LoadScene("PJW_Title");
        Cursor.lockState = CursorLockMode.None;
    }
}
