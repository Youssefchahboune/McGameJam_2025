using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // movement variables
    public float playerSpeed = 5f;
    private Rigidbody2D playerRb;
    public SpriteRenderer playerSprite;
    private float originalPlayerSpeed;
    private float originalPlayerGravity;
    public Animator animator;
    private AudioSource[] audioSources;

    // jump variables
    public float jumpForce = 5f;
    private bool isGrounded = true;
    private bool isJumping = false;
    private float jumpTimeCounter;
    public float jumpTime;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;

    // Dash variables
    private bool isDashing = false;
    private bool canDash = true;
    public float timeBeforeDashing = 0.5f;
    public float dashSpeed = 1f;
    public float dashingTime = 1f;
    public float dashcoolDown = 1f;
    public GameObject dashAfterEffectParticlesGO;
    public ParticleSystem dashAfterEffectParticles;

    private void Awake()
    {
        dashAfterEffectParticles.Stop();
    }

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
        originalPlayerSpeed = playerSpeed;
        originalPlayerGravity = playerRb.gravityScale;
        dashAfterEffectParticles = dashAfterEffectParticlesGO.GetComponent<ParticleSystem>();
        audioSources = GetComponents<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isDashing)
        {
            return;
        }
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        Jump();
        Move();
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !Input.GetKeyDown(KeyCode.D) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.Space))
        {
            isJumping = false;
            canDash = false;
            isDashing = true;
            StartCoroutine(Dash());
        }
    }

    public void Move()
    {
        if (isDashing)
        {
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            if (!isDashing)
            {
                playerSprite.flipX = true;
            }

            if (isGrounded && !isJumping)
            {
            
                animator.SetBool("isRunning", true);
                if (!audioSources[0].isPlaying)
                {
                    audioSources[0].Play();
                }

            }

            playerRb.velocity = new Vector2(-playerSpeed, playerRb.velocity.y);

        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (!isDashing)
            {
                playerSprite.flipX = false;
            }

            if (isGrounded && !isJumping)
            {

                animator.SetBool("isRunning", true);
                if (!audioSources[0].isPlaying)
                {
                    audioSources[0].Play();
                }

            }
            
            playerRb.velocity = new Vector2(playerSpeed, playerRb.velocity.y);
            

        }
        else
        {
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            animator.SetBool("isRunning", false);
            audioSources[0].Stop();
        }
    }

    public void Jump()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.LeftShift)))
        {
            isJumping = true;
            isGrounded = false;
            jumpTimeCounter = jumpTime;
            animator.SetBool("isRunning", false);
            animator.SetBool("isJumping", true);
            playerRb.velocity = Vector2.up * jumpForce;

        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter > 0f)
            {
                playerRb.velocity = Vector2.up * jumpForce;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        if (isGrounded && !isJumping)
        {
            isJumping = false;
            animator.SetBool("isJumping", false);
            animator.SetBool("isRunning", false);
        }
    }

    public IEnumerator Dash()
    {
        playerRb.velocity = Vector2.zero;
        animator.SetBool("isRunning", false);
        animator.SetBool("isjumping", false);
        playerRb.gravityScale = 0f;

        yield return new WaitForSeconds(timeBeforeDashing);

        playerRb.velocity = new Vector2((PlayerFacing() == "RIGHT" ? 1 : -1) * dashSpeed, 0f);

        

        if (PlayerFacing() == "RIGHT")
        {
            dashAfterEffectParticlesGO.transform.localScale = new Vector3(1,1);

        } else if (PlayerFacing() == "LEFT")
        {
            dashAfterEffectParticlesGO.transform.localScale = new Vector3(-1, 1);
            
        }
   
        dashAfterEffectParticles.Play();

        yield return new WaitForSeconds(dashingTime);
        playerRb.gravityScale = originalPlayerGravity;
        dashAfterEffectParticles.Stop();
        isDashing = false;

        yield return new WaitForSeconds(dashcoolDown);
        canDash = true;
    }

    public String PlayerFacing()
    {
        bool isflipped = playerSprite.flipX;

        if (isflipped)
        {
            // Facing left
            return "LEFT";
        }
        else
        {
            // Facing right
            return "RIGHT";
        }
    }
}
