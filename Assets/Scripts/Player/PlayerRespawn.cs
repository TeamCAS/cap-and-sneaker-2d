using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour {

    private void OnTriggerEnter2D (Collider2D other) {
        if (other.CompareTag("OutOfBounds")) {
            GameManager.DataHandler.setPlayerOutOfBounds(true);
            print("went out of bounds");
        }
    }

}
