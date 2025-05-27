using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Base Movement")]
    [SerializeField] float MovementSpeed;
    [SerializeField] float JumpForce;
    [SerializeField] float JumpCooldown;
    [SerializeField] float DashDistance;
    [SerializeField] float DashTime;
    [SerializeField] float DashCooldown;
    [SerializeField] float GroundedDistance;

    [Header("Abilities")]
    public bool HasDash = true;
    public bool HasDoubleJump = true;
    public bool HasVision = true;

    // Private variables
    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 forward = Vector2.right;
    private int jumps = 1;
    private LayerMask groundMask;
    private bool canJump = true;
    private bool isDashing = false;
    private bool canDash = true;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
        groundMask = LayerMask.GetMask("Default");
    }

    void Update()
    {
        if (isDashing)
        {
            return;
        }
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        if (moveDirection.x != 0.0f)
        {
            forward.x = moveDirection.x;
            rb.linearVelocityX = moveDirection.x * MovementSpeed;
        }
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (!canJump || isDashing) { return; }
        if (IsGrounded()) { jumps = 0; }

        if (jumps == 0 || (jumps == 1 && HasDoubleJump))
        {
            rb.linearVelocityY = JumpForce;
            jumps++;
            StartCoroutine(StartJumpCooldown());
        }
    }

    IEnumerator StartJumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(JumpCooldown);
        canJump = true;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (canDash && HasDash)
        {
            StartCoroutine(Dash());
        }
    }

    IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float timer = 0.0f;
        float oldGravity = rb.gravityScale;
        rb.gravityScale = 0.0f;
        rb.linearVelocityY = 0.0f;
        rb.MovePosition(rb.position + new Vector2(0.0f, 0.3f)); // Prevent bumping on floor
        float velocity = DashDistance / DashTime * forward.x;

        while (timer < DashTime)
        {
            timer += Time.deltaTime;
            rb.linearVelocityX = velocity;
            yield return null;
        }

        rb.linearVelocityX = 0.0f;
        rb.gravityScale = oldGravity;
        isDashing = false;

        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GroundedDistance, groundMask);
        return hit.collider != null;
    }
}
