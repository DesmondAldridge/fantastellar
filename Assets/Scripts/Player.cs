using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;



public class Player : MonoBehaviour
{
    // Config
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float jumpSpeed = 5f;
    [SerializeField] float climbSpeed = 5f;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    // State
    // bool isAlive = true;

    // Cached component references
    Rigidbody2D myRigidBody;
    Animator myAnimator;
    Collider2D myCollider2D;
    float gravityScaleAtStart;

    // Message then methods
    // Start is called before the first frame update
    void Start()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myCollider2D = GetComponent<Collider2D>();
        gravityScaleAtStart = myRigidBody.gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        Run();
        Jump();
        FlipSprite();
        ClimbLadder();

        if (myRigidBody.velocity.y < 0)
        {
            myRigidBody.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else if (myRigidBody.velocity.y > 0 && !Input.GetButton("Jump"))
        {
            myRigidBody.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        bool isGrounded = myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));


        if (isGrounded == true)
        {
            print("On the ground!");
            myAnimator.SetBool("Octavia - Jumping", false);
        } 

        bool inAir = !myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (inAir == true) 
        {
            print("In air!");
            myAnimator.SetBool("Octavia - Running", false);
            myAnimator.SetBool("Octavia - Jumping", true);
        }

    }

    private void Run()
    {
        float controlThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 to +1
        Vector2 playerVelocity = new Vector2(controlThrow * runSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
        print(playerVelocity);

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("Octavia - Running", playerHasHorizontalSpeed);
    }

    private void ClimbLadder()
    {
        if(!myCollider2D.IsTouchingLayers(LayerMask.GetMask("Climbing"))) 
        {
            myAnimator.SetBool("Climbing", false);
            myRigidBody.gravityScale = gravityScaleAtStart;
            return;
        }

        float controlThrow = CrossPlatformInputManager.GetAxis("Vertical");
        Vector2 climbVelocity = new Vector2(myRigidBody.velocity.x, controlThrow * climbSpeed);
        myRigidBody.velocity = climbVelocity;
        myRigidBody.gravityScale = 0f;

        bool playerHasVerticalSpeed = Mathf.Abs(myRigidBody.velocity.y) > Mathf.Epsilon;
        myAnimator.SetBool("Climbing", playerHasVerticalSpeed);
    }

    private void Jump()
    {
        if (!myCollider2D.IsTouchingLayers(LayerMask.GetMask("Ground"))){return;}

        if(CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocityToAdd = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocityToAdd;
            myAnimator.SetBool("Octavia - Jumping", true);
        }
    }



    private void FlipSprite()
    {
        // If the player is moving horizontally
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            // Reverse the current scaling of x axis
            transform.localScale = new Vector2 (Mathf.Sign(myRigidBody.velocity.x) * 1.5f, 1.5f);
        }

    }
}
