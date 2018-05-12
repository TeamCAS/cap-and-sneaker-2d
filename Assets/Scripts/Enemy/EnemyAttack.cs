using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        print(other.name);
        if (other.CompareTag("Player")) {
            PlayerController plyCtrl= other.GetComponentInParent<PlayerController>();
            FistStrike fistStrike = other.GetComponentInParent<FistStrike>();
            //if (fistStrike.)
            plyCtrl.TakeDamage(collision.contacts[0].point);
            GameManager.DataHandler.SetPlayerHit();
        }
    }
}
