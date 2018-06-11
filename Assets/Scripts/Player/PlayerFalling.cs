using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFalling {

    float startTime = -1;

    Transform transform;
    Rigidbody2D rbody;

    public PlayerFalling (Rigidbody2D rbody, Transform transform) {
        this.transform = transform;
        this.rbody = rbody;
    }

    public void RotateToUpRight (float rotateSpeed) {
        Quaternion current = transform.rotation;
        Quaternion dest = Quaternion.Euler(0, 0, 0);
        startTime = startTime == -1 ? Time.time : startTime;
        float t = (Time.time - startTime) * rotateSpeed;
        transform.rotation = Quaternion.Slerp(current, dest, t);
    }

    public void SetGrounded() {
        startTime = -1;
    }

    public void MultiplyGravity(float descendMultiplier, float ascendMultiplier) {
        float force = 0;
        if (rbody.velocity.y < 0) force = Physics2D.gravity.y * (descendMultiplier - 1);
        else if (rbody.velocity.y > 0) force = Physics2D.gravity.y * (ascendMultiplier - 1);
        rbody.AddForce(new Vector2(0, force));
    }
}
