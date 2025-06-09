using System.Collections;
using UnityEngine;
using Yarn.Unity;

public class StartScene : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] float fadeInDuration;
    [SerializeField] string sceneToLoad;
    [SerializeField] AK.Wwise.State PreambleComplete;

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
        PreambleComplete.SetValue();
        // Load tutorial level
        LevelManager.Instance.LoadLevel(sceneToLoad);
    }
}
