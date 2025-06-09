using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public void LoadLevel(string name)
    {
        LevelManager.Instance.LoadLevel(name);
    }
}
