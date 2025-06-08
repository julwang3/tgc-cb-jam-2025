using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[RequireComponent(typeof(DialogueRunner))]
public class InteractionSystem : MonoBehaviour
{
    public static InteractionSystem Instance;
    public static DialogueRunner DialogueRunner;
    public bool IsInteractionRunning => isRunning || DialogueRunner.IsDialogueRunning;

    private List<DialogueTrigger> dialogueTriggers;
    private bool isRunning = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        dialogueTriggers = new List<DialogueTrigger>();
    }

    private void Start()
    {
        DialogueRunner = GetComponent<DialogueRunner>();
    }

    private void Update()
    {
        for (int i = 0; i < dialogueTriggers.Count; i++)
        {
            if (dialogueTriggers[i].IsInteractionRunning)
            {
                isRunning = true;
                return;
            }
        }
        isRunning = false;
    }

    public void AddDialogueTrigger(DialogueTrigger trigger)
    {
        dialogueTriggers.Add(trigger);
    }

    public void RemoveDialogueTrigger(DialogueTrigger trigger)
    {
        if (dialogueTriggers.Contains(trigger))
        {
            dialogueTriggers.Remove(trigger);
        }
    }

    [YarnCommand("GiveDash")]
    public void GiveDash()
    {
        PlayerController.Instance.HasDash = false;
        for (int i = 0; i < dialogueTriggers.Count; i++)
        {
            dialogueTriggers[i].GiveAbility();
        }
    }

    [YarnCommand("GiveDoubleJump")]
    public void GiveDoubleJump()
    {
        PlayerController.Instance.HasDoubleJump = false;
        for (int i = 0; i < dialogueTriggers.Count; i++)
        {
            dialogueTriggers[i].GiveAbility();
        }
    }

    [YarnCommand("GiveVision")]
    public void GiveVision()
    {
        PlayerController.Instance.HasVision = false;
        for (int i = 0; i < dialogueTriggers.Count; i++)
        {
            dialogueTriggers[i].GiveAbility();
        }
    }
}
