using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistStrike : MonoBehaviour {

    CircleCollider2D fistCollider;

	// Use this for initialization
	void Start () {
        fistCollider = GetComponent<CircleCollider2D>();
	}
	
    void FixedUpdate() {
        
    }

    public void SetActive(bool value) {
        fistCollider.enabled = value;
    }
}
