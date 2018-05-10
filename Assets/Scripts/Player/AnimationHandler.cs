using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {

    Animator animator;
    PlayerController playerCtrl;

    GameObject boom;
    SmokeTrail smokeTrail;

	void Start () {
        playerCtrl = gameObject.GetComponentInParent<PlayerController>();
        animator = gameObject.GetComponentInParent<Animator>();
        smokeTrail = GetComponent<SmokeTrail>();

        foreach(Transform t in transform) {
            if (t.name == "Boom") {
                boom = t.gameObject;
            }
        }
    }

    public void UpdateParamaters(
        bool grounded, 
        Vector2 velocity, 
        bool parachuteOpen, 
        bool playerHit,
        bool runAttackActive,
        bool capKick) 
    {

        // Set the animation value, if true don't update other values
        // since the damage animation should take priority
        animator.SetBool("HitBack", playerHit);
        if (playerHit) return;

        animator.SetBool("CapKick", capKick);

        float speed = velocity.magnitude / playerCtrl.maxRunSpeed;
        animator.SetFloat("Velocity", speed);
        animator.SetBool("ParachuteOpen", parachuteOpen);

        float verticalVelocity = velocity.y;
        verticalVelocity = verticalVelocity < 0 ? -1 : verticalVelocity;
        verticalVelocity = verticalVelocity > 0 ? 1 : verticalVelocity;
        
        animator.SetFloat("VerticalVelocity", verticalVelocity);
        animator.SetBool("Grounded", grounded);

        boom.SetActive(runAttackActive);
        if (runAttackActive) smokeTrail.enabled = true;
        else smokeTrail.enabled = false;

        ScaleFlip(velocity, playerCtrl.transform.up);
    }

    void ScaleFlip (Vector2 velocity, Vector2 up) {
        float xScale = transform.localScale.x;
        // player traveling left
        if (velocity.x < -0.1) {
            if (up.y >= 0) {
                xScale = Mathf.Abs(xScale) * -1;
            }
            else if (up.y < 0) {
                xScale = Mathf.Abs(xScale);
            }
        }
        // Player traveling right
        else if (velocity.x > 0.1) {
            if (up.y >= 0) {
                xScale = Mathf.Abs(xScale);
            }
            else if (up.y < 0) {
                xScale = Mathf.Abs(xScale) * -1;
            }
        }
        transform.localScale = new Vector3(
            xScale, 
            transform.localScale.y, 
            transform.localScale.z
        );
    }
}
