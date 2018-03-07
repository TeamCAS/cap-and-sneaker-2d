using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour {

    bool collected = false;

    AudioSource audio;

    private void Start() {
        audio = GameObject.Find("PlayerLifeSFX").GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !collected) {
            collected = true;
            GameManager.DataHandler.incrementPlayerLives();
            audio.PlayOneShot(audio.clip);
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}
