using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour {

    // Time delay before destroying this orbs gameobject
    public float destroyDelayMs = 3;

    AudioSource audio;

    private void Start() {
        audio = GameObject.Find("OrbSFX").GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player")) {
            GameManager.DataHandler.incrementOrbCount();
            audio.PlayOneShot(audio.clip);
            Destroy(gameObject, destroyDelayMs);
            gameObject.SetActive(false);
        }
    }

}
