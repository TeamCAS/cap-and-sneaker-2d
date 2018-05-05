using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Transformer : MonoBehaviour {

    public enum Transformation { Position, Rotation, Scale }
    public Transformation action = Transformation.Scale;
    public bool loop;
    public bool reverse;
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

            float x, y, z;
            if (reverse) {
                x = Mathf.Lerp(xEnd, xStart, progress);
                y = Mathf.Lerp(yEnd, yStart, progress);
                z = Mathf.Lerp(zEnd, zStart, progress);
            } else {
                x = Mathf.Lerp(xStart, xEnd, progress);
                y = Mathf.Lerp(yStart, yEnd, progress);
                z = Mathf.Lerp(zStart, zEnd, progress);
            }

            if (action == Transformation.Scale) transform.localScale = new Vector3(x, y, z);
            else if (action == Transformation.Position) transform.position = new Vector3(x, y, z);
            else if (action == Transformation.Rotation) {
                Quaternion start = Quaternion.Euler(xStart, yStart, zStart);
                Quaternion end = Quaternion.Euler(xEnd, yEnd, zEnd);
                //transform.rotation = Quaternion.Lerp(start, end, progress);
                transform.rotation = Quaternion.Euler(x,y,z);
            }
            if (progress == 1) complete = true;
            if (progress == 1 && loop) {
                timerStart = Time.time;
            }
        }

        if (complete && !scheduledToDestroySelf && timeToSelfDestruct >= 0) {
            scheduledToDestroySelf = true;
            Destroy(gameObject, timeToSelfDestruct);
        }
	}
}
