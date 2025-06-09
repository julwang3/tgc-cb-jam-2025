using UnityEngine;
using UnityEngine.Events;
using Yarn;
using Yarn.Unity;

public class DialogueTriggerDashArea : DialogueTrigger
{
    [SerializeField] private UnityEvent tortoiseEnterEvent;

    [YarnCommand("EnterTortoise")]
    public void EnterTortoise()
    {
        if (tortoiseEnterEvent != null)
        {
            tortoiseEnterEvent.Invoke();
        }
    }
}
