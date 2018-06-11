using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck {

    Transform trans;

    public PlayerGroundCheck (Transform trans) {
        this.trans = trans;
    }

    public bool IsGrounded (float sideOffset, float dist) {
        Vector2 down = trans.up * -1;

        Vector2 centerStart = trans.position;
        Vector2 rightStart = trans.position + (trans.right * sideOffset);
        Vector2 leftStart = trans.position + (-trans.right * sideOffset);
        
        int mask = 1 << LayerMask.NameToLayer("Solid");

        RaycastHit2D centerHit = Physics2D.Raycast(centerStart, down, dist, mask);
        RaycastHit2D leftHit = Physics2D.Raycast(leftStart, down, dist, mask);
        RaycastHit2D rightHit = Physics2D.Raycast(rightStart, down, dist, mask);
        
        if (false) {
            if (centerHit.collider != null)
                Utilities.DrawArrow(centerStart, centerHit.point, Color.cyan, 0);
            else Utilities.DrawArrow(centerStart, centerStart + (down * dist), Color.cyan, 0);

            if (leftHit.collider != null)
                Utilities.DrawArrow(leftStart, leftHit.point, Color.blue, 0);
            else Utilities.DrawArrow(leftStart, leftStart + (down * dist), Color.blue, 0);

            if (rightHit.collider != null)
                Utilities.DrawArrow(rightStart, rightHit.point, Color.yellow, 0);
            else Utilities.DrawArrow(rightStart, rightStart + (down * dist), Color.yellow, 0);
        }

        if (centerHit.collider != null || leftHit.collider != null || rightHit.collider != null) {
            return true;
        }

        return false;
    }
	
}
