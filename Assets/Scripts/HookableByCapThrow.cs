using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookableByCapThrow : MonoBehaviour {

    SpriteRenderer sprite;

	// Use this for initialization
	void Start () {
        sprite = GetComponent<SpriteRenderer>();
	}
	
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("ThrownCap")) {
            other.transform.parent.GetComponent<CapThrow>().HookedTo(gameObject);
        }
    }

    public void Hooked(bool val) {
        sprite.enabled = val;
    }
}
