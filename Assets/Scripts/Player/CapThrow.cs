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
    float timerStart = -1;
    bool started;
    Vector3 targetPos;
    GameObject closestTarget;

    GameObject path;
    float maxTargetDistance;

    bool prevThrow;

    void Start() {
        path = GameObject.Find("Path");
        maxTargetDistance = GetComponent<CircleCollider2D>().radius;
    }

    void Update() {
        PathUpdater();
        UpdateTarget();
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

            if (throwDist > maxTargetDistance && !started) {
                closestTarget = null;
            }
        } else {
            reticle.SetActive(false);
            transform.right = Vector3.right;
            path.transform.localScale = new Vector3(maxTargetDistance, 1, 1);
        }
    }

    void OnTriggerStay2D(Collider2D collision) {
        bool targetable = false;
        int layer = collision.gameObject.layer;

        if (layer == (int)GameManager.Layer.Collectible) targetable = true;
        else if (layer == (int)GameManager.Layer.Enemy) targetable = true;

        if (targetable && !started) {
            if (closestTarget == null) {
                closestTarget = collision.gameObject;
                targetPos = closestTarget.transform.position;
            } else {
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
        if (collision.gameObject == closestTarget && !started) {
            closestTarget = null;
            targetPos = new Vector3();
        }
    }

    void FixedUpdate() {
        
        if (closestTarget != null && closestTarget.layer == (int)GameManager.Layer.Enemy) {
            targetPos = closestTarget.transform.position;
        }

        bool currThrow = GameManager.InputHandler.getThrow();

        PlayerController plyCtrl = transform.parent.GetComponent<PlayerController>();
        bool capLatched = targetLocked && prevThrow && currThrow;
        plyCtrl.CapLatched(capLatched, targetPos);

        if (currThrow) {
            print("THROWING");
            
            if (!started && !targetLocked) {
                timerStart = Time.time;
                cap.SetActive(true);
                started = true;
                print("basic throw");
            }
        }
        if (started && !targetLocked) {
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

        prevThrow = currThrow;
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
        //target = dest;

        if (started) {
            Debug.LogWarning("Attempted to start a new throw when cap throw already in progress.");
        }
        started = true;
    }

    bool targetLocked;
    public void PauseCapThrow() {
        started = false;
        targetLocked = true;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (visualizePath != null)
            visualizePath.gizmoDraw();
    }
}
