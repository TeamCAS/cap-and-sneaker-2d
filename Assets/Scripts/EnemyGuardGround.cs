using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGuardGround : MonoBehaviour {

    public float patrolSpeed = 1;
    public float chaseSpeed = 2;
    public float chaseDuration = 2;
    [Header("How long to wait before chasing again")]
    public float restDuration = 2;
    [Header("Max distance to travel before turning around")]
    public float maxDistance = 1;
    [Header("If empty, will be the enemy starting position")]
    public Transform source;

    Rigidbody2D rbody;
    Vector3 sourcePos;
    float travelDir = 1;

    Transform chaseTarget;

    bool targetSpotted, rested, chaseTimerStarted, restTimerStarted;
    float chaseTimeStart, restTimeStart;

    void Start() {
        rbody = gameObject.GetComponent<Rigidbody2D>();
        if (source != null) {
            sourcePos = source.position;
        } else {
            sourcePos = transform.position;
        }
    }

    // Update is called once per frame
    void FixedUpdate () {

        if (GameManager.DataHandler.GetPlayerHitStatus()) {
            restTimerStarted = false;
            rested = false;
            //print("HIT THE PLAYER YAY!!!!");
        }

        if (targetSpotted && rested) {
            Chase();
            if (!chaseTimerStarted) {
                chaseTimerStarted = true;
                chaseTimeStart = Time.time;
            }
            if (Time.time - chaseTimeStart >= chaseDuration) {
                restTimerStarted = false;
                rested = false;
            }
        }
        else {
            Patrol();
            if (!restTimerStarted) {
                restTimerStarted = true;
                restTimeStart = Time.time;
            }
            if (Time.time - restTimeStart >= restDuration) {
                chaseTimerStarted = false;
                rested = true;
            }
        }
        
        
	}

    void Patrol () {
        TurnAround();
        rbody.velocity = new Vector2(travelDir * patrolSpeed, rbody.velocity.y);
    }

    void Chase () {
        if (chaseTarget.position.x < transform.position.x) travelDir = -1;
        else travelDir = 1;

        rbody.velocity = new Vector2(travelDir * chaseSpeed, rbody.velocity.y);
    }

    void TurnAround () {
        float dist = Vector3.Distance(transform.position, sourcePos);

        if (dist > maxDistance) {
            if (transform.position.x < sourcePos.x) travelDir = 1;
            else travelDir = -1;
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            chaseTarget = other.transform;
            targetSpotted = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            chaseTarget = null;
            targetSpotted = false;
            chaseTimerStarted = false;
            restTimerStarted = false;
        }
    }

    void OnDrawGizmos () {
        // Draw circle to see the source in the editor but not in game
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(sourcePos, 0.5f);
    }

}
