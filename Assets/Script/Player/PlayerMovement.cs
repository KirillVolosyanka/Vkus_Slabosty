using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerBody;
    private Vector2 moveInput;

    public PlayerState playerState;
    public bool IsFacingRight { get; private set; }

    public float LastPressedJumpTime { get; private set; }
    public float LastOnGroundTime { get; private set; }

    public bool IsJumping { get; private set; }

    private bool canDash = true;
    private bool isDashing;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 1f;

    [SerializeField] private Transform groundCheckerPoint;
    [SerializeField] private Vector2 groundCheckerSize = new(0.5f, 0.03f);

    [Header("Layers")]
    [SerializeField] private LayerMask whatIsGround;

    private void Awake()
    {
        IsFacingRight = true;
        LastOnGroundTime = 0f;
        playerBody = GetComponent<Rigidbody2D>();
    }


    private void Update()
    {
        if (isDashing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;

        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        if (moveInput.x != 0 && !IsJumping)
        {
            CheckFaceDirection(moveInput.x > 0.01);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (!IsJumping)
        {
            if (Physics2D.OverlapBox(groundCheckerPoint.position, groundCheckerSize, 0, whatIsGround))
            {
                LastOnGroundTime = playerState.coyoteTime;
            }
        }

        if (IsJumping && playerBody.velocity.y < 0)
        {
            IsJumping = false;
        }

        if(CanJump() && LastPressedJumpTime > 0)
        {
            IsJumping = true;
            Jump();
        }
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        
        Run(1); 
    }

    private void Run(float lerpAmount)
    {
        float targetSpeed = moveInput.x * playerState.runMaxSpeed;

        targetSpeed = Mathf.Lerp(playerBody.velocity.x, targetSpeed, lerpAmount);

        float accelRate;
        accelRate = (Mathf.Abs(targetSpeed) > 0.01) ? playerState.runAccelAmount: playerState.runDeccelAmount;

        float speedDif = targetSpeed - playerBody.velocity.x;
        float movement = speedDif * accelRate;

        playerBody.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void Jump()
    {
        LastOnGroundTime = 0;
        LastPressedJumpTime = 0;

        float force = playerState.jumpForce;

        if (playerBody.velocity.y < 0)
        {
            force -= playerBody.velocity.y;
        }
        playerBody.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void OnJumpInput()
    {
        LastPressedJumpTime = playerState.jumpInputBufferTime;
    }

    private bool CanJump()
    {
        return LastOnGroundTime > 0 && !IsJumping;
    }
    private void CheckFaceDirection(bool isMovingRight)
    {
        if (IsFacingRight != isMovingRight)
        {
            Turn();
        }
    }

    public void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        IsFacingRight = !IsFacingRight;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(groundCheckerPoint.position, groundCheckerSize);
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = playerBody.gravityScale;
        playerBody.gravityScale = 0f;
        playerBody.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        yield return new WaitForSeconds(dashingTime);
        playerBody.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
