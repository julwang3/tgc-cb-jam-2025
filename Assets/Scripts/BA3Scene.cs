using UnityEngine;

public class BA3Scene : MonoBehaviour
{
    [SerializeField] private Transform turtle;
    [SerializeField] private Transform kiwi;
    [SerializeField] private Transform bat;

    private PlayerController playerController;

    private void Start()
    {
        playerController = PlayerController.Instance;
        
        turtle.gameObject.SetActive(!PlayerController.HasDash);
        kiwi.gameObject.SetActive(!PlayerController.HasDoubleJump);
        bat.gameObject.SetActive(!PlayerController.HasVision);
    }

    public void StartTortoiseDash()
    {
        // playerController.transform.parent = turtle;
        // PlayerController.Instance
    }

    public void EndTortoiseDash()
    {
        if (!PlayerController.HasDash)
        {
            // playerController.transform.parent = null;
            playerController.transform.position = new Vector3(33.5f, 0.71f, 0.0f);
        }
    }

    public void EndKiwiDoubleJump()
    {
        if (!PlayerController.HasDoubleJump)
        {
            playerController.transform.position = new Vector3(58.77f, 1f, 0.0f);
        }
    }

    public void EndTortoiseDash2()
    {
        if (!PlayerController.HasDash)
        {
            // playerController.transform.parent = null;
            playerController.transform.position = new Vector3(80.82f, -1.91f, 0.0f);
        }
    }

    public void EndBatVision()
    {
        if (!PlayerController.HasVision)
        {
            playerController.transform.position = new Vector3(117.51f, -1.12f, 0.0f);
            PlayerController.HasVision = true;
        }
    }
}
