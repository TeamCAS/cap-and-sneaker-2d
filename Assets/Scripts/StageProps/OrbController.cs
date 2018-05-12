using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbController : MonoBehaviour {

    // Time delay before destroying this orbs gameobject
    //public float destroyDelayMs = 3;

    bool collected = false;

    AudioSource audio;

    private void Start() {
        GameObject orbSFX = GameObject.Find("OrbSFX");
        if (orbSFX == null) Debug.LogWarning("OrbSFX GameObject not found!");
        else audio = orbSFX.GetComponent<AudioSource>();
        if (audio == null) Debug.LogWarning("OrbSFX GameObject does not have an AudioSource!");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !collected) {
            collected = true;
            GameManager.DataHandler.incrementOrbCount();
            if (audio != null) audio.PlayOneShot(audio.clip);
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

}
