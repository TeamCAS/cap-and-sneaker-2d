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

    [Header("Bind Settings")]
    public float bindSpeed = 20;
    public float bindDistance = 1;
    public float bindIntervals = 4;

    public float runAttackMinSpeed = 36;

    public float jumpStrength = 1000;
    public float parachuteDescentSpeed = 4.5f;
    public float parachuteTravelAcceleration = 1;
    public float parachuteTravelMaxSpeed = 20;

    [Header("Fall Settings")]
    public float fallingRotationSpeed = 0.1f;
    public float ascendMultiplier = 2;
    public float descendMultiplier = 3;

    [Header("Ground Check Settings")]
    public float gcSideOffet = 1;
    public float gcCheckDist = 1;

    [Header("How far the player gets pushed when hit")]
    public float hitPushStrength = 1;
    [Header("Duration in seconds the player has no control when damaged")]
    public float damagedDuration = 1;
    [Header("The speed of the player when pulled towards the latched cap")]
    public float latchedCapSpeed = 1;

    Vector3 spawnPoint;
    Rigidbody2D rbody;
    bool interacting = false;

    CapController capCtrl;


    AnimationHandler animations;
    bool grounded;

    //Indicators for debugging
    GameObject groundedSignal;


    float hitTimerStart;
    bool runAttackActive = false;
    FistStrike fistStrike;
    FeetStrike feetStrike;
    bool capLatched;
    public float orbDropVelocity = 1;
    
    PlayerJump pJump;
    PlayerDamaged pDamaged;
    PlayerGroundCheck pGC;
    PlayerFalling pFalling;
    PlayerParachute pParachute;
    PlayerBindToSurface pBts;



	// Use this for initialization
	void Start () {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        spawnPoint = new Vector3() + transform.position;

        capCtrl = gameObject.GetComponentInChildren<CapController>();

        animations = gameObject.GetComponentInChildren<AnimationHandler>();

        groundedSignal = GameObject.Find("GroundedSignal");

        foreach (Transform t in transform) {
            if (t.name == "FistStrike") fistStrike = t.GetComponent<FistStrike>();
            if (t.name == "FeetStrike") feetStrike = t.GetComponent<FeetStrike>();
        }

        pJump = new PlayerJump(rbody, jumpStrength);
        pDamaged = new PlayerDamaged(hitPushStrength, rbody, orbDropVelocity, damagedDuration);
        pGC = new PlayerGroundCheck(transform);
        pFalling = new PlayerFalling(rbody, transform);
        pParachute = new PlayerParachute(rbody, parachuteDescentSpeed, capCtrl);
        pBts = new PlayerBindToSurface(transform, rbody);
    }

    bool repositioned = false;
    void Update() {
        pBts.Reposition(bindIntervals, bindDistance, bindSpeed, jumped);
        /************* Determine values *************/
        grounded = pGC.IsGrounded(gcSideOffet, gcCheckDist);

        runAttackActive = false;
        surfaceSpeed = grounded ? Utilities.GetSurfaceSpeed2D(rbody) : 0;
        if (grounded && surfaceSpeed >= runAttackMinSpeed) runAttackActive = true;
        if (grounded) pFalling.SetGrounded();

        groundedSignal.SetActive(!grounded);
        fistStrike.SetActive(runAttackActive);

        repositioned = true;



        animations.UpdateParamaters(
            grounded,
            rbody.velocity,
            pParachute.isOpen(), 
            pDamaged.isDamaged(), 
            runAttackActive, 
            capLatched
        );
    }

    bool jumped = false;

    void FixedUpdate() {

        if (rbody.velocity.magnitude == 0) pBts.SetDistanceToSurface();

        // If player is damaged, no need to do anything, player has no control
        if (pDamaged.isDamaged()) return;

        // return if player is interacting
        if (interacting) return;


        /************* Reposition *************/
        if (repositioned) repositioned = false;
        else pBts.Reposition(bindIntervals, bindDistance, bindSpeed, jumped);


        // RO, not used for calculations
        speed = rbody.velocity.magnitude;


        /************* Determine values *************/
        grounded = pGC.IsGrounded(gcSideOffet, gcCheckDist);

        runAttackActive = false;
        surfaceSpeed = grounded ? Utilities.GetSurfaceSpeed2D(rbody) : 0;
        if (grounded && surfaceSpeed >= runAttackMinSpeed) runAttackActive = true;
        if (grounded) pFalling.SetGrounded();

        groundedSignal.SetActive(!grounded);
        fistStrike.SetActive(runAttackActive);


        /************* Apply New Forces *************/
        if (grounded) {
            jumped = pJump.Jump(rbody, jumpStrength);
        }
        else {
            pFalling.RotateToUpRight(fallingRotationSpeed);
            pFalling.MultiplyGravity(descendMultiplier, ascendMultiplier);
        }

        MoveHorizontal();
        pParachute.Toggle(grounded);


        //feetStrike.SetActive(capLatched);
        //if (capLatched) return;

        /*
        bool leftSolid = groundCheck.solidToLeft();
        bool rightSolid = groundCheck.solidToRight();
        if (leftSolid && rbody.velocity.x < 0 || rightSolid && rbody.velocity.x > 0) {
            rbody.velocity = new Vector2(0, rbody.velocity.y);
        }
        */

        // RO, not used for calculations
        speedAfter = rbody.velocity.magnitude;

        // Utilities.DrawArrow(transform.position, transform.position + transform.up, Color.cyan, 1);
        // Utilities.DrawArrow(transform.position, transform.position + transform.right, Color.magenta, 1);
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
   
    public bool isGrounded() { return pGC.IsGrounded(gcSideOffet, gcCheckDist); }
}
