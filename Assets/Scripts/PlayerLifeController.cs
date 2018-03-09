using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLifeController : MonoBehaviour {

    bool collected = false;

    AudioSource audio;

    private void Start() {
        GameObject plSFX = GameObject.Find("PlayerLifeSFX");
        if (plSFX == null) Debug.LogWarning("PlayerLifeSFX GameObject not found!");
        else audio = plSFX.GetComponent<AudioSource>();
        if (audio == null) Debug.LogWarning("PlayerLifeSFX GameObject does not have an AudioSource!");
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("Player") && !collected) {
            collected = true;
            GameManager.DataHandler.incrementPlayerLives();
            if (audio != null) audio.PlayOneShot(audio.clip);
            Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }
}
