using UnityEngine;

public class PopupSprite : MonoBehaviour
{
    [SerializeField] float openRadius;

    private Animator animator;
    private bool isOpen = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen) { return; }

        if (Vector2.Distance(transform.position, PlayerController.Instance.transform.position) <= openRadius)
        {
            animator.enabled = true;
            isOpen = true;
        }
    }
}
