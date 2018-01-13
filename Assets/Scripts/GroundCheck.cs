using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {
    public float groundCheckDistance = 1;

    Transform leftCheck;
    Transform rightCheck;

    void Start() {
        foreach (Transform t in transform) {
            if (t.name.Contains("Left")) {
                leftCheck = t;
            } else if (t.name.Contains("Right")) {
                rightCheck = t;
            }
        }
    }
    
    public bool isGrounded() {
        RaycastHit2D leftHit = Physics2D.Raycast(
            leftCheck.position,
            leftCheck.up * -1,
            groundCheckDistance,
            1 << LayerMask.NameToLayer("Solid"));
        RaycastHit2D rightHit = Physics2D.Raycast(
            rightCheck.position,
            rightCheck.up * -1,
            groundCheckDistance,
            1 << LayerMask.NameToLayer("Solid"));

        if (false) {
            Vector2 src, dest;

            src = leftCheck.position;
            dest = leftHit.collider != null
                ? leftHit.point
                : (Vector2)leftCheck.up * -groundCheckDistance;
            Debug.DrawLine(src, dest, Color.green, 0, false);

            src = rightCheck.position;
            dest = rightHit.collider != null
                ? rightHit.point
                : (Vector2)rightCheck.up * -groundCheckDistance;
            Debug.DrawLine(src, dest, Color.red, 0, false);
        }

        if (leftHit.collider != null || rightHit.collider != null) {
            return true;
        }
        else {
            return false;

        }
    }
}
