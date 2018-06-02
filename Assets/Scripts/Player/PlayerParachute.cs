using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParachute {

    bool open;

    Rigidbody2D rbody;
    float descentSpeed;
    CapController capCtrl;

    public PlayerParachute (Rigidbody2D rbody, float descentSpeed, CapController capCtrl) {
        this.rbody = rbody;
        this.descentSpeed = descentSpeed;
        this.capCtrl = capCtrl;
    }

    // Toggle whether the parachute is open or not
    public void Toggle(bool grounded) {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();
        bool isVelocityDown = rbody.velocity.y < 0;
        bool insideWindChannel = capCtrl.isInsideWind();

        open = false;

        if (isVelocityDown || insideWindChannel) {
            if (jumpPressed && !grounded) {
                open = true;
                rbody.AddRelativeForce(new Vector3(0, 9.8f * descentSpeed));
            }
        }
    }

    public bool isOpen() { return open; }

}
