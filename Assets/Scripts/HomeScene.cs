using UnityEngine;

public class HomeScene : MonoBehaviour
{
    [SerializeField] private Animator turtle;
    [SerializeField] private Animator kiwi;
    [SerializeField] private Animator bat;
    [SerializeField] private string creditsScene;

    private void Start()
    {
        turtle.gameObject.SetActive(!PlayerController.HasDash);
        kiwi.gameObject.SetActive(!PlayerController.HasDoubleJump);
        bat.gameObject.SetActive(!PlayerController.HasVision);

        if (!PlayerController.HasDoubleJump)
        {
            kiwi.SetTrigger("Jump");
        }
        if (!PlayerController.HasVision)
        {
            bat.SetTrigger("Lamp");
        }
    }

    public void End()
    {
        LevelManager.Instance.LoadLevel(creditsScene);
    }
}
