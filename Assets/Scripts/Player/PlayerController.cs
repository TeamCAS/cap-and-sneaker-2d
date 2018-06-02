using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Read Only")]
    public float speed;
    public float surfaceSpeed;
    public float speedAfter;


    [Header("Max speed when on a flat horizontal surface")]
    public float maxRunSpeed = 30;
    [Header("Acceleration before binding to surface")]
    public float runMultiplier = 30;
    [Header("Bind to surface min speed")]
    public float bindSpeed = 20;
    [Header("Bind to surface min distance, used when player is not grounded")]
    public float bindDistance = 1;

    public float runAttackMinSpeed = 36;


    public float jumpStrength = 1000;
    public float parachuteDescentSpeed = 4.5f;
    public float parachuteTravelAcceleration = 1;
    public float parachuteTravelMaxSpeed = 20;
    public float fallingRotationSpeed = 0.1f;
    public float ascendMultiplier = 2;
    public float descendMultiplier = 3;

    public float playerWidth = 1;
    public float playerHeight = 2.5f;

    public float leftCheckDist = 0.1f;

    [Header("How far the player gets pushed when hit")]
    public float hitPushStrength = 1;
    [Header("Duration in seconds the player has no control when damaged")]
    public float damagedDuration = 1;
    [Header("The speed of the player when pulled towards the latched cap")]
    public float latchedCapSpeed = 1;

    Vector3 spawnPoint;
    Rigidbody2D rbody;
    GroundCheck groundCheck;
    bool canJump = false;
    bool interacting = false;

    float rotationStartTime = -1;

    GameObject hat;
    CircleCollider2D plyCollider;
    CapController capCtrl;
    Vector3 prevPosition;

    GameObject leftGroundCheck;
    GameObject rightGroundCheck;
    GameObject centerGroundCheck;
    float floorMode = -1;

    AnimationHandler animations;
    bool grounded;

    Vector3 prevCheckPos;
    float gspd;

    bool prevGrounded = false;

    Vector3 cloneVector3(Vector3 orig) {
        return new Vector3(orig.x, orig.y, orig.z);
    }

    //Indicators for debugging
    GameObject groundedSignal;


    float hitTimerStart;
    bool runAttackActive = false;
    FistStrike fistStrike;
    FeetStrike feetStrike;
    bool capLatched;
    public float orbDropVelocity = 1;

    // Keeps the BTS off for short duration so that other forces
    // can apply i.e. jump
    float btsTimeOff = 0.1f;
    float btsTimer;

    PlayerJump pJump;
    PlayerDamaged pDamaged;
    PlayerFalling pFalling;
    PlayerParachute pParachute;
    PlayerBindToSurface pBts;



	// Use this for initialization
	void Start () {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        groundCheck = gameObject.GetComponentInChildren<GroundCheck>();
        spawnPoint = cloneVector3(transform.position);

        hat = GameObject.Find("Hat");
        //plyCollider = GameObject.Find("Collider").GetComponent<CircleCollider2D>();
        capCtrl = gameObject.GetComponentInChildren<CapController>();
        leftGroundCheck = GameObject.Find("PlayerLeftGroundCheck");
        rightGroundCheck = GameObject.Find("PlayerRightGroundCheck");
        centerGroundCheck = GameObject.Find("PlayerCenterGroundCheck");

        prevPosition = cloneVector3(transform.position);
        animations = gameObject.GetComponentInChildren<AnimationHandler>();

        groundedSignal = GameObject.Find("GroundedSignal");

        foreach (Transform t in transform) {
            if (t.name == "FistStrike") fistStrike = t.GetComponent<FistStrike>();
            if (t.name == "FeetStrike") feetStrike = t.GetComponent<FeetStrike>();
        }

        pJump = new PlayerJump();
        pDamaged = new PlayerDamaged(hitPushStrength, rbody, orbDropVelocity, damagedDuration);
        pFalling = new PlayerFalling(rbody, transform, fallingRotationSpeed, ascendMultiplier, descendMultiplier);
        pParachute = new PlayerParachute(rbody, parachuteDescentSpeed, capCtrl);
        pBts = new PlayerBindToSurface(transform, 4, rbody, bindDistance);
    }

    void Update() {
        grounded = groundCheck.isGrounded();

        animations.UpdateParamaters(
            grounded,
            rbody.velocity,
            pParachute.isOpen(), 
            pDamaged.isDamaged(), 
            runAttackActive, 
            capLatched
        );
    }

    float GetSurfaceSpeed() {
        if (rbody.velocity.magnitude == 0) return 0;

        float result = 0;

        float angle = Vector2.SignedAngle(transform.up, rbody.velocity);
        if (angle > 90) angle -= 90;
        else if (angle < -90) angle += 90;
        else if (angle < 0) angle = -90 - angle;
        else if (angle > 0) angle = 90 - angle;
        else return 0;

        angle *= Mathf.Deg2Rad;
        result = Mathf.Cos(angle) * rbody.velocity.magnitude;
        
        return result;
    }

    void FixedUpdate() {
        speed = rbody.velocity.magnitude;
        grounded = groundCheck.isGrounded();
        surfaceSpeed = GetSurfaceSpeed();

        // If player is damaged, no need to do anything, player has no control
        if (pDamaged.isDamaged()) return;

        // return if player is interacting
        if (interacting) return;

        if (surfaceSpeed > bindSpeed && Time.time - btsTimer > btsTimeOff) {
            pBts.BindToSurface(surfaceSpeed);
            Debug.Log("Binding to surface");
        }
        else {
            pBts.AdjustToSurface();
            Debug.Log("Adjusting to surface");
        }

        if (grounded) {

            if (speed == 0) pBts.SetDistanceToSurface();

            // Activate player run attack if traveling fast enough.
            runAttackActive = false;
            if (speed >= runAttackMinSpeed) runAttackActive = true;

            bool jumped = pJump.Jump(rbody, jumpStrength);
            if (jumped) btsTimer = Time.time;

            pFalling.setGrounded();

        }
        else {

            pFalling.RotateToUpRight();
            pFalling.MultiplyGravity();

        }

        groundedSignal.SetActive(!grounded);

        fistStrike.SetActive(runAttackActive);
        feetStrike.SetActive(capLatched);
        if (capLatched) return;

        bool leftSolid = groundCheck.solidToLeft();
        bool rightSolid = groundCheck.solidToRight();
        if (leftSolid && rbody.velocity.x < 0 || rightSolid && rbody.velocity.x > 0) {
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }
        
        MoveHorizontal();
        pParachute.Toggle(grounded);

        speedAfter = rbody.velocity.magnitude;
        prevGrounded = grounded;
    }

    void UndoSolidPassThrough() {
        // shoot ray cast from old to new
        Vector3 dir = transform.position - prevPosition;
        float maxDist = dir.magnitude;
        int layerMask = 1 << LayerMask.NameToLayer("Solid");
        RaycastHit2D hit = Physics2D.Raycast(
            prevPosition, 
            dir, 
            maxDist, 
            layerMask,
            -Mathf.Infinity,
            Mathf.Infinity);

        // check for collisions
        if (hit.collider != null) {
            print("WARNING: Player passed through solid! @ " + hit.collider.gameObject.name);
            Debug.Break();
        }

        // reposition to collision if exist

        // save position for repositioning in next loop
        prevPosition = cloneVector3(transform.position);
    }

    Vector3 RotateToSurfaceNormal() {

        RaycastHit2D centerHit = Physics2D.Raycast(
            groundCheck.transform.position,
            groundCheck.transform.up * -1,
            groundCheck.groundCheckDistance,
            1 << LayerMask.NameToLayer("Solid"));

        Debug.DrawLine(
            groundCheck.transform.position,
            centerHit.collider != null
            ? (Vector3) centerHit.point
            : groundCheck.transform.position + groundCheck.transform.up * -groundCheck.groundCheckDistance,
            Color.yellow,
            0,
            false);


        if (centerHit.collider != null) {

            // Rotate player to normal
            float angle = Vector3.Angle(centerHit.normal, Vector3.up);
            transform.rotation = new Quaternion();

            if (centerHit.normal.x > 0) {
                transform.Rotate(new Vector3(0, 0, 360 - angle));
            }
            else {
                transform.Rotate(new Vector3(0, 0, angle));
            }

            // Update velocity vector to mimic player rotation changes
            Vector3 surfaceSlope = rightGroundCheck.transform.position;
            surfaceSlope -= leftGroundCheck.transform.position;
            surfaceSlope.Normalize();

            float rightVelAngle = Vector2.Angle(surfaceSlope, rbody.velocity);
            if (rightVelAngle < 90) {
                rbody.velocity = surfaceSlope * rbody.velocity.magnitude;
            }
            else if (rightVelAngle > 90) {
                rbody.velocity = surfaceSlope * -rbody.velocity.magnitude;
            }

            return (Vector3)centerHit.point - centerGroundCheck.transform.position;
        }

        return new Vector3();
    }

    void Reposition() {
        //UndoSolidPassThrough();

        if (!grounded) return;


        RaycastHit2D centerHit = Physics2D.Raycast(
            transform.position,
            transform.up * -1,
            groundCheck.groundCheckDistance,
            1 << LayerMask.NameToLayer("Solid"));

        Vector3 leftSrc = leftGroundCheck.transform.position;
        RaycastHit2D leftHit = Physics2D.Raycast(
            leftSrc,
            leftGroundCheck.transform.right * -1,
            leftCheckDist,
            1 << LayerMask.NameToLayer("Solid"));

        // DEBUGGING
        Debug.DrawLine(
            prevCheckPos,
            centerGroundCheck.transform.position,
            Color.magenta,
            0,
            false);

        Debug.DrawLine(
            leftSrc,
            leftHit.collider != null ? (Vector3)leftHit.point : leftSrc + leftGroundCheck.transform.right * -leftCheckDist,
            Color.green,
            0,
            false);

        if (leftHit.collider != null) {
            float angle = Vector3.Angle(leftHit.normal, Vector3.up);
            transform.rotation = new Quaternion();

            if (leftHit.normal.x > 0) {
                transform.Rotate(new Vector3(0, 0, 360 - angle));
            }
            else {
                transform.Rotate(new Vector3(0, 0, angle));
            }
            Vector3 posDifference = (Vector3)leftHit.point - centerGroundCheck.transform.position;
            transform.position += posDifference;
            rbody.velocity = new Vector2();
            return;
        }


        if (centerHit.collider != null) {
            float angle = Vector3.Angle(centerHit.normal, Vector3.up);
            transform.rotation = new Quaternion();
            
            if (centerHit.normal.x > 0) {
                transform.Rotate(new Vector3(0, 0, 360 - angle));
            } else {
                transform.Rotate(new Vector3(0, 0, angle));
            }

            if (speed < 5) return;

            Vector3 posDifference = (Vector3)centerHit.point - centerGroundCheck.transform.position;
            transform.position += posDifference;
        }

        /*
        Debug.DrawLine(
            rbody.transform.position,
            rbody.transform.position + (Vector3)rbody.velocity.normalized,
            Color.blue,
            5,
            false);
            */
        Vector3 surfaceSlope = rightGroundCheck.transform.position;
        surfaceSlope -= leftGroundCheck.transform.position;
        surfaceSlope.Normalize();

        float rightVelAngle = Vector2.Angle(surfaceSlope, rbody.velocity);
        if (rightVelAngle < 90) {
            rbody.velocity = surfaceSlope * rbody.velocity.magnitude;
        }
        else if (rightVelAngle > 90) {
            rbody.velocity = surfaceSlope * -rbody.velocity.magnitude;
        }

        prevCheckPos = centerGroundCheck.transform.position;

        centerHit = Physics2D.Raycast(
            transform.position,
            transform.up * -1,
            1000,
            1 << LayerMask.NameToLayer("Solid"));

        /*
        Debug.DrawLine(
            rbody.transform.position,
            rbody.transform.position + (Vector3)rbody.velocity.normalized,
            Color.cyan,
            0,
            false);
        Debug.DrawLine(
            centerHit.point,
            centerHit.point + centerHit.normal,
            Color.yellow,
            0,
            false);
            */
    }
    
    // Applies horizontal force to the rigidbody based on the horizontal input
    void MoveHorizontal() {
        float hInput = GameManager.InputHandler.getHorizontal();
        float speed = rbody.velocity.magnitude;
        // Apply force to the rigidbody while the player is on the ground
        if (hInput != 0 
            && speed <= maxRunSpeed
            && grounded) 
        {
             rbody.AddRelativeForce(new Vector2(runMultiplier * hInput, 0));
        }
        // Apply force when the player is airborne
        else if (hInput != 0
            && rbody.velocity.magnitude < parachuteTravelMaxSpeed
            && !grounded) 
        {
            rbody.AddRelativeForce(new Vector2(parachuteTravelAcceleration * hInput, 0));
        }
    }

    void Respawn() {
        transform.position.Set(spawnPoint.x, spawnPoint.y, spawnPoint.z);
    }
    
    public void TakeDamage(Vector3 hitPoint) {
        pDamaged.TakeDamage(hitPoint);
    }
        
    public void SetInteracting(bool state) {
        interacting = state;
    }
    
    public void CapLatched(bool val, Vector2 target) {
        capLatched = val;
        if (capLatched) {
            runAttackActive = false;
            rbody.gravityScale = 0;
            rbody.velocity = new Vector2();
            transform.position = Vector2.MoveTowards(transform.position, target, latchedCapSpeed);
            transform.up = (target - (Vector2)transform.position).normalized * -1;
        } else {
            //rbody.gravityScale = gravityScale;
        }
    }
   
    public bool isGrounded() { return groundCheck.isGrounded(); }
}
