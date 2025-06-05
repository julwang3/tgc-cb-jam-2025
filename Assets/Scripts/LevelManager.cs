using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [HideInInspector] public bool hasSpawnPos = false;
    [HideInInspector] public Vector2 spawnPos;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
            
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void LoadLevel(string sceneName)
    {
        // Load the scene with the given name
        SceneManager.LoadScene(sceneName);
    }

    public void LoadLevel(string sceneName, Vector2 inSpawnPos)
    {
        hasSpawnPos = true;
        spawnPos = inSpawnPos;

        // Load the scene with the given name
        SceneManager.LoadScene(sceneName);
    }
}
