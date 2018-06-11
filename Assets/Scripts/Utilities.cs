using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    public static void DrawArrow(Vector2 start, Vector2 end, Color color, float duration) {
        Debug.DrawLine(start, end, color, duration, false);

        Vector3 arrow = end - start;
        arrow *= -0.1f;

        Vector2 line1 = Rotate(arrow, 45) + end;
        Debug.DrawLine(end, line1, color, duration, false);

        Vector2 line2 = Rotate(arrow, -45) + end;
        Debug.DrawLine(end, line2, color, duration, false);
    }

    public static Vector2 Rotate(Vector2 vector, float angle) {

        float x, y, ca, sa;
        ca = Mathf.Cos(Mathf.Deg2Rad * angle);
        sa = Mathf.Sin(Mathf.Deg2Rad * angle);

        x = ca * vector.x - sa * vector.y;
        y = sa * vector.x + ca * vector.y;

        return new Vector2(x, y);
    }

    public static float GetSurfaceSpeed2D (Rigidbody2D rb) {
        if (rb.velocity.magnitude == 0) return 0;

        Transform tf = rb.transform;

        float result = 0;

        Vector3 start = tf.position;
        Vector3 up = tf.position + tf.up;
        Vector3 vel = tf.position + (Vector3)rb.velocity;

        float angle = Vector2.SignedAngle(tf.up, rb.velocity);
        if (angle > 90) angle -= 90;
        else if (angle < -90) angle += 90;
        else if (angle < 0) angle = -90 - angle;
        else if (angle > 0) angle = 90 - angle;
        else return 0;

        angle *= Mathf.Deg2Rad;
        result = Mathf.Cos(angle) * rb.velocity.magnitude;

        return result;
    }

}
