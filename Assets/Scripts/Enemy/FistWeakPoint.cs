using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistWeakPoint : MonoBehaviour {

    EnemyGuardGround guard;
    public float launchStrength = 1;
    
    void Start() {
        guard = GetComponentInParent<EnemyGuardGround>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerFist")) {
            print("Should Die");

            Destroy(transform.parent.gameObject, 2);
            Rigidbody2D rbody = guard.GetComponent<Rigidbody2D>();
            Vector3 launch = transform.position - other.transform.position;
            rbody.velocity = launch * launchStrength;
            rbody.gravityScale = 0;
            guard.enabled = false;
            foreach (Collider2D col in transform.parent.GetComponents<Collider2D>()) {
                col.enabled = false;
            }
            foreach (Transformer tran in transform.parent.GetComponents<Transformer>()) {
                tran.enabled = true;
            }
            //Debug.Break();
        }
    }
}
