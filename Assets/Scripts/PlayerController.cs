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
    [SerializeField] float JumpCooldown;
    [SerializeField] float DashDistance;
    [SerializeField] float DashTime;
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
    private InputAction jumpAction;
    private InputAction dashAction;
    private Vector2 forward = Vector2.right;

    private int jumps = 1;
    private bool isGrounded = false;
    private bool canJump = true;

    private DashState dashState;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
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
                jumps = 0;
            }
            else if (jumps < 1)
            {
                jumps = 1;
            }
        }
    }

    void FixedUpdate()
    {
        if (dashState == DashState.Dashing 
            || (PauseMenuUI.Instance && PauseMenuUI.Instance.IsPaused) 
            || LevelManager.Instance.IsLoading)
        {
            return;
        }
        Vector2 moveDirection = moveAction.ReadValue<Vector2>();
        if (moveDirection.x != 0.0f)
        {
            forward.x = moveDirection.x;
            rb.AddForceX(moveDirection.x * Acceleration);
            rb.linearVelocityX = Mathf.Clamp(rb.linearVelocityX, -MaxSpeed, MaxSpeed);
        }
    }

    public void OnJump(CallbackContext ctx)
    {
        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading)
        {
            if (!canJump || dashState == DashState.Dashing) { return; }

            if (jumps == 0 || (jumps == 1 && HasDoubleJump))
            {
                rb.linearVelocityY = JumpForce;
                jumps++;
                StartCoroutine(StartJumpCooldown());
            }
        }
    }

    IEnumerator StartJumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(JumpCooldown);
        canJump = true;
    }

    public void OnDashStart(CallbackContext ctx)
    {
        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading)
        {
            if (HasDash && dashState == DashState.NotDashing)
            {
                Debug.Log("Charging dash");
                dashState = DashState.Charging;
            }
        }
    }

    public void OnDashCharged(CallbackContext ctx)
    {
        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading)
        {
            if (dashState == DashState.Charging)
            {
                Debug.Log("Dash charged");
                dashState = DashState.Charged;
            }
        }
        else
        {
            dashState = DashState.NotDashing;
        }
    }

    public void OnDashRelease(CallbackContext ctx)
    {
        if ((!PauseMenuUI.Instance || !PauseMenuUI.Instance.IsPaused) && !LevelManager.Instance.IsLoading)
        {
            // Charged enough
            if (dashState == DashState.Charged)
            {
                Debug.Log("Dashing");
                dashState = DashState.Dashing;
                StartCoroutine(Dash());
            }
            else
            {
                Debug.Log("Dash charge canceled");
                dashState = DashState.NotDashing;
            }
        }
        else
        {
            dashState = DashState.NotDashing;
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
}
