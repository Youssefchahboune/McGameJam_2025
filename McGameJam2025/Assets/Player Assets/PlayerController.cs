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
        if (Input.GetKey(KeyCode.A))
        {
            playerSprite.flipX = true;
            playerRb.velocity = new Vector2(-playerSpeed, playerRb.velocity.y);

        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerSprite.flipX = false;
            playerRb.velocity = new Vector2(playerSpeed, playerRb.velocity.y);

        }
        else
        {
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
        }
    }

    public void Jump()
    {
        if (isGrounded && (Input.GetKeyDown(KeyCode.Space) && !Input.GetKeyDown(KeyCode.LeftShift)))
        {
            isJumping = true;
            isGrounded = false;
            jumpTimeCounter = jumpTime;
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
    }

    public IEnumerator Dash()
    {
        playerRb.velocity = Vector2.zero;
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
