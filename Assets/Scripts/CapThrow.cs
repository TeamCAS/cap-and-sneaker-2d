using UnityEngine;
using System.Collections;
using DentedPixel;
using System.Collections.Generic;

// Execute this code in edit mode so that the path is visible
// while it is being edited!
[ExecuteInEditMode]
public class CapThrow : MonoBehaviour {

    [Header("How long it takes for the cap to return in seconds")]
    public float throwDuration = 3;

    public Transform[] pathPoints;

    public GameObject cap;

    LTSpline visualizePath;
    float timerStart = -1;
    bool started;
    GameObject target;

    void Start() {
    }

    void Update() {
        PathUpdater();
    }

    void FixedUpdate() {
        if (GameManager.InputHandler.getThrow()) {
            if (!started) {
                timerStart = Time.time;
                cap.SetActive(true);
            }
            started = true;
            //StartCapThrow(null);
            print("THROWING");
        }
        if (started) {
            PathUpdater();
            float remaining = Time.time - timerStart;
            remaining /= throwDuration;
            visualizePath.place2d(cap.transform, remaining);
            if (remaining >= 1) {
                timerStart = -1;
                started = false;
                cap.SetActive(false);
            }
        }
    }

    void PathUpdater() {
        List<Vector3> path = new List<Vector3>();
        foreach (var tform in pathPoints) {
            path.Add(tform.position);
        }
        if (path.Count > 0) {
            visualizePath = new LTSpline(path.ToArray());
        }
    }

    public void StartCapThrow(GameObject dest) {
        target = dest;

        if (started) {
            Debug.LogWarning("Attempted to start a new throw when cap throw already in progress.");
        }
        started = true;
    }


    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (visualizePath != null)
            visualizePath.gizmoDraw();
    }
}
