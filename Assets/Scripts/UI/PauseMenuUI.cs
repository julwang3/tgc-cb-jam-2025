using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.InputSystem.InputAction;

public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance;
    public bool IsPaused;

    [SerializeField] private string mainMenuScene = "MainMenu";

    [SerializeField] private Button resumeButtom;
    [SerializeField] private Button menuButton;
    [SerializeField] private Button quitButton;

    [SerializeField] AK.Wwise.Event PauseWwiseEvent;
    [SerializeField] AK.Wwise.Event UnpauseWwiseEvent;

    private InputAction pauseAction;
    private GameObject pauseMenu;

    private void Awake()
    {
        Instance = this;

        pauseAction = InputSystem.actions.FindAction("Pause");
        pauseAction.performed += OnPause;

        pauseMenu = gameObject.transform.GetChild(0).gameObject;

        Time.timeScale = 1f;
        IsPaused = false;
    }

    private void Start()
    {
        AudioManager.Instance.PostEventPersist(UnpauseWwiseEvent);
    }

    private void OnDestroy()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPause;
        }
    }

    public void OnPause()
    {
        OnPause(!IsPaused);
    }

    public void OnPause(CallbackContext ctx)
    {
        OnPause(!IsPaused);
    }

    private void OnPause(bool pause)
    {
        if (LevelManager.Instance && LevelManager.Instance.IsLoading) { return; }

        if (pause)
        {
            Time.timeScale = 0f;
            AudioManager.Instance.PostEventPersist(PauseWwiseEvent);
            pauseMenu.SetActive(true);
            EventSystem.current.SetSelectedGameObject(resumeButtom.gameObject);
        }
        else
        {
            Time.timeScale = 1f;
            AudioManager.Instance.PostEventPersist(UnpauseWwiseEvent);
            pauseMenu.SetActive(false);
        }
        IsPaused = pause;
    }

    public void OnMenuPressed()
    {
        if (LevelManager.Instance.IsLoading) { return; }
        AudioManager.Instance.StopLevel();
        LevelManager.Instance.LoadLevel(mainMenuScene);
    }

    public void OnQuitPressed()
    {
        if (LevelManager.Instance.IsLoading) { return; }
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
