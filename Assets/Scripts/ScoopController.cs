using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoopController : MonoBehaviour
{

    public bool rotating = false;
    public float rpm = 30;
    public bool counterClockwise = false;
    public float launchSpeed = 10;

    float timeElapsed = 0;
    Transform payLoadPositioner = null;
    GameObject payLoad;
    Vector3 startingRotation;

    void Start() {
        Transform[] childTransforms = gameObject.GetComponentsInChildren<Transform>();
        foreach (Transform t in childTransforms) {
            payLoadPositioner = t.gameObject.name.Contains("Payload") ? t : null;
        }

        if (payLoadPositioner == null) Debug.LogError("Payload is null");


        startingRotation = transform.eulerAngles;
    }

    void FixedUpdate() {
        if (payLoad != null) {
            payLoad.transform.position = payLoadPositioner.position;
            payLoad.transform.rotation = payLoadPositioner.rotation;

            if (GameManager.InputHandler.jumpPressed()) {
                payLoad.GetComponent<PlayerController>().SetInteracting(false);
                payLoad.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0, launchSpeed));
                payLoad = null;
            }
        } else {
            if (transform.eulerAngles.z < 5 + startingRotation.z && transform.eulerAngles.z > startingRotation.z - 5) {
                rotating = false;
                transform.eulerAngles = startingRotation;
                gameObject.GetComponent<Collider2D>().enabled = true;
            }
        }
    }

    // Update is called once per frame
    void Update() {
        Rotate();
    }

    void Rotate() {
        if (rotating) {
            timeElapsed += Time.deltaTime;
            float rotation = -360 * timeElapsed * rpm / 60;
            rotation += startingRotation.z;
            if (counterClockwise) rotation *= -1;
            transform.eulerAngles = new Vector3(0, 0, rotation);
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            PlayerController ply = other.gameObject.GetComponentInParent<PlayerController>();
            ply.SetInteracting(true);
            ply.transform.position = payLoadPositioner.position;
            ply.transform.rotation = payLoadPositioner.rotation;
            payLoad = ply.gameObject;
            rotating = true;
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
    }
}
