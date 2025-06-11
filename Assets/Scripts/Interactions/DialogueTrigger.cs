using System.Collections;
using System.Linq;
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
        Vision,
        None
    };

    [SerializeField] protected string dialogueName;
    [SerializeField] private PlayableDirector givePlayableDirector;
    [SerializeField] private Ability abilityToGive;
    [SerializeField] private UnityEvent onGiveAbility;
    [SerializeField] private UnityEvent onCompleteEvent;

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
            if (InteractionSystem.DialogueRunner.YarnProject.NodeNames.Contains(dialogueName))
            {
                InteractionSystem.DialogueRunner.StartDialogue(dialogueName);
            }
            else
            {
                OnDialogueComplete();
            }
        }
    }

    protected void OnDialogueComplete()
    {
        InteractionSystem.DialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);

        // Play playable director
        if (givePlayableDirector && ((abilityToGive == Ability.Dash && !PlayerController.HasDash)
            || (abilityToGive == Ability.DoubleJump && !PlayerController.HasDoubleJump)
            || (abilityToGive == Ability.Vision && !PlayerController.HasVision)))
        {
            givePlayableDirector.Play();
            StartCoroutine(WaitForInteractionFinish());
        }
        else
        {
            if (onCompleteEvent != null)
            {
                onCompleteEvent.Invoke();
            }
            isRunning = false;
        }
    }

    protected IEnumerator WaitForInteractionFinish()
    {
        while (givePlayableDirector.state == PlayState.Playing || InteractionSystem.DialogueRunner.IsDialogueRunning)
        {
            yield return null;
        }
        if (transform.parent != null)
        {
            transform.parent.gameObject.SetActive(false);
        }
        if (onCompleteEvent != null)
        {
            onCompleteEvent.Invoke();
        }
        isRunning = false;
    }

    public void GiveAbility()
    {
        onGiveAbility.Invoke();
    }
}
