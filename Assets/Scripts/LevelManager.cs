using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public bool IsLoading = false;

    [SerializeField] GameObject FadeToBlackObj;
    [SerializeField] Animator FadeToBlackAnim;
    [SerializeField] AK.Wwise.Event TransitionEvent;
    [SerializeField] AK.Wwise.State NextLevelState;
    [SerializeField] AK.Wwise.State MenuState;
    [SerializeField] AK.Wwise.State CreditsState;

    [HideInInspector] public bool HasSpawnPos = false;
    [HideInInspector] public Vector2 SpawnPos;

    private void Awake()
    {
        if (Instance != null)
        {
            Instance.NextLevelState = NextLevelState;
            Destroy(gameObject);
            return;
            
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(FadeFromBlack());
    }

    public void LoadLevel(string sceneName)
    {
        // Load the scene with the given name
        StartCoroutine(Transition(sceneName));
    }

    public void LoadLevel(string sceneName, Vector2 inSpawnPos)
    {
        HasSpawnPos = true;
        SpawnPos = inSpawnPos;

        // Load the scene with the given name
        StartCoroutine(Transition(sceneName));
    }

    public IEnumerator FadeToBlack()
    {
        IsLoading = true;
        FadeToBlackObj.SetActive(true);
        FadeToBlackAnim.SetTrigger("FadeToBlack");
        yield return new WaitForSecondsRealtime(2.0f);
    }

    public IEnumerator FadeFromBlack()
    {
        FadeToBlackObj.SetActive(true);
        FadeToBlackAnim.SetTrigger("FadeFromBlack");
        yield return new WaitForSecondsRealtime(2.0f);
        FadeToBlackObj.SetActive(false);
        IsLoading = false;
    }

    IEnumerator Transition(string sceneName)
    {
        AudioManager.Instance.PostEventPersist(TransitionEvent);
        if (sceneName == "MainMenu" && MenuState != null && MenuState.IsValid())
        {
            MenuState.SetValue();
        }
        else if (sceneName == "Credits" && CreditsState != null && CreditsState.IsValid())
        {
            CreditsState.SetValue();
        }
        else if (NextLevelState != null && NextLevelState.IsValid())
        {
            NextLevelState.SetValue();
        }

        if (LevelAudio.Instance && !LevelAudio.Instance.gameObject.IsDestroyed())
        {
            LevelAudio.Instance.OnLevelUnload();
        }
        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(sceneName);

        yield return StartCoroutine(FadeFromBlack());
    }
}
