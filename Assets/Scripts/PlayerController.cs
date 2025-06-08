using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines.Interpolators;
using static UnityEngine.InputSystem.InputAction;

public enum DashState
{
    NotDashing,
    Charging,
    Charged,
    Dashing
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Base Movement")]
    [SerializeField] float Acceleration;
    [SerializeField] float MaxSpeed;
    [SerializeField] float JumpForce;
    [SerializeField] float JumpStart;
    [SerializeField] float JumpCooldown;
    [SerializeField] float DashDistance;
    [SerializeField] float DashTime;
    [SerializeField] float GroundedDistance;
    [SerializeField] Light2D Light;
    [SerializeField] float LightChangeTime;
    [SerializeField] float LightMinRadius;
    [SerializeField] float LightMaxRadius = 20.0f;

    [Header("Abilities")]
    public static bool HasDash = true;
    public static bool HasDoubleJump = true;
    public static bool HasVision = true;

    [Header("Audio")]
    [SerializeField] AK.Wwise.Event FootstepSfx;
    [SerializeField] AK.Wwise.Event JumpSfx;
    [SerializeField] AK.Wwise.Event JumpMidairSfx;
    [SerializeField] AK.Wwise.Event DashChargeStart;
    [SerializeField] AK.Wwise.Event DashChargeReady;
    [SerializeField] AK.Wwise.Event DashExecuteSfx;

    // Private variables
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction dashAction;
    private Vector2 forward = Vector2.right;

    private int jumps = 1;
    private bool isGrounded = false;
    private bool canJump = true;
    private bool canMove = true;

    private DashState dashState;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        dashAction = InputSystem.actions.FindAction("Dash");

        jumpAction.started += OnJump;
        dashAction.started += OnDashStart;
        dashAction.performed += OnDashCharged;
        dashAction.canceled += OnDashRelease;

