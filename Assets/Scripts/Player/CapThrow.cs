using UnityEngine;
using System.Collections;
using DentedPixel;
using System.Collections.Generic;

// Execute this code in edit mode so that the path is visible
// while it is being edited!
//[ExecuteInEditMode]
public class CapThrow : MonoBehaviour {

    [Header("How long it takes for the cap to return in seconds")]
    public float throwDuration = 3;

    public Transform[] pathPoints;

    public GameObject cap;
    public GameObject reticle;

    LTSpline visualizePath;
    GameObject path;
    Vector3 targetPos;
    GameObject closestTarget;
    float maxTargetDistance;


    void Start() {
        path = GameObject.Find("Path");
        maxTargetDistance = GetComponent<CircleCollider2D>().radius;
    }

    void Update() {
        PathUpdater();
        UpdateTarget();
    }


    float timeElapsed = 0; // Time the cap has been in flight
    float remaining = 0; // previous flight time plus 
    bool throwStarted = false;

    GameObject hookedTarget;
    // TODO Setup detection for this
    bool hooked = false;
    bool prevThrow;

    void FixedUpdate() {

        if (closestTarget != null && closestTarget.layer == (int)GameManager.Layer.Enemy) {
            targetPos = closestTarget.transform.position;
        }

        bool currThrow = GameManager.InputHandler.getThrow();

        // Throw is being held down
        if (prevThrow && currThrow) {
            if (hooked) {
                cap.SetActive(false); // hide cap
                PauseThrow(); // Pause cap travel
            }
            else {
                cap.SetActive(true); // Unhide cap
                ResumeThrow(); // Resume cap travel
            }
        }
        // Starting a basic throw
        else if (prevThrow && !currThrow) {
            cap.SetActive(true); // Unhide cap
            
            // If hooked, need to unhook and return cap to player
            if (hooked) {
                cap.GetComponent<Collider2D>().enabled = false;
            }
            hooked = false;
            ResumeThrow();
        }
        else if (throwStarted) {
            hooked = false;
            ResumeThrow();
        }

        PlayerController ctrl = transform.parent.GetComponent<PlayerController>();
        ctrl.CapLatched(hooked, targetPos);

        if (hookedTarget != null) {
            hookedTarget.GetComponent<HookableByCapThrow>().Hooked(hooked);
        }

        prevThrow = currThrow;
    }


    void OnTriggerStay2D(Collider2D collision) {
        bool targetable = false;
        int layer = collision.gameObject.layer;

        if (layer == (int)GameManager.Layer.Collectible) targetable = true;
        else if (layer == (int)GameManager.Layer.Targetable) targetable = true;

        if (targetable && !throwStarted) {
            if (closestTarget == null) {
                closestTarget = collision.gameObject;
                targetPos = closestTarget.transform.position;
            }
            else {
                float dist = Vector3.Distance(transform.position, collision.transform.position);
                float closestDistance = Vector3.Distance(transform.position, closestTarget.transform.position);
                if (dist < closestDistance) {
                    closestTarget = collision.gameObject;
                    targetPos = closestTarget.transform.position;
                }
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision) {
        if (collision.gameObject == closestTarget && !throwStarted) {
            closestTarget = null;
            targetPos = new Vector3();
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


    void UpdateTarget() {
        if (closestTarget != null) {
            Vector3 dir = targetPos - transform.position;
            dir.Normalize();
            transform.right = dir;
            reticle.SetActive(true);
            reticle.transform.position = closestTarget.transform.position;

            float throwDist = Vector3.Distance(targetPos, transform.position);
            path.transform.localScale = new Vector3(throwDist, 1, 1);

            if (throwDist > maxTargetDistance && !throwStarted) {
                closestTarget = null;
            }
        }
        else {
            reticle.SetActive(false);
            transform.right = Vector3.right;
            path.transform.localScale = new Vector3(maxTargetDistance, 1, 1);
        }
    }


    float timerStart = -1;

    void ResumeThrow() {
        PathUpdater();

        if (!throwStarted) {
            timerStart = Time.time;
            throwStarted = true;
        }
        remaining = (Time.time - timerStart) + timeElapsed;

        visualizePath.place2d(cap.transform, remaining / throwDuration);
        if (remaining / throwDuration >= 1) {
            timerStart = -1;
            throwStarted = false;
            timeElapsed = 0;
            cap.GetComponent<Collider2D>().enabled = true; // reenable for new throws
            hooked = false; // Can't be hooked to something if cap has returned
            cap.SetActive(false);
        }

    }

    void PauseThrow() {
        timeElapsed = remaining;
        remaining = 0;
        throwStarted = false;
    }


    public void HookedTo(GameObject target) {
        hooked = true;
        hookedTarget = target;
    }
    

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (visualizePath != null)
            visualizePath.gizmoDraw();
    }
}
