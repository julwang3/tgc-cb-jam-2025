using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Yarn.Unity;

public class DialogueTriggerKiwi : DialogueTrigger
{
    [SerializeField] private float ignoreTriggerDuration;
    private bool isIgnored = false;
    private bool isNearKiwi = false;
    private float duration = 0f;

    [YarnCommand("KiwiFly")]
    public void KiwiFly()
    {
        OnDialogueComplete();
    }

    [YarnCommand("IgnoreKiwi")]
    public void IgnoreKiwi()
    {
        isIgnored = true;
        isNearKiwi = true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (isIgnored && collision.gameObject.GetComponent<PlayerController>() != null)
        {
            isNearKiwi = true;
        }

        base.OnTriggerEnter2D(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (isIgnored && collision.gameObject.GetComponent<PlayerController>() != null)
        {
            isNearKiwi = false;
        }
    }

    private void Update()
    {
        if (isIgnored && isNearKiwi)
        {
            duration += Time.deltaTime;
            if (duration >= ignoreTriggerDuration)
            {
                // Run dialogue
                InteractionSystem.DialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
                InteractionSystem.DialogueRunner.StartDialogue(dialogueName);

                isIgnored = false;
                duration = 0;
            }
        }
        else
        {
            duration = 0;
        }
    }
}
