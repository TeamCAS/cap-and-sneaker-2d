using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour {

    EnemyGuardGround egg;

    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        if (other.CompareTag("Player")) {
            PlayerController plyCtrl= other.GetComponentInParent<PlayerController>();
            plyCtrl.TakeDamage(collision.contacts[0].point);
            egg.SetHitPlayer();
        }
    }

    // Use this for initialization
    void Start () {
        egg = GetComponentInParent<EnemyGuardGround>();
        if (egg == null) Debug.LogWarning("EnemyGuardGround not found in parent object");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
