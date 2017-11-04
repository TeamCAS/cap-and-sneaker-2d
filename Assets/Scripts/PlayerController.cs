using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float runMultiplier = 100;
    public float maxRunSpeed = 20;
    public float jumpStrength = 1000;
    public float parachuteSpeed = 1;


    Vector3 spawnPoint;
    Rigidbody2D rbody;
    GroundCheck groundCheck;
    bool canJump = false;
    bool parachuteOpen;
    float originalGravityScale;

    GameObject hat;

	// Use this for initialization
	void Start () {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        groundCheck = gameObject.GetComponentInChildren<GroundCheck>();
        spawnPoint = new Vector3(transform.position.x, 
                                 transform.position.y, 
                                 transform.position.z);

        hat = GameObject.Find("Hat");
        originalGravityScale = rbody.gravityScale;
    }

    void FixedUpdate() {
        MoveHorizontal();
        Jump();
        ToggleParachute();
    }

    // Applies horizontal force to the rigidbody based on the horizontal input
    void MoveHorizontal() {
        float hInput = GameManager.InputHandler.getHorizontal();
        if (hInput != 0 && Mathf.Abs(rbody.velocity.x) < maxRunSpeed) {
            rbody.AddForce(new Vector2(runMultiplier * hInput, 0));
        }
    }
    
    // Applies vertical forces to the rigidbody based on whether jump input
    // has been detected and if the player is on the ground
    void Jump() {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();

        // If input is not 1, the player released the button, they can jump
        if (!jumpPressed) canJump = true;

        // Player can jump as long as they are grounded and weren't holding
        // down jump i.e. canJump is true
        if (jumpPressed && groundCheck.isGrounded() && canJump) {
            rbody.AddForce(new Vector2(0, jumpStrength));
            canJump = false;
        }
    }

    // Toggle whether the parachute is open or not
    void ToggleParachute() {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();
        bool isVelocityDown = rbody.velocity.y < 0;
        bool playerFalling = !groundCheck.isGrounded();
        playerFalling = playerFalling && isVelocityDown;

        if (playerFalling && jumpPressed) {
            parachuteOpen = true;
            rbody.gravityScale = parachuteSpeed;
        } else {
            parachuteOpen = false;
            rbody.gravityScale = originalGravityScale;
        }

        showParachute(parachuteOpen);
    }

    // This will show the parachute when falling, animations should be 
    // setup and handled here. Temporary sprite for now.
    void showParachute(bool isShowing) {
        hat.SetActive(isShowing);
    }

    void Respawn() {
        transform.position.Set(spawnPoint.x, spawnPoint.y, spawnPoint.z);
    }
}
