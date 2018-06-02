using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamaged {

    bool damageTaken = false;
    float hitTimerStart;
    float duration;
    Rigidbody2D rbody;
    float orbVel;
    float hitStrength;

    public PlayerDamaged(float hitStrength, Rigidbody2D rbody, float orbVel, float damageDuration) {
        this.hitStrength = hitStrength;
        this.rbody = rbody;
        this.orbVel = orbVel;
        this.duration = damageDuration;
    }

    public void TakeDamage(Vector3 hitPoint) {
        
        if (damageTaken) return; // Only take damage once recovered

        hitTimerStart = Time.time;
        damageTaken = true;
        Transform transform = rbody.transform;

        GameManager.DataHandler.SetPlayerHit();
        GameManager.SoundHandler.StartPlayerHitSFX();
        GameManager.InputHandler.disableControls();

        // If no orbs then player faints
        if (GameManager.DataHandler.getOrbCount() == 0) {
            GameManager.DataHandler.SetPlayerDead();
        }

        // Else player loses orbs
        else {
            DropOrbs(orbVel, transform.position);
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
                hitVector *= hitStrength;
                rbody.AddForce(hitVector);
            }
            else {
                // Player was hit from the player gameobjects left
                Vector3 hitVector = transform.right;
                hitVector += new Vector3(0, 1, 0);
                hitVector.Normalize();
                hitVector *= hitStrength;
                rbody.AddForce(hitVector);
            }
        }

        // Update animation parameters

        // Start recovery time

    }

    void DropOrbs(float velocity, Vector3 startPos) {
        float orbCount = GameManager.DataHandler.getOrbCount();
        GameObject orbClone;

        if (orbCount >= 1) {
            orbClone = GameManager.ObjectCreator.createOrb(startPos, new Quaternion());
            Vector2 vel = new Vector2(0, 1).normalized * velocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 2) {
            orbClone = GameManager.ObjectCreator.createOrb(startPos, new Quaternion());
            Vector2 vel = new Vector2(1, 1).normalized * velocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 3) {
            orbClone = GameManager.ObjectCreator.createOrb(startPos, new Quaternion());
            Vector2 vel = new Vector2(-1, 1).normalized * velocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 4) {
            orbClone = GameManager.ObjectCreator.createOrb(startPos, new Quaternion());
            Vector2 vel = new Vector2(0.5f, 1).normalized * velocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }
        if (orbCount >= 5) {
            orbClone = GameManager.ObjectCreator.createOrb(startPos, new Quaternion());
            Vector2 vel = new Vector2(-0.5f, 1).normalized * velocity;
            orbClone.GetComponent<Rigidbody2D>().velocity = vel;
        }

        GameManager.DataHandler.zeroOrbCount();
    }


    public bool isDamaged() {
        float timeElapsed = Time.time - hitTimerStart;
        if (timeElapsed >= duration) {
            damageTaken = false;
            GameManager.InputHandler.enableControls();
            GameManager.DataHandler.SetPlayerRecovered();
        }

        return damageTaken;
    }

    public void setDamaged(bool val) { damageTaken = val; }
}
