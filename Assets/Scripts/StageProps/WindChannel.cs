using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindChannel : MonoBehaviour {

    public float windStrength = 1;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay2D(Collider2D other) {
        GameObject gobj = other.gameObject;
        if (gobj.tag.CompareTo("Player") == 0) {
            gobj.GetComponentInParent<Rigidbody2D>()
                .AddForce(transform.right * windStrength);
        }
    }
}
