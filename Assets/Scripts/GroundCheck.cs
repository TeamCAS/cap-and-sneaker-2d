using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {
    private bool grounded = false;

    public bool isGrounded() {
        return grounded;
    }

    // Set grounded to true when a collider with Ground tag is entering
    // this gameobjects collider
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Ground")) {
            grounded = true;
        }
    }

    // Set grounded to false when a collider with Ground tag is exiting
    // this gameobjects collider
    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("Ground")) {
            grounded = false;
        }
    }
}
