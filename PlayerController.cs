using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //[NOTES]The level generation properties are all defined here and sorted so they they appear nicely in the unity editor
    //[NOTES]Note the use of serialized private fields, to allow for access and editing in the editor window for test purposes, while maintaining encapsulation of the variables.
    [Header("[PLAYER attributes]")]
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private int maxJumps = 3;

    private float moveInput;
    private bool facingLeft = true;
    private bool isGrounded;
    private int currentJumps;
    private Rigidbody2D rb;
    private LayerMask whatIsGround;
    private Transform groundCheck;

    //[NOTES]Initialize the container variables
    private void Start()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("PlayerBase");
        groundCheck = obj.transform;
        whatIsGround = LayerMask.GetMask("Ground");
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    //[NOTES]Input functions are handled in the Update, rigid body events are handled in the fixed update
    private void Update()
    {
        CheckJumpReset();
        GetMoveInput();
    }

    private void FixedUpdate()
    {
        CheckIfGrounded();
        Move();
    }

    //[NOTES]There's nothing too fancy here, but I wanted to highlight the focus on the Single-Responsibility design ideas here
    //[NOTES]So the get input could easily have all the move and jump logic embedded into it
    //[NOTES]But you can see, though they're simple code snippets, I've extracted and "function-ified" them
    private void GetMoveInput()
    {
        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown("space") && currentJumps >= 1)
        {
            Jump();
        }
        if (facingLeft == true && moveInput > 0)
        {
            Flip();
        }
        else if (facingLeft == false && moveInput < 0)
        {
            Flip();
        }
    }

    private void CheckJumpReset()
    {
        if (isGrounded == true)
        {
            currentJumps = maxJumps;
        }
    }

    private void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
        currentJumps--;
    }

    private void Flip()
    {
        facingLeft = !facingLeft;
        Vector3 Scaler = transform.localScale;
        Scaler.x *= -1;
        transform.localScale = Scaler;
    }

    private void CheckIfGrounded()
    {
        Vector2 floorDirection = new Vector2(0, -1);
        Debug.DrawRay(groundCheck.position, floorDirection * 0.2f, Color.red);
        isGrounded = Physics2D.Raycast(groundCheck.position, floorDirection, 0.2f, whatIsGround);
    }

    private void Move()
    {
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
    }
}
