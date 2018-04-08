using UnityEngine;
using System.Collections;
using DentedPixel;

public class PathSpline2d : MonoBehaviour {

	public Transform[] cubes;

	public GameObject dude1;
	public GameObject dude2;

	private LTSpline visualizePath;

    int id;

    float timerStart = -1;
	void Start () {
		Vector3[] path = new Vector3[] {
			cubes[0].position,
			cubes[1].position,
			cubes[2].position,
			cubes[3].position,
			cubes[4].position,
			cubes[5].position,
			cubes[6].position,
			cubes[7].position
        };

		visualizePath = new LTSpline( path );

        timerStart = Time.time;

	}

    void FixedUpdate() {
        Vector3[] path = new Vector3[] {
            cubes[0].position,
            cubes[1].position,
            cubes[2].position,
            cubes[3].position,
            cubes[4].position,
            cubes[5].position,
            cubes[6].position,
            cubes[7].position
        };

        visualizePath = new LTSpline(path);

        float remaining = Time.time - timerStart;
        remaining /= 1;
        visualizePath.place2d(dude2.transform, remaining);
    }

    void OnDrawGizmos(){
		Gizmos.color = Color.red;
		if(visualizePath!=null)
			visualizePath.gizmoDraw();
	}
}
