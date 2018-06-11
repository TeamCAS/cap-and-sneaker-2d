using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump {

    bool canJump;

    Rigidbody2D rbody;
    float jumpStrength;

    public PlayerJump (Rigidbody2D rbody, float jumpStrength) {
        this.rbody = rbody;
        this.jumpStrength = jumpStrength;
    }

    // Applies vertical forces to the rigidbody based on whether jump input
    // has been detected and if the player is on the ground
    public bool Jump(Rigidbody2D rbody, float jumpStrength) {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();

        // If input is not 1, the player released the button, they can jump
        if (!jumpPressed) canJump = true;

        // Player can jump as long as they weren't holding
        // down jump i.e. canJump is true
        if (jumpPressed && canJump) {
            rbody.AddRelativeForce(new Vector2(0, jumpStrength));
            canJump = false;
            return true;
        }
        return false;
    }
}
