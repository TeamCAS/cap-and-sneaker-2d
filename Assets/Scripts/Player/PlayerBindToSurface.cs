using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBindToSurface {

    float distToSurface;
    GameObject temp;
    Vector3 prevPosition;

    // Number of intervals used to interpolate distance across surface.
    // The more intervals used, the more accurate the repositioning of
    // the player is at the cost of computation time. 
    Transform transform;
    int intervalCount = 2;
    Rigidbody2D rbody;
    float bindDist;

    public PlayerBindToSurface(Transform transform, int intervalCount, Rigidbody2D rbody, float bindDist) {
        this.transform = transform;
        this.intervalCount = intervalCount;
        this.rbody = rbody;
        this.bindDist = bindDist;

        prevPosition = transform.position;

        temp = new GameObject();
    }

    bool CheckGround () {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            transform.up * -1,
            bindDist,
            1 << LayerMask.NameToLayer("Solid"));

        if (hit.collider == null) prevPosition = transform.position;

        return hit.collider != null;
    }

    void UpdateVelocity(float surfaceSpeed) {
        Vector3 right = Rotate(transform.up, -90);
        Vector3 left = Rotate(transform.up, 90);
        float distToRight = Vector3.Distance(right, rbody.velocity);
        float distToLeft = Vector3.Distance(left, rbody.velocity);

        float sign = 0;
        if (distToRight < distToLeft) sign = 1;
        else if (distToRight > distToLeft) sign = -1;

        Vector2 vel = sign * right;
        //vel *= rbody.velocity.magnitude;
        vel *= surfaceSpeed;
        rbody.velocity = vel;
    }
    
    void UpdateStance () {
        temp.transform.position = prevPosition;

        float dist = Vector3.Distance(transform.position, prevPosition);
        float intervalDist = dist / intervalCount;

        Vector3 right = Rotate(temp.transform.up, -90);
        Vector3 left = Rotate(temp.transform.up, 90);
        float distToRight = Vector3.Distance(right, rbody.velocity);
        float distToLeft = Vector3.Distance(left, rbody.velocity);

        float sign = 0;
        if (distToRight < distToLeft) sign = 1;
        else if (distToRight > distToLeft) sign = -1;

        for (int i = 0; i < intervalCount; i++) {
            right = Rotate(temp.transform.up, -90);
            temp.transform.position += sign * right * intervalDist;

            Vector3 start = temp.transform.position;
            Vector3 end = start + (temp.transform.up * -1 * bindDist);
            DrawArrow(start, end, Color.blue, 1);

            RaycastHit2D hit = Physics2D.Raycast(
                temp.transform.position,
                temp.transform.up * -1,
                bindDist,
                1 << LayerMask.NameToLayer("Solid"));

            if (hit.collider == null) Debug.Log("Collider is NULL");
            else {
                DrawArrow(start, hit.point, Color.cyan, 1);

                temp.transform.position = hit.point;
                temp.transform.up = hit.normal;
                temp.transform.position += temp.transform.up * distToSurface;
            }
        }

        transform.up = temp.transform.up;

        prevPosition = transform.position;
    }

    public void AdjustToSurface () {
        if (!CheckGround()) return;
        UpdateStance();
    }

    public void BindToSurface (float surfaceSpeed) {
        if (!CheckGround()) return;
        UpdateStance();
        UpdateVelocity(surfaceSpeed);
        transform.position = temp.transform.position;
    }

    public void SetDistanceToSurface() {
        RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                transform.up * -1,
                10,
                1 << LayerMask.NameToLayer("Solid"));
        distToSurface = hit.distance;
    }

    void DrawArrow(Vector2 start, Vector2 end, Color color, float duration) {
        Debug.DrawLine( start, end, color, duration, false );

        Vector3 arrow = end - start;
        arrow *= -0.1f;

        Vector2 line1 = Rotate(arrow, 45) + end;
        Debug.DrawLine( end, line1, color, duration, false );

        Vector2 line2 = Rotate(arrow, -45) + end;
        Debug.DrawLine( end, line2, color, duration, false );
    }

    Vector2 Rotate(Vector2 vector, float angle) {

        float x, y, ca, sa;
        ca = Mathf.Cos(Mathf.Deg2Rad * angle);
        sa = Mathf.Sin(Mathf.Deg2Rad * angle);

        x = ca * vector.x - sa * vector.y;
        y = sa * vector.x + ca * vector.y;

        return new Vector2(x, y);
    }
}
