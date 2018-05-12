using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapController : MonoBehaviour {

    bool insideWindChannel = false;

    void OnTriggerStay2D(Collider2D other) {
        if (other.CompareTag("WindChannel")) {
            insideWindChannel = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        if (other.CompareTag("WindChannel")) {
            insideWindChannel = false;
        }
    }

    public bool isInsideWind () {
        return insideWindChannel;
    }
}
