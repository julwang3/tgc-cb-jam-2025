using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using Yarn.Unity;

[RequireComponent(typeof(Collider2D))]
public class DialogueTrigger : MonoBehaviour
{
    public enum Ability
    {
        Dash,
        DoubleJump,
        Vision
    };

    [SerializeField] protected string dialogueName;
    [SerializeField] private PlayableDirector givePlayableDirector;
    [SerializeField] private Ability abilityToGive;
    [SerializeField] private UnityEvent onGiveAbility;

    private PlayerController player;
    private bool isTriggered = false;
    protected bool isRunning = false;

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

    protected virtual void OnTriggerEnter2D(Collider2D collision)
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

    protected void OnDialogueComplete()
    {
        InteractionSystem.DialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);

        // Play playable director
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

    protected IEnumerator WaitForInteractionFinish()
    {
        while (givePlayableDirector.state == PlayState.Playing || InteractionSystem.DialogueRunner.IsDialogueRunning)
        {
            yield return null;
        }
        transform.parent.gameObject.SetActive(false);
        isRunning = false;
    }

    public void GiveAbility()
    {
        onGiveAbility.Invoke();
    }
}
