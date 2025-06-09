using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class StartScene : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] float fadeInDuration;
    [SerializeField] string sceneToLoad;

    private void Start()
    {
        StartCoroutine(RunDialogue());
    }

    private IEnumerator RunDialogue()
    {
        // Start dialogue
        yield return new WaitForSeconds(fadeInDuration);
        dialogueRunner.StartDialogue("Start");

        // Wait for dialogue to finish
        while (dialogueRunner.IsDialogueRunning)
        {
            yield return null;
        }

        // Load tutorial level
        LevelManager.Instance.LoadLevel(sceneToLoad);
    }
}
