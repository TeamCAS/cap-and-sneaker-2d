using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetrievableByCapThrow : MonoBehaviour {

    Transform posAnchor;

    private void Start() {
        posAnchor = transform;
    }

    private void FixedUpdate() {
        transform.position = posAnchor.position;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("ThrownCap")) {
            posAnchor = other.transform;
            print("anchor set");
        }
    }
}
