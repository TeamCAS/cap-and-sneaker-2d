using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {
    public float groundCheckDistance = 1.2f;
    public float horizontalDistance = 1;

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

    public bool solidToLeft() {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right * -1,
            horizontalDistance,
            1 << LayerMask.NameToLayer("Solid"));

        Vector3 dest = hit.collider != null
            ? (Vector3)hit.point
            : Vector3.right * -1 * horizontalDistance + transform.position;

        //Debug.DrawLine( transform.position, dest, Color.green, 0, false);

        return hit.collider != null;
    }

    public bool solidToRight() {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.right,
            horizontalDistance,
            1 << LayerMask.NameToLayer("Solid"));

        Vector3 dest = hit.collider != null
            ? (Vector3)hit.point
            : Vector3.right * horizontalDistance + transform.position;

        //Debug.DrawLine( transform.position, dest, Color.red, 0, false);

        return hit.collider != null;
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
            Vector3 src, dest;

            src = leftCheck.position;
            dest = leftHit.collider != null
                ? (Vector3)leftHit.point
                : leftCheck.up * -groundCheckDistance + leftCheck.position;
            Debug.DrawLine(src, dest, Color.green, 0, false);

            src = rightCheck.position;
            dest = rightHit.collider != null
                ? (Vector3)rightHit.point
                : rightCheck.up * -groundCheckDistance + rightCheck.position;
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
