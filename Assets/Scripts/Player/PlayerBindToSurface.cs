using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBindToSurface {

    float distToSurface;

    Transform transform;
    Rigidbody2D rbody;
    GameObject temp;
    
    public PlayerBindToSurface(Transform transform, Rigidbody2D rbody) {
        this.transform = transform;
        this.rbody = rbody;
        temp = new GameObject();
    }


    Vector2 prevPos;
    Quaternion prevRot;

    public void Reposition (float bindIntervals, float bindDistance, float bindSpeed, bool jumped) {
        temp.transform.position = prevPos;
        temp.transform.rotation = prevRot;
        float prevSurfaceSpeed = Utilities.GetSurfaceSpeed2D(rbody);

        float dist = Vector2.Distance(transform.position, prevPos);
        float intervalDist = dist / bindIntervals;

        // Determine which direction player is traveling based on
        // stance and distance to the direction of their velocity
        Vector3 right = Utilities.Rotate(temp.transform.up, -90);
        Vector3 left = Utilities.Rotate(temp.transform.up, 90);
        float distToRight = Vector3.Distance(right, rbody.velocity);
        float distToLeft = Vector3.Distance(left, rbody.velocity);
        // A sign of -1 means player is traveling left with respect
        // to the players stance, a sign of 1 is to the right
        float sign = 0;
        if (distToRight < distToLeft) sign = 1;
        else if (distToRight > distToLeft) sign = -1;

        bool endOnBind = false;
        Vector2 vel = rbody.velocity;

        for (int i = 0; i < bindIntervals; i++) {
            right = Utilities.Rotate(temp.transform.up, -90);
            temp.transform.position += sign * right * intervalDist;


            Utilities.DrawArrow(
                temp.transform.position,
                temp.transform.position + (temp.transform.up * -1 * bindDistance),
                Color.cyan,
                0);


            RaycastHit2D hit = Physics2D.Raycast(
                temp.transform.position,
                temp.transform.up * -1,
                bindDistance,
                1 << LayerMask.NameToLayer("Solid"));

            Vector3 velDir = vel.normalized;

            Utilities.DrawArrow(transform.position, temp.transform.position, Color.magenta, 0);

            // If did not bind, simply follow direction of velocity
            if (hit.collider == null || jumped) {
                temp.transform.position += intervalDist * velDir;
                endOnBind = false;
            }
            // If binding, update position and rotation as well as
            // velocity just in case bind doesnt happen during next
            // loop i.e. player disconnects because of ramp
            else {
                Utilities.DrawArrow(temp.transform.position, hit.point, Color.green, 0);
                Vector2 start = transform.position;
                Vector2 end = start + vel.normalized;
                Utilities.DrawArrow(start, end, Color.red, 0);

                if (prevSurfaceSpeed >= bindSpeed) {
                    temp.transform.position = hit.point;
                    temp.transform.up = hit.normal;
                    temp.transform.position += temp.transform.up * distToSurface;

                    vel = GetUpdatedVelocity2D(temp.transform, vel);
                }
                else {
                    temp.transform.position += intervalDist * velDir;
                }

                endOnBind = true;

                start = transform.position;
                end = start + vel.normalized;
                Utilities.DrawArrow(start, end, Color.yellow, 0);
            }
        }

        if (!endOnBind) {
            temp.transform.up = transform.up;
        }

        // Rotate player to normal
        float angle = Vector3.Angle(temp.transform.up, Vector3.up);
        transform.rotation = new Quaternion();

        if (temp.transform.up.x > 0) {
            transform.Rotate(new Vector3(0, 0, 360 - angle));
        }
        else {
            transform.Rotate(new Vector3(0, 0, angle));
        }

        if (prevSurfaceSpeed >= bindSpeed) transform.position = temp.transform.position;

        rbody.velocity = vel;

        prevPos = transform.position;
        prevRot = transform.rotation;
    }

    Vector2 GetUpdatedVelocity2D (Transform tf, Vector2 velocity) {
        Vector3 right = Utilities.Rotate(tf.up, -90);
        Vector3 left = Utilities.Rotate(tf.up, 90);
        float distToRight = Vector3.Distance(right, velocity);
        float distToLeft = Vector3.Distance(left, velocity);

        float sign = 0;
        if (distToRight < distToLeft) sign = 1;
        else if (distToRight > distToLeft) sign = -1;

        Vector2 vel = sign * right;
        return vel.normalized * velocity.magnitude;
    }
    
    public void SetDistanceToSurface() {
        RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                transform.up * -1,
                2,
                1 << LayerMask.NameToLayer("Solid"));
        distToSurface = hit.distance;
    }

}
