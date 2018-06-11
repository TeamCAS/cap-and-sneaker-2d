using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove {

    float groundAccel;
    float groundMaxSpeed;
    float airAccel;
    float airMaxSpeed;
    float airDrag;

    public PlayerMove (float groundAccel, float groundMaxSpeed, float airAccel, float airMaxSpeed, float airDrag) {
        this.groundAccel = groundAccel;
        this.groundMaxSpeed = groundMaxSpeed;
        this.airAccel = airAccel;
        this.airMaxSpeed = airMaxSpeed;
        this.airDrag = airDrag;
    }

    // Apply force to the rigidbody while the player is on the ground
    public Vector2 GroundMove (Vector2 velocity, float surfaceSpeed) {
        float hInput = GameManager.InputHandler.getHorizontal();
        if (hInput != 0 && surfaceSpeed <= groundMaxSpeed) {
            velocity += new Vector2(groundAccel * hInput, 0);
        }
        velocity = new Vector2(velocity.x * airDrag, velocity.y);
        return velocity;
    }
    
    // Apply force when the player is airborne
    public Vector2 AirMove (Vector2 velocity, float speed) {
        float hInput = GameManager.InputHandler.getHorizontal();
        if (hInput != 0 && speed <= airMaxSpeed) {
            velocity += new Vector2(airAccel * hInput, 0);
        }
        velocity = new Vector2(velocity.x * airDrag, velocity.y);
        return velocity;
    }
}
