using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {

    Animator animator;
    PlayerController playerCtrl;
    Rigidbody2D playerBody;
	
	void Start () {
        playerCtrl = gameObject.GetComponentInParent<PlayerController>();
        playerBody = gameObject.GetComponentInParent<Rigidbody2D>();
        animator = gameObject.GetComponentInParent<Animator>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        float speed = playerBody.velocity.magnitude / playerCtrl.maxRunSpeed;
        animator.SetFloat("Velocity", speed);
        animator.SetBool("ParachuteOpen", playerCtrl.isParachuteOpen());

        float verticalVelocity = playerBody.velocity.y;
        verticalVelocity = verticalVelocity < 0 ? -1 : verticalVelocity;
        verticalVelocity = verticalVelocity > 0 ? 1 : verticalVelocity;

        if (!playerCtrl.isGrounded() && playerBody.velocity.y > 0) {
            animator.SetBool("Jumping", true);
            animator.SetFloat("VerticalVelocity", verticalVelocity);
        }
        else if (!playerCtrl.isGrounded() && playerBody.velocity.y < 0) {
            animator.SetBool("Jumping", true);
            animator.SetFloat("VerticalVelocity", verticalVelocity);
        }
        animator.SetBool("Grounded", playerCtrl.isGrounded());
    }
    private void Update() {
        ScaleFlip();
    }

    void ScaleFlip () {
        float xScale = transform.localScale.x;
        if (playerBody.velocity.x < -0.1) {
            xScale = Mathf.Abs(xScale) * -1;
        }
        else if (playerBody.velocity.x > 0.1) {
            xScale = Mathf.Abs(xScale);
        }
        transform.localScale = new Vector3(
            xScale, 
            transform.localScale.y, 
            transform.localScale.z
        );
    }
}
