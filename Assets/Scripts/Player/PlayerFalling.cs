using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFalling {

    float startTime = -1;

    Transform transform;
    float rotateSpeed;
    Rigidbody2D rbody;
    float ascendMultiplier;
    float descendMultiplier;

    public PlayerFalling (
        Rigidbody2D rbody, 
        Transform transform, 
        float rotateSpeed, 
        float ascendMultiplier, 
        float descendMultiplier) 
    {
        this.transform = transform;
        this.rotateSpeed = rotateSpeed;
        this.rbody = rbody;
        this.ascendMultiplier = ascendMultiplier;
        this.descendMultiplier = descendMultiplier;
    }

    public void RotateToUpRight () {
        Quaternion current = transform.rotation;
        Quaternion dest = Quaternion.Euler(0, 0, 0);
        startTime = startTime == -1 ? Time.time : startTime;
        float t = (Time.time - startTime) * rotateSpeed;
        transform.rotation = Quaternion.Slerp(current, dest, t);
    }

    public void setGrounded() {
        startTime = -1;
    }

    public void MultiplyGravity() {
        float force = 0;
        if (rbody.velocity.y < 0) force = Physics2D.gravity.y * (descendMultiplier - 1);
        else if (rbody.velocity.y > 0) force = Physics2D.gravity.y * (ascendMultiplier - 1);
        rbody.AddForce(new Vector2(0, force));
    }
}
