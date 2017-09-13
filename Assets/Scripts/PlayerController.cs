using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float runMultiplier = 100;
    public float maxRunSpeed = 20;
    public float jumpStrength = 1000;

    Rigidbody2D rbody;
    GroundCheck groundCheck;
    bool canJump= false;


	// Use this for initialization
	void Start () {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        groundCheck = gameObject.GetComponentInChildren<GroundCheck>();
    }

    void FixedUpdate() {
        MoveHorizontal();
        Jump();
    }

    // Applies horizontal force to the rigidbody based on the horizontal input
    void MoveHorizontal() {
        float hInput = GameManager.InputHandler.horizontal;
        if (hInput != 0 && Mathf.Abs(rbody.velocity.x) < maxRunSpeed) {
            rbody.AddForce(new Vector2(runMultiplier * hInput, 0));
        }
    }
    
    // Applies vertical forces to the rigidbody based on whether jump input
    // has been detected and if the player is on the ground
    void Jump() {
        float jumpInput = GameManager.InputHandler.jump;

        // If input is not 1, the player released the button, they can jump
        if (jumpInput != 1) canJump = true;

        // Player can jump as long as they are grounded and weren't holding
        // down jump i.e. canJump is true
        if (jumpInput != 0 && groundCheck.isGrounded() && canJump) {
            rbody.AddForce(new Vector2(0, jumpStrength));
            canJump = false;
        }
    }
}
