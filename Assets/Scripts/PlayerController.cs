using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float runMultiplier = 100;
    public float maxRunSpeed = 15;
    public float jumpStrength = 1000;

    Rigidbody2D rbody;
    GroundCheck groundCheck;


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
        if (jumpInput != 0 && groundCheck.isGrounded()) {
            rbody.AddForce(new Vector2(0, jumpStrength));
        }
    }
}
