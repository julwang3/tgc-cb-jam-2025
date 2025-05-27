using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.Interpolators;

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
    [SerializeField] Light2D Light;
    [SerializeField] float LightChangeTime;
    [SerializeField] float LightMinRadius;
    [SerializeField] float LightMaxRadius = 20.0f;

    [Header("Abilities")]
    public bool HasDash = true;
    public bool HasDoubleJump = true;
    public bool HasVision = true;

    // Private variables
    private Rigidbody2D rb;
    private InputAction moveAction;
    private Vector2 forward = Vector2.right;

    private int jumps = 1;
    private bool canJump = true;

    private bool isDashing = false;
    private bool canDash = true;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        moveAction = InputSystem.actions.FindAction("Move");
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
            Debug.Log(context.ReadValueAsButton());
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
        Vector2 originalPos = rb.position;

        while (timer < DashTime)
        {
            timer += Time.deltaTime;
            rb.linearVelocityX = velocity;
            
            yield return new WaitForFixedUpdate();

            // If bumped into a wall, stop dashing
            if (Mathf.Abs(rb.linearVelocityX) < 0.1f)
            {
                Debug.Log("Bumped into wall. Stopping dash");
                RaycastHit2D hit = Physics2D.Raycast(originalPos, forward, DashDistance);
                if (hit)
                {
                    rb.position = hit.point - forward / 2.0f;
                }
                break;
            }
        }

        rb.linearVelocityX = 0.0f;
        rb.linearVelocityY = 0.0f;
        rb.gravityScale = oldGravity;
        isDashing = false;

        yield return new WaitForSeconds(DashCooldown);
        canDash = true;
    }

    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, GroundedDistance);
        return hit.collider != null;
    }

    public void ActivateLight()
    {
        HasVision = true;
        StartCoroutine(OnActivateLight());
    }

    public void DeactivateLight()
    {
        HasVision = false;
        StartCoroutine(OnDeactivateLight());
    }

    IEnumerator OnActivateLight()
    {
        float timer = 0.0f;
        float initInner = Light.pointLightInnerRadius;
        float initOuter = Light.pointLightOuterRadius;

        while (timer <= LightChangeTime)
        {
            timer += Time.deltaTime;
            Light.pointLightInnerRadius = Mathf.Lerp(initInner, LightMaxRadius, timer / LightChangeTime);
            Light.pointLightOuterRadius = Mathf.Lerp(initOuter, LightMaxRadius, timer / LightChangeTime);
            yield return null;
        }
        Light.pointLightInnerRadius = LightMaxRadius;
        Light.pointLightOuterRadius = LightMaxRadius;
    }

    IEnumerator OnDeactivateLight()
    {
        float timer = 0.0f;
        float initInner = Light.pointLightInnerRadius;
        float initOuter = Light.pointLightOuterRadius;

        while (timer <= LightChangeTime)
        {
            timer += Time.deltaTime;
            Light.pointLightInnerRadius = Mathf.Lerp(initInner, 0.0f, timer / LightChangeTime);
            Light.pointLightOuterRadius = Mathf.Lerp(initOuter, LightMinRadius, timer / LightChangeTime);
            yield return null;
        }
        Light.pointLightInnerRadius = 0.0f;
        Light.pointLightOuterRadius = LightMinRadius;
    }
}
