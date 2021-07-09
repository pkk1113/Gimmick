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
    [SerializeField] float jumpSpeed; // 점프 속도
    [SerializeField] float fallSpeed; // 점프 후 떨어지는 속도
    [SerializeField] int jumpMaxSteps; // 점프 최대 프레임
    [SerializeField] float checkWidth; // 확인 버니
    [SerializeField] float groundCheckHeight; // 바닥 확인 길이
    [SerializeField] float roofCheckHeight; // 천장 확인 길이
    [SerializeField] LayerMask layerMask; // 점프 구분 마스크
    [Space(5)]

    [Header("Audio")]
    [SerializeField] AudioClip walkAudioClip; // 걷는 소리
    [SerializeField] AudioClip jumpAudioClip; // 점프 소리
    [SerializeField] AudioClip landAudioClip; // 착지 소리
    [Space(5)]

    /// Components
    Animator animator;
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    new Rigidbody2D rigidbody2D;

    // Fields
    float xAxis = 0f;
    bool jumpKeyPressed = false; // 점프키 입력 중
    bool walking = false; // 걷는 중
    bool lookingLeft = false; // 왼쪽 보는 중
    bool jumping = false; // 올라가는 중 
    bool falling = false; // 떨어지는 중
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
        Jump_Fall();
    }

    private void Jump_Fall()
    {
        if (jumping)
        {
            if (jumpKeyPressed)
            {
                if (jumpSteps < jumpMaxSteps && !Roofed())
                {
                    rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, jumpSpeed);
                    jumpSteps++;
                }
                else
                {
                    StartFall();
                }
            }
            else
            {
                StartFall();
            }
        }

        if (falling)
        {
            /// keep falling speed
            if (rigidbody2D.velocity.y < -fallSpeed)
            {
                rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, -fallSpeed);
            }

            if (Grounded())
            {
                StopFall();
            }
        }
        else
        {
            if (rigidbody2D.velocity.y < 0f)
            {
                StartFall();
                Debug.Log("falling~~");
            }
        }
    }

    private void StopFall()
    {
        /// fields
        falling = false;

        /// animation
        animator.SetBool("falling", false);

        /// audio
        audioSource.PlayOneShot(landAudioClip);
    }

    private void StartFall()
    {
        /// fields
        jumping = false;
        jumpSteps = 0;
        falling = true;

        /// velocity
        rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0f);

        /// animation
        animator.SetBool("jumping", false);
        animator.SetBool("falling", true);
    }

    private void Flip()
    {
        if (xAxis == 0f) return;

        lookingLeft = xAxis < 0;

        spriteRenderer.flipX = lookingLeft;
    }

    private void Walk()
    {
        /// set fields
        walking = Mathf.Abs(xAxis) != 0f;

        /// velocity
        rigidbody2D.velocity = new Vector2(walkSpeed * xAxis, rigidbody2D.velocity.y);

        /// animation
        animator.SetBool("walking", walking);

        /// audio
        if (walking && !audioSource.isPlaying && Grounded())
        {
            audioSource.PlayOneShot(walkAudioClip);
        }
    }

    private void GetInput()
    {
        xAxis = Input.GetAxis("Horizontal");

        jumpKeyPressed = Input.GetKey(jumpKey);

        if (jumpKeyPressed && Grounded() && !jumping)
        {
            StartJump();
        }
    }

    private void StartJump()
    {
        /// set fields
        jumping = true;

        /// animation
        animator.SetBool("jumping", true);

        /// audio
        audioSource.PlayOneShot(jumpAudioClip);
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