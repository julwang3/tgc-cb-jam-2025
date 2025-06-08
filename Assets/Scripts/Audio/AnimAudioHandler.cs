using UnityEngine;

public class AnimAudioHandler : MonoBehaviour
{
    public void PlaySound(string evt)
    {
        AkSoundEngine.PostEvent(evt, gameObject);
    }
}
