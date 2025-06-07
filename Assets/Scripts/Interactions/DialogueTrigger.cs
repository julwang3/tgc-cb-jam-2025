using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    public enum Ability
    {
        Dash,
        DoubleJump,
        Vision
    };

    [SerializeField] private string dialogueName;
    [SerializeField] private PlayableDirector givePlayableDirector;
    [SerializeField] private Ability abilityToGive;

    private PlayerController player;
    private bool isTriggered = false;
    private bool isRunning = false;

    public bool IsInteractionRunning => isRunning;

    private void Start()
    {
        player = PlayerController.Instance;

        InteractionSystem.Instance.AddDialogueTrigger(this);
    }

    private void OnDisable()
    {
        isRunning = false;
    }

    private void OnDestroy()
    {
        InteractionSystem.Instance.RemoveDialogueTrigger(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isTriggered && collision.gameObject.GetComponent<PlayerController>() != null)
        {
            isTriggered = true;
            isRunning = true;

            // Run dialogue
            InteractionSystem.DialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
            InteractionSystem.DialogueRunner.StartDialogue(dialogueName);
        }
    }

    private void OnDialogueComplete()
    {
        InteractionSystem.DialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);

        if (givePlayableDirector && ((abilityToGive == Ability.Dash && !player.HasDash)
            || (abilityToGive == Ability.DoubleJump && !player.HasDoubleJump)
            || (abilityToGive == Ability.Vision && !player.HasVision)))
        {
            givePlayableDirector.Play();
            StartCoroutine(WaitForInteractionFinish());
        }
        else
        {
            isRunning = false;
        }
    }

    private IEnumerator WaitForInteractionFinish()
    {
        while (givePlayableDirector.state == PlayState.Playing)
        {
            yield return null;
        }
        isRunning = false;
    }
}
