using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // movement variables
    public float playerSpeed = 5f;
    private Rigidbody2D playerRb;
    public SpriteRenderer playerSprite;

    // jump variables
    public float jumpForce = 5f;
    private bool isGrounded = true;
    private bool isJumping = false;
    private float jumpTimeCounter;
    public float jumpTime;
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;

    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, checkRadius, whatIsGround);
        Jump();
        Move();
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
}
