using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeetStrike : MonoBehaviour {

    CircleCollider2D feetCollider;
    
    // Use this for initialization
    void Start () {
        feetCollider = GetComponent<CircleCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetActive(bool value) {
        feetCollider.enabled = value;
    }
}
