using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        if (other.CompareTag("Player")) {
            PlayerController plyCtrl= other.GetComponentInParent<PlayerController>();
            plyCtrl.TakeDamage(collision.contacts[0].point);
            GameManager.DataHandler.SetPlayerHit();
        }
    }
}
