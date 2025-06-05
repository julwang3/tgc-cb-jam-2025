using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;
    public bool IsPaused;

    [SerializeField] private int mainMenuBuildIndex = 0;

    [SerializeField] private Button resumeButtom;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;

    private InputAction pauseAction;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        pauseAction = InputSystem.actions.FindAction("Pause");
        pauseAction.performed += _ => OnPause();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            Debug.LogError("Pause menu is missing a canvas group.");
        }

        OnPause(false);
    }

    private void OnPause()
    {
        OnPause(!IsPaused);
    }

    private void OnPause(bool pause)
    {
        if (pause)
        {
            resumeButtom.onClick.AddListener(OnPause);
            menuButton.onClick.AddListener(OnMenuPressed);
            quitButton.onClick.AddListener(OnQuitPressed);

            canvasGroup.alpha = 1;
            canvasGroup.interactable = true;
            Time.timeScale = 0f;
        }
        else
        {
            resumeButtom.onClick.RemoveListener(OnPause);
            menuButton.onClick.RemoveListener(OnMenuPressed);
            quitButton.onClick.RemoveListener(OnQuitPressed);

            canvasGroup.alpha = 0;
            canvasGroup.interactable = false;
            Time.timeScale = 1f;
        }
        IsPaused = pause;
    }

    private void OnMenuPressed()
    {
        SceneManager.LoadScene(mainMenuBuildIndex);
    }

    private void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
