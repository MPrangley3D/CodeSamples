using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.SceneManagement;

//[NOTES]This is the player class for my 2d platformer demo
//[NOTES]Some general highlights in this project: 
//[NOTES] -Use of interfaces for damageable entities
//[NOTES] -Use of precompile directives to make cross development testing simpler between the unity editor and the mobile device
//[NOTES] -Use of coroutines to manage cooldowns and wait events
public class Player : MonoBehaviour, IDamageable
{
    //[NOTES]You can see the implementation of unity editor decorating to make the editor view more comprehensible
    [Header("PLAYER SETTINGS")]
    [SerializeField]
    private float jumpforce = 4.0f;
    [SerializeField]
    private float playerJumpHeight = 1.5f;
    [SerializeField]
    private LayerMask groundLayer;
    [SerializeField]
    private bool jumpCooldown = false;
    [SerializeField]
    private float playerSpeed = 3.0f;


    //these all get initialized in the Start method
    private Rigidbody2D rigid;
    private PlayerAnimation playerAnim;
    private Animator anim;
    private SpriteRenderer playerSprite;
    private SpriteRenderer effectsSprite;

    public int Health { get; set; }
    private bool grounded = false;
    private bool dead = false;
    private float moveInput;
    private bool jumpInput = false;
    
    //Initialize some handles to different components and set player HP
    private void Start()
    {
        Health = 4;
        rigid = GetComponent<Rigidbody2D>();
        playerAnim = GetComponent<PlayerAnimation>();
        anim = GetComponentInChildren<Animator>();
        playerSprite = transform.GetChild(0).GetComponent<SpriteRenderer>();
        effectsSprite = transform.GetChild(1).GetComponent<SpriteRenderer>();
    }

    //[NOTES]The precompile directives make this a little more convoluted to read, but they make cross-development a breeze
    private void Update()
    {
        if (dead == false)
        {
            grounded = IsGrounded();
            GetPlayerInput();
        }
    }

    //[NOTES]Physics events are handled in the fixed update
    private void FixedUpdate()
    {
        UpdateMove();
        UpdateJump();
    }

    //[NOTES]Raycast used to detect collision with ground
    private bool IsGrounded()
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, 0.9f, groundLayer.value);
        Debug.DrawRay(transform.position, Vector2.down * 0.9f, Color.red);
        if (hitInfo.collider != null && jumpCooldown == false)
        {
            playerAnim.Jump(false);
            return true;
        }
        return false;
    }

    //[NOTES] The Attack input doesnt need a return value, because there are no physics implications
    //[NOTES] Move input & jump input get stored for access in the fixed update
    private void GetPlayerInput()
    {
        AttackInput();
        moveInput = MovementInput();
        jumpInput = JumpInput();
    }

    //[NOTES]All the collision logic for the attack is handled within the animation via embedded functions
    //[NOTES]So this attack trigger is all that is needed to manage the logic for harming enemies in this script
    private void AttackInput()
    {
        #if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        if (Input.GetMouseButtonDown(0) && grounded == true)
        {
            playerAnim.Attack();
        }
        #else
        if (CrossPlatformInputManager.GetButtonDown("A_Btn") && _grounded == true)
        {
            playerAnim.Attack();
        }
        #endif
    }

    //[NOTES]The move float is returned, so that rigid body physics can be handled in fixed update rather than update.
    private float MovementInput()
    {
        //Walk
        #if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        float move = Input.GetAxisRaw("Horizontal");
        #else
        float move = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        #endif

        //Flip Axis
        Flip(move);
        playerAnim.Move(move);
        return move;
    }

    //[NOTES]the jump bool is returned for processing in the fixed update, due to the rigidbody component.
    private bool JumpInput()
    {
        //Jump
        #if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space) && grounded == true)
        #else
        if (CrossPlatformInputManager.GetButtonDown("B_Btn") && _grounded == true)
        #endif
        {
            return true;
        }
        return false;
    }

    private void UpdateMove()
    {
        rigid.velocity = new Vector2(moveInput * playerSpeed, rigid.velocity.y);
    }

    //[NOTES]If the input for jump has been triggered, the player jumps - notive there is a check to the game maanger for the "Flying boots" powerup
    //[NOTES]I use a coroutine to manage the jump cooldown, and additionally the ability to trigger a jump is blocked until the 'isGrounded' is true
    private void UpdateJump()
    {
        if (jumpInput == true)
        {
            Debug.Log("jump");
            if (GameManager.Instance.hasBoots == true)
                rigid.velocity = new Vector2(rigid.velocity.x, jumpforce + 3 * playerJumpHeight + 2);
            else
                rigid.velocity = new Vector2(rigid.velocity.x, jumpforce * playerJumpHeight);
            StartCoroutine(JumpCooldown());
            playerAnim.Jump(true);
            jumpInput = false;
        }
    }

    //Flips the player based on inout direction
    private void Flip(float move)
    {
        Vector3 newLocalScale = transform.localScale;
        var size = Mathf.Abs(transform.localScale.x);

        if (move > 0)
        {
            newLocalScale.x = size;
        }

        else if (move < 0)
        {
            newLocalScale.x = -size;
        }

        transform.localScale = newLocalScale;
    }

    //[NOTES]Very simply damages the player until they are out of health, then triggers the reset coroutine
    //[NOTES]Notable is the use of the UIManager class to control the player HP on the UI level
    public void Damage()
    {
        if (dead == false)
        {
            Health--;
            Debug.Log("Current HP: " + Health);
            UIManager.Instance.UpdateLife(Health);
            if (Health < 0)
            {
                dead = true;
                rigid.velocity = new Vector2(0, 0);
                anim.SetTrigger("Death");
                StartCoroutine(Restart());
            }
        }
    }

    //Jump cooldown
    IEnumerator JumpCooldown()
    {
        jumpCooldown = true;
        yield return new WaitForSeconds(0.1f);
        jumpCooldown = false;
    }

    //Restarts after a delay
    IEnumerator Restart()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Game");
    }
}