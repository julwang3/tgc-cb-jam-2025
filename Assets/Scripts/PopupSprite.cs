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
        animator.enabled = false;
        GetComponent<SpriteRenderer>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isOpen) 
        { 
            // Using chessboard distance for performance reasons
            if (ChessboardDistance(PlayerController.Instance.transform.position) > openRadius)
            {
                animator.SetTrigger("Popdown");
                isOpen = false;
            }
        }
        else
        {
            if (ChessboardDistance(PlayerController.Instance.transform.position) <= openRadius)
            {
                if (animator.enabled)
                {
                    animator.SetTrigger("Popup");
                }
                else
                {
                    animator.enabled = true;
                }
                isOpen = true;
            }
        }
    }

    float ChessboardDistance(Vector2 other)
    {
        return Mathf.Max(Mathf.Abs(other.x - transform.position.x), Mathf.Abs(other.y - transform.position.y));
    }
}
