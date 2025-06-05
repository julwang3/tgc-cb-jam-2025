using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    public bool IsLoading = false;

    [SerializeField] GameObject FadeToBlackObj;
    [SerializeField] Animator FadeToBlackAnim;

    [HideInInspector] public bool HasSpawnPos = false;
    [HideInInspector] public Vector2 SpawnPos;

    private void Awake()
    {
        if (Instance != null)
        {
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

    IEnumerator FadeToBlack()
    {
        IsLoading = true;
        FadeToBlackObj.SetActive(true);
        FadeToBlackAnim.SetTrigger("FadeToBlack");
        yield return new WaitForSecondsRealtime(1.0f);
    }

    IEnumerator FadeFromBlack()
    {
        FadeToBlackObj.SetActive(true);
        FadeToBlackAnim.SetTrigger("FadeFromBlack");
        yield return new WaitForSecondsRealtime(1.0f);
        FadeToBlackObj.SetActive(false);
        IsLoading = false;
    }

    IEnumerator Transition(string sceneName)
    {
        yield return StartCoroutine(FadeToBlack());

        SceneManager.LoadScene(sceneName);

        yield return StartCoroutine(FadeFromBlack());
    }
}
