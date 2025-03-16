using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class KeyboardMove : MonoBehaviour
{
    [Header("Postprocesado")]
    public Volume volume;
    private MotionBlur motionBlur;

    [Header("Movimiento")]
    public float runMultiplier = 2f;
    public float playerSpeed = 5f;
    public float runDuration = 5f;

    [Header("Cooldowns")]
    public float runCooldown = 3f;
    public float slideCooldown = 3f;

    [Header("Crouch & Slide")]
    public float crouchSpeed = 0.5f;
    public float slideTime = 0.5f;
    public bool isSliding = false;
    public bool isCrouching = false;
    public BoxCollider2D regularColl;
    public BoxCollider2D crouchColl;

    [Header("Crouch Detection")]
    public Transform headCheck;
    public float headCheckRadius = 0.2f;
    public LayerMask groundLayer; 
    
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 moveDirection;
    private bool isRunning = false;
    private bool canRun = true;
    private bool canSlide = true;

    private float currentSpeed;
    private float acceleration = 3f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (volume != null && volume.profile.TryGet(out motionBlur))
        {
            Debug.Log("Motion Blur encontrado.");
        }
        else
        {
            Debug.LogError("Motion Blur no encontrado.");
        }
    }

    private void Update()
    {
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleCrouch();
    }

    //------------------------------------------------------------------------------- Animaciones

private void UpdateAnimation()
{
    float speedFactor = Mathf.Clamp01(currentSpeed / playerSpeed);
    float crouchValue = isCrouching ? -1f : 0f;
    float runValue = isRunning ? 2f : 0f;

    animator.SetFloat("HorizontalSpeed", Mathf.Abs(moveDirection.x)+runValue);
    animator.SetFloat("VerticalSpeed", crouchValue);
}

    //----------------------------------------------------------------------------- Movimiento del jugador
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }

    private void HandleMovement()
    {
        float targetSpeed = isRunning ? playerSpeed * runMultiplier : playerSpeed;
        if (isCrouching) targetSpeed *= crouchSpeed;
        
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);

        rb.linearVelocity = new Vector2(moveDirection.x * currentSpeed, rb.linearVelocity.y);

        if (moveDirection.x != 0)
            spriteRenderer.flipX = moveDirection.x < 0;
    }

    //------------------------------------------------------------------------------- Correr
    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.started && canRun && !isCrouching)
        {
            isRunning = true;
            StartCoroutine(RunCooldown());
            if (motionBlur != null) motionBlur.active = true;
        }
        else if (context.canceled)
        {
            isRunning = false;
            if (motionBlur != null) motionBlur.active = false;
        }
    }

    private IEnumerator RunCooldown()
    {
        yield return new WaitForSeconds(runDuration);
        isRunning = false;
        canRun = false;
        if (motionBlur != null) motionBlur.active = false;
        yield return new WaitForSeconds(runCooldown);
        canRun = true;
    }

    //------------------------------------------------------------------------------- Deslizarse
    public void OnSlide(InputAction.CallbackContext context)
    {
        if (context.started && canSlide && isRunning)
        {
            PerformSlide();
        }
    }

    private void PerformSlide()
    {
        isSliding = true;
        animator.SetBool("IsSliding", true);

        rb.linearVelocity = new Vector2(moveDirection.x * playerSpeed * 5f, rb.linearVelocity.y);

        StartCoroutine(StopSlide());
        StartCoroutine(SlideCooldown());
    }

    private IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(slideTime);
        isSliding = false;
        animator.SetBool("IsSliding", false);
    }

    private IEnumerator SlideCooldown()
    {
        canSlide = false;
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    //------------------------------------------------------------------------------- Crouch
private void HandleCrouch()
{
bool hasCeiling = !CanStandUp();

    if (moveDirection.y < -0.5f || hasCeiling) 
    {
        if (isRunning)
        {
            isRunning = false;
            if (motionBlur != null) motionBlur.active = false;
        }

        if (!isCrouching)
        {
            isCrouching = true;
            regularColl.enabled = false;
            crouchColl.enabled = true;
        }
    }
    else if (moveDirection.y >= 0 && !hasCeiling) 
    {
        isCrouching = false;
        regularColl.enabled = true;
        crouchColl.enabled = false;
    }

    animator.SetFloat("VerticalSpeed", isCrouching ? -1f : 0f);
}

//------------------------------------------------------------------------------- Checar si hay espacio arriba
private bool CanStandUp()
{
    return !Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
}
}
