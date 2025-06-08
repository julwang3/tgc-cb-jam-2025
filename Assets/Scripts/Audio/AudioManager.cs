using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [SerializeField] AK.Wwise.Bank[] PersistantBanks; // Persist through entire game
    [SerializeField] AK.Wwise.Event[] GameStartEvents;

    [Header("Level Persistant")]
    [SerializeField] AK.Wwise.Event LevelMusic;
    [SerializeField] AK.Wwise.Bank[] LevelPersistantBanks; // Persist through levels, but not menus
    private bool isInLevel = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            foreach (AK.Wwise.Bank bank in PersistantBanks)
            {
                if (bank != null && bank.IsValid())
                {
                    bank.Load();
                }
            }
            foreach (AK.Wwise.Event gameStartEvent in GameStartEvents)
            {
                PostEventPersist(gameStartEvent);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnApplicationQuit()
    {
        foreach (AK.Wwise.Bank bank in PersistantBanks)
        {
            if (bank != null && bank.IsValid())
            {
                bank.Unload();
            }
        }
    }

    public void PostEventPersist(AK.Wwise.Event evt)
    {
        if (evt != null && evt.IsValid())
        {
            evt.Post(gameObject);
        }
    }

    [YarnCommand("PlaySound")]
    public static void PostEventFromYarn(string evt)
    {
        AkSoundEngine.PostEvent(evt, Instance.gameObject);
    }

    // When entering from menus into a level
    public void StartLevel()
    {
        if (!isInLevel)
        {
            isInLevel = true;
            foreach (AK.Wwise.Bank bank in LevelPersistantBanks)
            {
                if (bank != null && bank.IsValid())
                {
                    bank.Load();
                }
            }
            PostEventPersist(LevelMusic);
        }
    }

    // When exiting levels into menus
    public void StopLevel()
    {
        isInLevel = false;
        LevelMusic.Stop(gameObject, 1);
        StartCoroutine(UnloadLevelBanks());
    }

    IEnumerator UnloadLevelBanks()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        foreach (AK.Wwise.Bank bank in LevelPersistantBanks)
        {
            if (bank != null && bank.IsValid())
            {
                bank.Unload();
            }
        }
    }
}
