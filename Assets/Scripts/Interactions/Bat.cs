using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;
using Yarn.Unity;

public class Bat : MonoBehaviour
{
    Light2D Light;

    private void Start()
    {
        Light = GetComponent<Light2D>();
    }

    [YarnCommand("GiveLight")]
    public void GiveLight()
    {
        StartCoroutine(OnActivateLight());
    }

    IEnumerator OnActivateLight()
    {
        float timer = 0.0f;
        float initInner = Light.pointLightInnerRadius;
        float initOuter = Light.pointLightOuterRadius;

        while (timer <= 1)
        {
            timer += Time.deltaTime;
            Light.pointLightInnerRadius = Mathf.Lerp(initInner, 5, timer / 1);
            Light.pointLightOuterRadius = Mathf.Lerp(initOuter, 20, timer / 1);
            yield return null;
        }
        Light.pointLightInnerRadius = 5;
        Light.pointLightOuterRadius = 20;
    }
}
