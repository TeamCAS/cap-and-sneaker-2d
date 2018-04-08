using UnityEngine;
using System.Collections;
using DentedPixel;
using System.Collections.Generic;

// Execute this code in edit mode so that the path is visible
// while it is being edited!
[ExecuteInEditMode]
public class PathSpline2d : MonoBehaviour {

	public Transform[] pathPoints;

	public GameObject dude1;
	public GameObject dude2;

	private LTSpline visualizePath;

    float timerStart = -1;

	void Start () {
        PathUpdater();

        timerStart = Time.time;

	}

    void PathUpdater () {
        List<Vector3> path = new List<Vector3>();
        foreach (var tform in pathPoints) {
            path.Add(tform.position);
        }
        visualizePath = new LTSpline(path.ToArray());
    }
    
    void Update() {
        PathUpdater();
    }

    void FixedUpdate() {
        PathUpdater();

        float remaining = Time.time - timerStart;
        remaining /= 1;
        visualizePath.place2d(dude2.transform, remaining);
        if (remaining >= 1) {
            timerStart = Time.time;
        }
    }

    void OnDrawGizmos(){
		Gizmos.color = Color.red;
		if(visualizePath!=null)
			visualizePath.gizmoDraw();
	}
}
