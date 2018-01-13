using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour {

    Animator animator;
    PlayerController playerCtrl;
	
	void Start () {
        playerCtrl = gameObject.GetComponentInParent<PlayerController>();
        animator = gameObject.GetComponentInParent<Animator>();
    }

    public void UpdateParamaters(bool grounded, Vector2 velocity, bool parachuteOpen) {

        float speed = velocity.magnitude / playerCtrl.maxRunSpeed;
        animator.SetFloat("Velocity", speed);
        animator.SetBool("ParachuteOpen", parachuteOpen);


        float verticalVelocity = velocity.y;
        verticalVelocity = verticalVelocity < 0 ? -1 : verticalVelocity;
        verticalVelocity = verticalVelocity > 0 ? 1 : verticalVelocity;
        
        animator.SetFloat("VerticalVelocity", verticalVelocity);
        animator.SetBool("Grounded", grounded);

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
