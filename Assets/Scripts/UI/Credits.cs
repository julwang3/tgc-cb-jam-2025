using System.Collections;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField] GameObject endImage;
    [SerializeField] GameObject theEnd;

    public void ShowEndImage()
    {
        StartCoroutine(OnShowEndImage());
    }

    IEnumerator OnShowEndImage()
    {
        if (!PlayerController.HasDash && !PlayerController.HasDoubleJump && !PlayerController.HasVision)
        {
            yield return StartCoroutine(LevelManager.Instance.FadeToBlack());
            endImage.SetActive(true);
            yield return StartCoroutine(LevelManager.Instance.FadeFromBlack());
            yield return new WaitForSeconds(5.0f);
        }
        yield return StartCoroutine(LevelManager.Instance.FadeToBlack());
        theEnd.SetActive(true);
        yield return StartCoroutine(LevelManager.Instance.FadeFromBlack());
        yield return new WaitForSeconds(5.0f);
        LevelManager.Instance.LoadLevel("MainMenu");
    }
}
