using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float runMultiplier = 200;
    public float maxRunSpeed = 40;
    public float jumpStrength = 1000;
    public float parachuteDescentSpeed = 4.5f;
    public float parachuteTravelAcceleration = 1;
    public float parachuteTravelMaxSpeed = 20;
    public float stickyStartSpeed = 5;
    public float stickyScale = 30;
    public float stickyDistance = 3;
    public float gravityScale= 5;

    Vector3 spawnPoint;
    Rigidbody2D rbody;
    GroundCheck groundCheck;
    bool canJump = false;
    bool parachuteOpen;
    
    GameObject hat;
    CircleCollider2D plyCollider;
    
	// Use this for initialization
	void Start () {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        groundCheck = gameObject.GetComponentInChildren<GroundCheck>();
        spawnPoint = new Vector3(transform.position.x, 
                                 transform.position.y, 
                                 transform.position.z);

        hat = GameObject.Find("Hat");

        plyCollider = GameObject.Find("Collider").GetComponent<CircleCollider2D>();
    }

    void FixedUpdate() {
        
        // Reset the players rotation when they are in freefall
        if (!groundCheck.isGrounded())
            transform.eulerAngles = Vector2.zero;

        StickyRun();
        MoveHorizontal();
        Jump();
        ToggleParachute();
    }


    Vector2 GetSurfaceSlope(ArrayList list) {
        
        if (list.Count == 0) return Vector2.zero;

        ArrayList slopes = new ArrayList();
        for (int i = 0; i < list.Count - 1; i++) {
            for (int j = i + 1; j < list.Count; j++) {
                Vector2 p1 = (Vector2)list[i];
                Vector2 p2 = (Vector2)list[j];
                float x = p1.x - p2.x;
                float y = p1.y - p2.y;

                if (x < 0) {
                    x *= -1;
                    y *= -1;
                }

                Vector2 slope = new Vector2(x, y);
                slopes.Add(slope);
            }
        }

        Vector2 avg = new Vector2();
        foreach(Vector2 v in slopes) {
            avg += v;
        }
        avg /= slopes.Count;
        
        /*
        Vector2 normal = new Vector2(-avg.y, avg.x);
        Debug.DrawLine(
                Vector2.zero,
                normal,
                Color.cyan,
                1,
                false);
        */

        return avg.normalized;
    }

    /* Uses a Unity GameObject and raycast to find the nearby ground and 
     * rotate the player to be perpendicular to it
     */
    void StickyRun() {

        Vector2 n = new Vector2(0, 1);
        Vector2 ne = new Vector2(1,1);
        Vector2 e = new Vector2(1,0);
        Vector2 se = new Vector2(1,-1);
        Vector2 s = new Vector2(0,-1);
        Vector2 sw = new Vector2(-1,-1);
        Vector2 w = new Vector2(-1,0);
        Vector2 nw = new Vector2(-1,1);
        ArrayList pointsHit = new ArrayList();
        Vector3 src = plyCollider.transform.position;
        float maxDist = plyCollider.radius * stickyDistance;
        int layerMask = 1 << 8; // Only hit ground colliders
        int q1 = 0, // quadrant 1
            q2 = 0, // quadrant 2
            q3 = 0, // quadrant 3
            q4 = 0; // quadrant 4


        Vector3 dir = n; // The direction to fire the raycast
        RaycastHit2D hit = Physics2D.Raycast(src, dir, maxDist, layerMask);

        // Only collect the information from raycast that hit another collider

        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q1++;
            q2++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        dir = ne;
        hit = Physics2D.Raycast(src, dir, maxDist, layerMask);
        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q1++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        dir = e;
        hit = Physics2D.Raycast(src, dir, maxDist, layerMask);
        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q1++;
            q4++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        dir = se;
        hit = Physics2D.Raycast(src, dir, maxDist, layerMask);
        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q4++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        dir = s;
        hit = Physics2D.Raycast(src, dir, maxDist, layerMask);
        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q3++;
            q4++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        dir = sw;
        hit = Physics2D.Raycast(src, dir, maxDist, layerMask);
        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q3++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        dir = w;
        hit = Physics2D.Raycast(src, dir, maxDist, layerMask);
        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q2++;
            q3++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        dir = nw;
        hit = Physics2D.Raycast(src, dir, maxDist, layerMask);
        if (hit.collider != null) {
            pointsHit.Add(hit.point);
            q2++;
            Debug.DrawLine(src, hit.point, Color.red, 0, false);
        }

        
        
        Vector3 surfaceSlope = GetSurfaceSlope(pointsHit);
        // determine the most number of hits from any quadrant
        int max = 0;
        max = (q1 > max) ? q1 : max;
        max = (q2 > max) ? q2 : max;
        max = (q3 > max) ? q3 : max;
        max = (q4 > max) ? q4 : max;
        // Create a key that signifies the quardants involved with the most hits
        string key = "";
        key += (q1 == max) ? "1" : "";
        key += (q2 == max) ? "2" : "";
        key += (q3 == max) ? "3" : "";
        key += (q4 == max) ? "4" : "";
            
        /*
        if (key.CompareTo("3") == 0
            || key.CompareTo("34") == 0
            || key.CompareTo("4") == 0
            || key.CompareTo("234") == 0
            || key.CompareTo("134") == 0) {
            surfaceSlope *= 1;
        }
        */
        
        bool negativeSlope = surfaceSlope.y < 0;
        bool upsideDown = false;
        // Only work with a certain number of decimal places to avoid rounding errors
        string xTrue = surfaceSlope.x.ToString("n2");

        // Most hits resided in quadrant 1 and 4
        if (key.CompareTo("14") == 0) {
            // The ground is a vertical wall on right of player
            if (xTrue.CompareTo("0.00") == 0) {
                surfaceSlope = new Vector3(0, 1, 0);
            }
            // The ground is leading to a roof and player is below the ground i.e. --> \
            else if (negativeSlope) {
                surfaceSlope *= -1;
            }
        }
        // Most hits reside in quadrants 2 and 3
        else if (key.CompareTo("23") == 0) {
            // The ground is a vertical wall left of player
            if (xTrue.CompareTo("0.00") == 0) {
                surfaceSlope = new Vector3(0, -1, 0);
            }
            // The ground is leading to a roof and player is below the ground i.e. / <--
            else if ( ! negativeSlope ) {
                surfaceSlope *= -1;
            }
        }
        // The player is essentially upside down
        else if (key.CompareTo("1") == 0
            || key.CompareTo("12") == 0
            || key.CompareTo("2") == 0
            || key.CompareTo("124") == 0
            || key.CompareTo("123") == 0) 
        {
            // is the surface is almost perfectly horizontal, they are on the roof and 
            // rotate the player accordingly
            if (surfaceSlope.x >= 0.975f) {
                transform.eulerAngles = new Vector3();
                transform.Rotate(new Vector3(0, 0, 180));
                upsideDown = true;
            }
            // Otherwise, use the slope as if it were mirrored across the origin
            else {
                surfaceSlope *= -1;
            }
        }

        // Only apply when not completely upside down, otherwise Unity resets the
        // transforms up vector, this will also use the surface slope as is when none
        // of the cases above applu i.e. the ground is below the player
        if (!upsideDown) {
            transform.right = surfaceSlope;
        }

        // Determine the gravity vector to use when the player is running fast enough
        Vector2 runGravity = new Vector2(0, -9.8f * stickyScale);
        float playerSpeed = rbody.velocity.magnitude;
        // Apply sticky gravity if the player is traveling fast enough and they 
        // are on the ground
        if (playerSpeed > stickyStartSpeed && groundCheck.isGrounded()) {
            rbody.AddRelativeForce(runGravity);
        } else {
            rbody.AddForce(new Vector2(0, -9.8f * gravityScale));
        }
    }


    // Applies horizontal force to the rigidbody based on the horizontal input
    void MoveHorizontal() {
        float hInput = GameManager.InputHandler.getHorizontal();
        // Apply force to the rigidbody while the player is on the ground
        if (hInput != 0 
            && rbody.velocity.magnitude < maxRunSpeed
            && groundCheck.isGrounded()) 
        {
            rbody.AddRelativeForce(new Vector2(runMultiplier * hInput, 0));
        }
        // Apply force when the player is airborne
        else if (hInput != 0
            && rbody.velocity.magnitude < parachuteTravelMaxSpeed
            && !groundCheck.isGrounded()) 
        {
            rbody.AddRelativeForce(new Vector2(parachuteTravelAcceleration * hInput, 0));
        }
    }
    
    // Applies vertical forces to the rigidbody based on whether jump input
    // has been detected and if the player is on the ground
    void Jump() {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();

        // If input is not 1, the player released the button, they can jump
        if (!jumpPressed) canJump = true;

        // Player can jump as long as they are grounded and weren't holding
        // down jump i.e. canJump is true
        if (jumpPressed && groundCheck.isGrounded() && canJump) {
            rbody.AddRelativeForce(new Vector2(0, jumpStrength));
            canJump = false;
        }
    }

    // Toggle whether the parachute is open or not
    void ToggleParachute() {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();
        bool isVelocityDown = rbody.velocity.y < 0;
        bool playerFalling = !groundCheck.isGrounded();
        playerFalling = playerFalling && isVelocityDown;

        if (playerFalling && jumpPressed) {
            parachuteOpen = true;
            rbody.AddRelativeForce(new Vector3(0, 9.8f * parachuteDescentSpeed));
            rbody.transform.eulerAngles = new Vector3();
        } else {
            parachuteOpen = false;
        }

        showParachute(parachuteOpen);
    }

    // This will show the parachute when falling, animations should be 
    // setup and handled here. Temporary sprite for now.
    void showParachute(bool isShowing) {
        hat.SetActive(isShowing);
    }

    void Respawn() {
        transform.position.Set(spawnPoint.x, spawnPoint.y, spawnPoint.z);
    }
}
