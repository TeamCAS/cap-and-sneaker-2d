using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeakPoints : MonoBehaviour {

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerFist")) {
            print("Should Die");
        }
    }
}
