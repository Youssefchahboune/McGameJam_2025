using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float playerSpeed = 5f;
    private Rigidbody2D playerRb;
    public SpriteRenderer playerSprite;
    void Start()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
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
}
