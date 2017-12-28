using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopController : MonoBehaviour {

    public bool rotating = false;
    public float rpm = 30;
    public bool counterClockwise = false;

    float timeElapsed = 0;
    
	// Update is called once per frame
	void Update () {
        Rotate();
	}

    void Rotate() {
        if (rotating) {
            timeElapsed += Time.deltaTime;
            float rotation = -360 * timeElapsed * rpm / 60;
            if (counterClockwise) rotation *= -1;
            transform.eulerAngles = new Vector3(0, 0, rotation);
        }
    }
}
