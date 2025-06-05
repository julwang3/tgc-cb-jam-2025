using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        startButton.onClick.AddListener(OnStartPressed);
        creditsButton.onClick.AddListener(OnCreditsPressed);
        quitButton.onClick.AddListener(OnQuitPressed);
    }

    private void OnDisable()
    {
        startButton.onClick.RemoveListener(OnStartPressed);
        creditsButton.onClick.RemoveListener(OnCreditsPressed);
        quitButton.onClick.RemoveListener(OnQuitPressed);
    }

    private void OnStartPressed()
    {
        Debug.Log("Start"); // TODO
    }

    private void OnCreditsPressed()
    {
        Debug.Log("Credits"); // TODO
    }

    private void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
