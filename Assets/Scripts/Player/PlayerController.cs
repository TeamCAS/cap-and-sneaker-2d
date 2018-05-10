﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float speed;

    public float runMultiplier = 200;
    public float maxRunSpeed = 40;
    public float runAttackMinSpeed = 36;


    public float jumpStrength = 1000;
    public float parachuteDescentSpeed = 4.5f;
    public float parachuteTravelAcceleration = 1;
    public float parachuteTravelMaxSpeed = 20;
    public float stickyStartSpeed = 5;
    public float stickyScale = 30;
    public float stickyDistance = 3;
    public float gravityScale= 5;
    public float fallingRotationSpeed = 0.1f;

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
    bool parachuteOpen;
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
    bool capLatched;



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
            if (t.name == "Fist") fistStrike = t.GetComponent<FistStrike>();
        }
    }

    void Update() {
        grounded = groundCheck.isGrounded();
        
        animations.UpdateParamaters(grounded, rbody.velocity, parachuteOpen, damageTaken, runAttackActive, capLatched);
    }

    void FixedUpdate() {
        

        // Check if player has recovered
        if (damageTaken && Time.time - hitTimerStart >= damagedDuration) {
            damageTaken = false;
            GameManager.InputHandler.enableControls();
            GameManager.DataHandler.SetPlayerRecovered();
        }

        if (capLatched) return;

        speed = rbody.velocity.magnitude;
        grounded = groundCheck.isGrounded();

        // Activate player run attack if traveling fast enough.
        runAttackActive = false;
        if (grounded && speed >= runAttackMinSpeed) {
            runAttackActive = true;
            print("RUN ATTACK ACTIVATED");
        }
        fistStrike.SetActive(runAttackActive);

        bool leftSolid = groundCheck.solidToLeft();
        bool rightSolid = groundCheck.solidToRight();
        if (leftSolid && rbody.velocity.x < 0 || rightSolid && rbody.velocity.x > 0) {
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }

        groundedSignal.SetActive(!grounded);
        rbody.gravityScale = 0;

        // return if player is interacting
        if (interacting) return;

        if (grounded) {
            Vector3 posDIff = RotateToSurfaceNormal();
            if (speed > 20) {
                transform.position += posDIff;
            } else {
                rbody.gravityScale = gravityScale;
            }
            rotationStartTime = -1;
        } else {
            rbody.gravityScale = gravityScale;
            InterpolateRotation();
        }
        
        MoveHorizontal();
        Jump();
        ToggleParachute();

        prevGrounded = grounded;
    }

    void InterpolateRotation() {
        rotationStartTime = rotationStartTime == -1 ? Time.time : rotationStartTime;
        Quaternion current = transform.rotation;
        Quaternion dest = Quaternion.Euler(0, 0, 0);
        float t = (Time.time - rotationStartTime) * fallingRotationSpeed;
        transform.rotation = Quaternion.Slerp(current, dest, t);
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
        // Apply force to the rigidbody while the player is on the ground
        if (hInput != 0 
            && rbody.velocity.magnitude < maxRunSpeed
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
    
    // Applies vertical forces to the rigidbody based on whether jump input
    // has been detected and if the player is on the ground
    void Jump() {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();

        // If input is not 1, the player released the button, they can jump
        if (!jumpPressed) canJump = true;

        // Player can jump as long as they are grounded and weren't holding
        // down jump i.e. canJump is true
        if (jumpPressed && grounded && canJump) {
            rbody.AddRelativeForce(new Vector2(0, jumpStrength));
            canJump = false;
        }
    }

    // Toggle whether the parachute is open or not
    void ToggleParachute() {
        bool jumpPressed = GameManager.InputHandler.jumpPressed();
        bool isVelocityDown = rbody.velocity.y < 0;
        bool playerFalling = !grounded;
        bool insideWindChannel = capCtrl.isInsideWind();
        //playerFalling = playerFalling && isVelocityDown;

        if (playerFalling 
            && jumpPressed
            && (isVelocityDown || insideWindChannel)
            ) {
                parachuteOpen = true;
                rbody.AddRelativeForce(new Vector3(0, 9.8f * parachuteDescentSpeed));
        } else {
            parachuteOpen = false;
        }
    }
    

    void Respawn() {
        transform.position.Set(spawnPoint.x, spawnPoint.y, spawnPoint.z);
    }

    bool damageTaken = false;
    public void TakeDamage(Vector3 hitPoint) {
        print("Player Hit");
        // Only take damage once recovered
        if (damageTaken) return;
        hitTimerStart = Time.time;
        damageTaken = true;
        GameManager.DataHandler.SetPlayerHit();
        GameManager.SoundHandler.StartPlayerHitSFX();

        // Disable Controls
        GameManager.InputHandler.disableControls();

        // If no orbs then player faints
        if (GameManager.DataHandler.getOrbCount() == 0) {
            print("Player should faint");
            GameManager.DataHandler.SetPlayerDead();
        }

        // Else player loses orbs
        else {
            print("Player should lose orbs");
            DropOrbs();
            rbody.velocity = new Vector2();

            // Get right and left points based on characters location
            // in world space
            Vector3 trueRight = transform.right + transform.position;
            Vector3 trueLeft = -1 * transform.right + transform.position;

            // Test which side the player was hit from, left or right
            float rightDist = Vector3.Distance(trueRight, hitPoint);
            float leftDist = Vector3.Distance(trueLeft, hitPoint);

            if (rightDist < leftDist) {
                // Player was hit from the player gameobjects right
                Vector3 hitVector = transform.right * -1;
                hitVector += new Vector3(0, 1, 0);
                hitVector.Normalize();
                hitVector *= hitPushStrength;
                rbody.AddForce(hitVector);
            }
            else {
                // Player was hit from the player gameobjects left
                Vector3 hitVector = transform.right;
                hitVector += new Vector3(0, 1, 0);
                hitVector.Normalize();
                hitVector *= hitPushStrength;
                rbody.AddForce(hitVector);
            }
        }

        // Update animation parameters

        // Start recovery time

    }

    public float orbDropVelocity = 1;
    void DropOrbs() {
        float orbCount = GameManager.DataHandler.getOrbCount();
        GameObject orbClone;
        if (orbCount >= 1) {
            orbClone = GameManager.ObjectCreator.createOrb(transform.position, new Quaternion());
            Vector2 vel = new Vector2(0, 1).normalized * orbDropVelocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 2) {
            orbClone = GameManager.ObjectCreator.createOrb(transform.position, new Quaternion());
            Vector2 vel = new Vector2(1, 1).normalized * orbDropVelocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 3) {
            orbClone = GameManager.ObjectCreator.createOrb(transform.position, new Quaternion());
            Vector2 vel = new Vector2(-1, 1).normalized * orbDropVelocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 4) {
            orbClone = GameManager.ObjectCreator.createOrb(transform.position, new Quaternion());
            Vector2 vel = new Vector2(0.5f, 1).normalized * orbDropVelocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 5) {
            orbClone = GameManager.ObjectCreator.createOrb(transform.position, new Quaternion());
            Vector2 vel = new Vector2(-0.5f, 1).normalized * orbDropVelocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }

        GameManager.DataHandler.zeroOrbCount();
    }

    public void SetInteracting(bool state) {
        interacting = state;
    }


    public void CapLatched(bool val, Vector2 target) {
        capLatched = val;
        if (capLatched) {
            rbody.gravityScale = 0;
            rbody.velocity = new Vector2();
            transform.position = Vector2.MoveTowards(transform.position, target, latchedCapSpeed);
        } else {
            rbody.gravityScale = gravityScale;
        }
    }

    public bool isParachuteOpen() { return parachuteOpen; }

    public bool isGrounded() { return groundCheck.isGrounded(); }
}