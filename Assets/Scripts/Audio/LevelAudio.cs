using UnityEngine;

public class LevelAudio : MonoBehaviour
{
    public static LevelAudio Instance;
    [SerializeField] AK.Wwise.State LevelState;
    [SerializeField] AK.Wwise.Event Ambience;

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LevelState.SetValue();
        PostEventLocal(Ambience);
        AudioManager.Instance.StartLevel();
    }

    public void OnLevelUnload()
    {
        Ambience.Stop(gameObject, 1);
    }

    // Does not persist
    public void PostEventLocal(AK.Wwise.Event evt)
    {
        if (evt != null && evt.IsValid())
        {
            evt.Post(gameObject);
        } 
    }

    // Only use when calling through a Unity.Event in the inspector
    public void PostEventLocal(string evt)
    {
        AkSoundEngine.PostEvent(evt, gameObject);
    }
}