        if (LevelManager.Instance && LevelManager.Instance.HasSpawnPos)
        {
            rb.position = LevelManager.Instance.SpawnPos;
            Physics.SyncTransforms();
            LevelManager.Instance.HasSpawnPos = false; // Reset spawn position
        }
    }

    public static void Reset()
    {
        HasDash = true;
        HasDoubleJump = true;
        HasVision = true;
    }
    private void OnDestroy()
    {
        jumpAction.started -= OnJump;
        dashAction.started -= OnDashStart;
        dashAction.performed -= OnDashCharged;
        dashAction.canceled -= OnDashRelease;
    }

    private void Update()
    {
        isGrounded = IsGrounded();

        if (canJump)
        {
            if (isGrounded)
            {
                if (canJump)
                {
                    jumps = 0;
                }

                animator.SetInteger("Jump", jumps);
            }
            // else if (jumps < 1)
            // {
            //     jumps = 1;
            // }
        }
    }

    void FixedUpdate()
    {
        if (canMove && dashState == DashState.NotDashing
            && !(PauseMenuUI.Instance && PauseMenuUI.Instance.IsPaused)
            && !(LevelManager.Instance && LevelManager.Instance.IsLoading)
            && !(InteractionSystem.Instance && InteractionSystem.Instance.IsInteractionRunning))
        {
            Vector2 moveDirection = moveAction.ReadValue<Vector2>();
            if (moveDirection.x != 0.0f)
            {
                forward.x = moveDirection.x;
                rb.AddForceX(moveDirection.x * Acceleration);
                rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -MaxSpeed, MaxSpeed);

                transform.rotation = moveDirection.x > 0.0f ? Quaternion.identity : Quaternion.Euler(new Vector3(0, 180, 0));
            }
            else if (isGrounded)
            {
                rb.linearVelocityX = 0;
            }
        }

        animator.SetBool("Walk", rb.linearVelocityX != 0.0f);
    }

    public void SetPosition(Vector2 position)
    {
        rb.position = position;
        Physics.SyncTransforms();
    }

    public void OnJump(CallbackContext ctx)
    {
        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading
            && (!InteractionSystem.Instance || !InteractionSystem.Instance.IsInteractionRunning))
        {
            if (!canJump || dashState != DashState.NotDashing) { return; }

            if (jumps == 0 || (jumps == 1 && HasDoubleJump))
            {
                animator.SetInteger("Jump", jumps + 1);
                StartCoroutine(StartJumpCooldown());
            }
        }
    }

    IEnumerator StartJumpCooldown()
    {
        canJump = false;

        // Crouch to jump
        if (jumps == 0 && isGrounded)
        {
            canMove = false;

            float linearVelocityX = rb.linearVelocityX;
            rb.linearVelocityX = 0;
            yield return new WaitForSeconds(JumpStart);
            rb.linearVelocityX = linearVelocityX;

            canMove = true;
        }

        PlayJump();
        rb.linearVelocityY = JumpForce;
        yield return new WaitForSeconds(JumpCooldown);

        jumps++;
        canJump = true;

        // Jump finished
        if (jumps == 1)
        {
            while (!isGrounded)
            {
                yield return null;
            }
            animator.SetTrigger("JumpDone");
        }
    }

    public void OnDashStart(CallbackContext ctx)
    {
        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading
            && (!InteractionSystem.Instance || !InteractionSystem.Instance.IsInteractionRunning))
        {
            if (HasDash && dashState == DashState.NotDashing)
            {
                Debug.Log("Charging dash");
                dashState = DashState.Charging;
                DashChargeStart.Post(gameObject);
                animator.SetTrigger("DashCharging");
            }
        }
    }

    public void OnDashCharged(CallbackContext ctx)
    {
        DashChargeStart.Stop(gameObject);

        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading
            && (!InteractionSystem.Instance || !InteractionSystem.Instance.IsInteractionRunning))
        {
            if (dashState == DashState.Charging)
            {
                Debug.Log("Dash charged");
                dashState = DashState.Charged;
                DashChargeReady.Post(gameObject);
                animator.SetTrigger("DashCharged");
            }
        }
        else
        {
            dashState = DashState.NotDashing;
            animator.SetTrigger("DashCanceled");
        }
    }

    public void OnDashRelease(CallbackContext ctx)
    {
        DashChargeStart.Stop(gameObject);
        DashChargeReady.Stop(gameObject);

        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading
            && (!InteractionSystem.Instance || !InteractionSystem.Instance.IsInteractionRunning))
        {
            // Charged enough
            if (dashState == DashState.Charged)
            {
                Debug.Log("Dashing");
                dashState = DashState.Dashing;
                animator.SetTrigger("Dashing");
                DashExecuteSfx.Post(gameObject);
                StartCoroutine(Dash());
            }
            else
            {
                Debug.Log("Dash charge canceled");
                dashState = DashState.NotDashing;
                animator.SetTrigger("DashCanceled");
            }
        }
        else
        {
            dashState = DashState.NotDashing;
            animator.SetTrigger("DashCanceled");
        }
    }

    IEnumerator Dash()
    {
        float timer = 0.0f;
        float oldGravity = rb.gravityScale;
        rb.gravityScale = 0.0f;
        rb.linearVelocityY = 0.0f;
        rb.MovePosition(rb.position + new Vector2(0.0f, 0.3f)); // Prevent bumping on floor
        Physics.SyncTransforms();

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
                    Physics.SyncTransforms();
                }
                break;
            }
        }

        rb.linearVelocityX = 0.0f;
        rb.linearVelocityY = 0.0f;
        rb.gravityScale = oldGravity;
        dashState = DashState.NotDashing;
        Debug.Log("Dash complete");
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

    public void PlayFootstep()
    {
        if (isGrounded)
        {
            FootstepSfx.Post(gameObject);
        }
    }

    public void PlayJump()
    {
        if (isGrounded)
        {
            JumpSfx.Post(gameObject);
        }
        else
        {
            JumpMidairSfx.Post(gameObject); 
        }
    }
}