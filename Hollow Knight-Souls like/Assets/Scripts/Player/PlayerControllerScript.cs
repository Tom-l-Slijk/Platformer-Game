using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour
{
    private Rigidbody2D rb;

    public bool dashEnabled = true;
    public bool movementEnabled = true;
    public bool jumpEnabled = true;
    public bool wallMovementEnabled = true;
    public bool checkMovementDirectionEnabled = true;

    //Movement
    public float speed;
    public float jumpForce;
    public float extraJumpForce;
    private float moveInput;
    private bool isWalking;
    
    //Jumping
    private float jumpTimeCounter;
    public float jumpTime;
    private bool isJumping;
    private float jumpPressedRemember;
    public float jumpPressedRememberTime = 0.2f;

    [Range(0, 1)]
    public float cutJumpHeight;

    private int extraJumps;
    public int extraJumpValue;

    //Dash
    private bool isDashing;
    public float dashForce;
    public float startDashTimer;
    private float currentDashTimer;
    private bool canDash = true;
    public float timeBetweenDash;
    
    //GroundCheck
    private bool isGrounded;
    public Transform feetPos;
    public LayerMask whatIsGround;
    private float groundedRemember;
    public float groundedRememberTime = 0.1f;

    //WallCheck
    private bool isTouchingFront;
    public Transform frontPos;
    private bool canGrab, isGrabbing;
    public float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    public float yWallForce;
    public float xWallForce;
    public float slideSpeed;

    private float grabbingRemember;
    public float grabbingRememberTime = 0.1f;

    private float movementDirectionRemember;
    public float movementDirectionRememberTime = 0.2f;

    public LayerMask whatIsGrabbable;

    //Sprite flip
    private bool isFacingRight = true;
    private bool canFlip;

    //Animatrions
    private Animator anim;

    //Misc.
    private float yVelocity;
    public float checkRadius;
    private float direction;
    private float defaultGravityScale;


    private bool knockback;
    private float knockbackStartTime;
    [SerializeField]
    private float knockbackDuration;

    public Vector2 knockbackSpeed;

    void Start()
    {
        canFlip = true;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        defaultGravityScale = rb.gravityScale;
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Update()
    {
        if (groundedRemember > 0 && jumpPressedRemember > 0)
        {
           isTouchingFront = false; 
           isGrabbing = false;
        }
        if (isGrounded)
        {
            extraJumps = extraJumpValue;
        }
        yVelocity = rb.velocity.y;
        Jump();
        WallMovement();
        CheckMovementDirection();
        UpdateAnimations();
        Dash();
        CheckKnockback();
    }

    public bool GetDashStatus()
    {
        return isDashing;
    }

    public void Knockback(int direction)
    {
        knockback = true;
        knockbackStartTime= Time.time;
        rb.velocity = new Vector2(knockbackSpeed.x * direction, knockbackSpeed.y);
    }

    private void CheckKnockback()
    {
        if (Time.time >= knockbackStartTime + knockbackDuration && knockback)
        {
            knockback = false;
            rb.velocity = new Vector2(0.0f, rb.velocity.y);
        }
    }

    private void Dash()
    {
        if (dashEnabled)
        {
            if (canDash && Input.GetButtonDown("Dash") && moveInput != 0)
            {
                isDashing = true;
                currentDashTimer = startDashTimer;

                movementEnabled = false;
                jumpEnabled = false;
                wallMovementEnabled = false;
                checkMovementDirectionEnabled = false;
                rb.velocity = Vector2.zero;
            }

            if (isDashing)
            {
                rb.velocity = new Vector2(moveInput * dashForce, 1);
                StartCoroutine(DashTimer());

                currentDashTimer -= Time.deltaTime;
                if (currentDashTimer <= 0)
                {
                    isDashing = false;

                    movementEnabled = true;
                    jumpEnabled = true;
                    wallMovementEnabled = true;
                    checkMovementDirectionEnabled = true;
                }
            }
        }
    }

    IEnumerator DashTimer()
    {
        canDash = false;
        yield return new WaitForSeconds(timeBetweenDash);
        canDash = true;
    }

    private void Movement()
    {
        if (movementEnabled)
        {
            moveInput = Input.GetAxisRaw("Horizontal");
            if (wallJumpCounter <= 0)
            {
                if (!isTouchingFront && !knockback)
                { 
                    rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
                } 
            }
            else
            {
                wallJumpCounter -= Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        if (jumpEnabled)
        {
            if (wallJumpCounter <= 0)
            {
                isGrounded = Physics2D.OverlapCircle(feetPos.position ,checkRadius, whatIsGround);

                groundedRemember -= Time.deltaTime;
                if (isGrounded)
                {
                    groundedRemember = groundedRememberTime;
                }

                if (Input.GetButtonDown("Jump") && extraJumps > 0 && yVelocity != 0)
                {
                    rb.velocity = Vector2.up * extraJumpForce;
                    extraJumps --;
                }

                jumpPressedRemember -= Time.deltaTime;
                if (Input.GetButtonDown("Jump"))
                {
                    jumpPressedRemember = jumpPressedRememberTime;
                }

                if (groundedRemember > 0 && jumpPressedRemember > 0)
                {

                    groundedRemember = 0;
                    jumpPressedRemember = 0;
                    anim.SetBool("Jump", true);
                    isJumping = true;
                    jumpTimeCounter = jumpTime;
                    rb.velocity = Vector2.up * jumpForce;
                }

                if (Input.GetButton("Jump") && isJumping == true)
                {
                    if (jumpTimeCounter > 0)
                    {
                        rb.velocity = Vector2.up * jumpForce;
                        jumpTimeCounter -= Time.deltaTime;
                    }
                    else
                    {
                        isJumping = false;
                    }
                }

                if (Input.GetButtonUp("Jump"))
                {
                    if (rb.velocity.y > 0 && !isGrabbing)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * cutJumpHeight);
                    }
                    isJumping = false;
                }
            }
            else
            {
                wallJumpCounter -= Time.deltaTime;
            }
        }
    }

    private void WallMovement()
    {
        if (wallMovementEnabled)
        {
            if (wallJumpCounter <= 0)
            {
                canGrab = Physics2D.OverlapCircle(frontPos.position, .2f, whatIsGrabbable);

                grabbingRemember -= Time.deltaTime;
                if (isGrabbing)
                {
                    grabbingRemember = grabbingRememberTime;
                    extraJumps = 0;
                }

                jumpPressedRemember -= Time.deltaTime;
                if (Input.GetButtonDown("Jump"))
                {
                    jumpPressedRemember = jumpPressedRememberTime;
                }

                movementDirectionRemember -= Time.deltaTime;
                if (isGrabbing && isFacingRight)
                {
                    movementDirectionRemember = movementDirectionRememberTime;
                    direction = 1;
                }
                else if (isGrabbing && !isFacingRight)
                {
                    movementDirectionRemember = movementDirectionRememberTime;
                    direction = -1;
                }

                isGrabbing = false;
                if (canGrab && !isGrounded)
                {
                    
                    if (isFacingRight && moveInput == 1 || !isFacingRight && moveInput == -1)
                    {
                        rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(0, -slideSpeed, float.MaxValue));
                        isGrabbing = true;
                    }
                }

                if (grabbingRemember > 0 && jumpPressedRemember > 0)
                {
                    movementDirectionRemember = 0;
                    grabbingRemember = 0;
                    jumpPressedRemember = 0;

                    isGrabbing = true;
                    if (Input.GetButtonDown("Jump"))
                    {
                        rb.gravityScale = 2;
                        wallJumpCounter = wallJumpTime;
                        // StartCoroutine(flipTimer());
                        if (isFacingRight && direction > 0)
                        {
                            Flip();
                        }
                        else if (!isFacingRight && direction < 0)
                        {
                            Flip();
                        }
                        rb.velocity = new Vector2(-direction * xWallForce, yWallForce);
                        isGrabbing = false;
                    }
                }
                else
                {
                    rb.gravityScale = defaultGravityScale;
                }
            }
            else
            {
                wallJumpCounter -= Time.deltaTime;
            }
        }
    }

    IEnumerator flipTimer()
    {
        yield return new WaitForSeconds(0.1f);
        Flip();
    }

    private void CheckMovementDirection()
    {
        if (checkMovementDirectionEnabled)
        {
            if (isFacingRight && moveInput < 0 && wallJumpCounter <= 0)
            {
                Flip();
            }
            else if (!isFacingRight && moveInput > 0 && wallJumpCounter <= 0)
            {
                Flip();
            }

            if(rb.velocity.x > 0.01f || rb.velocity.x < -0.01f)
            {
                isWalking = true;
            }
            else
            {
                isWalking = false;
            }
        }
    }

    public void DisableFlip()
    {
        canFlip = false;
    }

    public void EnableFlip()
    {
        canFlip = true;
    }

    public bool GetFacingDirection()
    {
        return isFacingRight;
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isGrounded", isGrounded && !isGrabbing);
        anim.SetBool("Jump", !isGrounded && !isGrabbing);
        anim.SetFloat("yVelocity", yVelocity);
        anim.SetBool("isWalking", isWalking);
        anim.SetBool("isGrabbing", isGrabbing && !isGrounded);
        anim.SetBool("isDashing", isDashing);
    }

    private void Flip()
    {
        if (canFlip && !knockback)
        {
            isFacingRight = !isFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(feetPos.position, checkRadius);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(frontPos.position, checkRadius);
    }
}
