using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] Slider masterVolumeSlider;
    [SerializeField] Slider musicVolumeSlider;
    [SerializeField] Slider sfxVolumeSlider;
    [SerializeField] AK.Wwise.RTPC masterRTPC;
    [SerializeField] AK.Wwise.RTPC musicRTPC;
    [SerializeField] AK.Wwise.RTPC sfxRTPC;

    private void Start()
    {
        // Load saved volume settings
        masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 100f);
        musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 100f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 100f);
    }

    public void OnMasterVolumeChanged(float value)
    {
        masterRTPC.SetGlobalValue(value);
        PlayerPrefs.SetFloat("MasterVolume", value);
    }

    public void OnMusicVolumeChanged(float value)
    {
        musicRTPC.SetGlobalValue(value);
        PlayerPrefs.SetFloat("MusicVolume", value);
    }

    public void OnSfxVolumeChanged(float value)
    {
        sfxRTPC.SetGlobalValue(value);
        PlayerPrefs.SetFloat("SFXVolume", value);
    }
}
