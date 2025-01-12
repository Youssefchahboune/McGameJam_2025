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
    private AudioSource footsteps;

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

    // Attack Variables
    public float slashCoolDown = 0.5f;
    private bool canAttack = true;
    public float slashHideTime = 0.2f;
    public float playerSpeedWhileAttackingOnGround = 0f;
    public float playerSpeedWhileAttackingInTheAir = 5f;
    private bool isAttacking = false;

    // swoosh
    public GameObject swooshGameObject;
    public float delayBeforeShowingSwoosh = 1f;
    public GameObject swooshPos;
    public GameObject swooshPos2;

    private void Awake()
    {
        dashAfterEffectParticles.Stop();
    }

    void Start()
    {
        footsteps = GetComponent<AudioSource>();
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
        attack();
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !Input.GetKeyDown(KeyCode.D) && !Input.GetKeyDown(KeyCode.A) && !Input.GetKeyDown(KeyCode.Space))
        {
            footsteps.Stop();
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
            if (!isDashing && !isAttacking)
            {
                playerSprite.flipX = true;
            }

            if (isGrounded && !isJumping && !isAttacking)
            {
            
                animator.SetBool("isRunning", true);
                if (!footsteps.isPlaying)
                {
                    footsteps.Play();
                }

            }

            playerRb.velocity = new Vector2(-playerSpeed, playerRb.velocity.y);

        }
        else if (Input.GetKey(KeyCode.D))
        {
            if (!isDashing && !isAttacking)
            {
                playerSprite.flipX = false;
            }

            if (isGrounded && !isJumping && !isAttacking)
            {

                animator.SetBool("isRunning", true);
                if (!footsteps.isPlaying)
                {
                    footsteps.Play();
                }

            }
            
            playerRb.velocity = new Vector2(playerSpeed, playerRb.velocity.y);
            

        }
        else
        {
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            animator.SetBool("isRunning", false);
            footsteps.Stop();
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
            footsteps.Stop();
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
        animator.SetBool("isJumping", false);
        
        playerRb.gravityScale = 0f;

        yield return new WaitForSeconds(timeBeforeDashing);
        animator.SetBool("isDashing", true);
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
        animator.SetBool("isDashing", false);
        playerRb.gravityScale = originalPlayerGravity;
        dashAfterEffectParticles.Stop();
        isDashing = false;
        

        yield return new WaitForSeconds(dashcoolDown);
        canDash = true;
    }

    public void attack()
    {
        if (isDashing)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && canAttack)
        {
            canAttack = false;
            animator.SetBool("isRunning", false);
            footsteps.Stop();
            animator.SetBool("isJumping", false);
            isAttacking = true;
            isJumping = false;
            if (isGrounded)
            {
                playerSpeed = playerSpeedWhileAttackingOnGround;
            }
            else if (!isGrounded)
            {
                playerSpeed = playerSpeedWhileAttackingInTheAir;
            }

            animator.SetBool("isAttacking", true);

            if (PlayerFacing() == "RIGHT")
            {
                swooshGameObject.gameObject.transform.position = swooshPos.transform.position;
                swooshGameObject.GetComponent<SpriteRenderer>().flipX = false;
                swooshGameObject.GetComponent<Collider2D>().offset = new Vector2(Mathf.Abs(swooshGameObject.GetComponent<Collider2D>().offset.x), swooshGameObject.GetComponent<Collider2D>().offset.y);
            }
            else if (PlayerFacing() == "LEFT")
            {
                swooshGameObject.gameObject.transform.position = swooshPos2.transform.position;
                swooshGameObject.GetComponent<SpriteRenderer>().flipX = true;
                swooshGameObject.GetComponent<Collider2D>().offset = new Vector2(-Mathf.Abs(swooshGameObject.GetComponent<Collider2D>().offset.x), swooshGameObject.GetComponent<Collider2D>().offset.y);

            }
            StartCoroutine(attackDelayLeftRight());
        }
               
        //Special Attack Throw Gear
        /*else if ((Input.GetMouseButtonDown(1) || inputActions.Gameplay.SpecialAttack.WasPressedThisFrame()) && canAttack && hasScythe)
        {
            isJumping = false;
            specialAttack_DeathBoomerang();
        }*/


    }

    private IEnumerator attackDelayLeftRight()
    {
        yield return new WaitForSeconds(delayBeforeShowingSwoosh);
        swooshGameObject.SetActive(true);

        yield return new WaitForSeconds(slashHideTime);
        swooshGameObject.SetActive(false);
        isAttacking = false;
        playerSpeed = originalPlayerSpeed;
        animator.SetBool("isAttacking", false);


        yield return new WaitForSeconds(slashCoolDown);
        // Allow attacks again
        canAttack = true;
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
