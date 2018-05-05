using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeTrail : MonoBehaviour {

    public float interval = 1;
    GameObject smoke;

    // Use this for initialization
	void Start () {
        foreach (Transform item in transform) {
            if (item.name == "Smoke") smoke = item.gameObject;
        }
	}

    float lastCreatedTime;
	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time - lastCreatedTime > interval) {
            GameObject clone = GameObject.Instantiate(smoke, transform.root.parent, false);
            clone.SetActive(true);
            clone.transform.position = smoke.transform.position;
            lastCreatedTime = Time.time;
        }
	}

    
}
