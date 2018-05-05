using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour {

    //public enum Transformation { Grow, Shrink }
    //public Transformation action = Transformation.Grow;
    [Header("Time in seconds to complete transformation")]
    public float duration = 1;
    public float xStart = 1;
    public float xEnd = 1;
    public float yStart = 1;
    public float yEnd = 1;
    public float zStart = 1;
    public float zEnd = 1;

    [Header("Self destruct after x seconds, -1 to disable")]
    public float timeToSelfDestruct = -1;
    
    // Use this for initialization
    void Start () {
        timerStart = Time.time;
	}

    float timerStart = -1;

    bool complete;
    bool scheduledToDestroySelf;

	// Update is called once per frame
	void Update () {
		if (timerStart != -1) {
            float progress = (Time.time - timerStart) / duration;
            progress = Mathf.Clamp(progress, 0, 1);

            float x = Mathf.Lerp(xStart, xEnd, progress);
            float y = Mathf.Lerp(yStart, yEnd, progress);
            float z = Mathf.Lerp(zStart, zEnd, progress);

            transform.localScale = new Vector3(x,y,z);

            if (progress == 1) complete = true;
        }

        if (complete && !scheduledToDestroySelf) {
            scheduledToDestroySelf = true;
            Destroy(gameObject, timeToSelfDestruct);
        }
	}
}
