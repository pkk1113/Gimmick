using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerCtrl : MonoBehaviour
{
    [Header("Key")]
    [SerializeField] KeyCode jumpKey; // 점프 키

    [Header("Walk")]
    [SerializeField] float walkSpeed; // 걷는 속도
    [Space(5)]

    [Header("Jump")]
    [SerializeField] float jumpUpSpeed; // 점프 속도
    [SerializeField] float jumpDownSpeed; // 점프 후 떨어지는 속도
    [SerializeField] int jumpMaxSteps; // 점프 최대 프레임
    [SerializeField] float checkWidth; // 확인 버니
    [SerializeField] float groundCheckHeight; // 바닥 확인 길이
    [SerializeField] float roofCheckHeight; // 천장 확인 길이
    [SerializeField] LayerMask layerMask; // 점프 구분 마스크

    [Space(5)]

    [Header("Audio")]
    [SerializeField] AudioClip walkAudioClip; // 걷는 소리
    [SerializeField] AudioClip jumpAudioClip; // 점프 소리
    [Space(5)]

    /// Components
    Animator animator;
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    new Rigidbody2D rigidbody2D;

    // Fields
    float xAxis = 0f;
    bool jumpKeyPressed = false;
    bool walking = false; // 걷는 중
    bool lookingLeft = false; // 왼쪽 보는 중
    bool jumping = false; // 올라가는 중 (내려가는 상태는 jumping으로 보지 않습니다.)
    int jumpSteps = 0;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        GetInput();
        Walk();
        Flip();
    }

    private void FixedUpdate()
    {
        Jump();
    }

    private void Jump()
    {
        /// velocity
        if (jumpKeyPressed && jumping)
        {
            if (jumpSteps < jumpMaxSteps)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpUpSpeed);
                jumpSteps++;
            }
            else
            {
                StopJump();
            }
        }
        else
        {
            StopJump();
        }

        if (rigidbody2D.velocity.y < -jumpDownSpeed)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -jumpDownSpeed);
        }
    }

    private void StopJump()
    {
        /// animation
        animator.SetBool("jumping", false);

        /// set fields
        jumping = false;
        jumpSteps = 0;
    }

    private void Flip()
    {
        if (xAxis == 0f) return;

        lookingLeft = xAxis < 0;

        spriteRenderer.flipX = lookingLeft;
    }

    private void Walk()
    {
        walking = Mathf.Abs(xAxis) != 0f;

        /// audio
        if (walking && !audioSource.isPlaying && Grounded())
        {
            audioSource.PlayOneShot(walkAudioClip);
        }

        /// animation
        animator.SetBool("walking", walking);

        /// velocity
        rigidbody2D.velocity = new Vector2(walkSpeed * xAxis, rigidbody2D.velocity.y);
    }

    private void GetInput()
    {
        xAxis = Input.GetAxis("Horizontal");

        jumpKeyPressed = Input.GetKey(jumpKey);

        if (Input.GetKeyDown(jumpKey) && Grounded())
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        /// animation
        animator.SetBool("jumping", true);

        /// audio
        audioSource.PlayOneShot(jumpAudioClip);

        /// set field
        jumping = true;
    }

    private bool Grounded()
    {
        return
            Physics2D.Raycast(transform.position - transform.right * checkWidth, Vector2.down, groundCheckHeight, layerMask) ||
            Physics2D.Raycast(transform.position + transform.right * checkWidth, Vector2.down, groundCheckHeight, layerMask);
    }

    private bool Roofed()
    {
        return
            Physics2D.Raycast(transform.position - transform.right * checkWidth, Vector2.up, groundCheckHeight, layerMask) ||
            Physics2D.Raycast(transform.position + transform.right * checkWidth, Vector2.up, groundCheckHeight, layerMask);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position - transform.right * checkWidth, Vector2.down * groundCheckHeight);
        Gizmos.DrawRay(transform.position + transform.right * checkWidth, Vector2.down * groundCheckHeight);
    }
}