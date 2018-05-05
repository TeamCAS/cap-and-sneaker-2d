using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public float launchStrength = 1;

    EnemyGuardGround guard;
    void Start() {
        guard = GetComponentInParent<EnemyGuardGround>();
    }

    enum Action { HitPlayer, HitByFist, None, Blocked }
    Action action = Action.None;

    void FixedUpdate() {
        if (action == Action.Blocked) return;
        if (action == Action.HitByFist) {
            action = Action.Blocked;
            DieFromFistAttack();
        }
        else if (action == Action.HitPlayer) {
            action = Action.None;
            HitPlayer();
        }
    }

    PlayerController plyCtrl;
    Vector3 hitPoint;
    void OnCollisionEnter2D(Collision2D collision) {
        Collider2D other = collision.collider;
        print(other.name);
        if (other.CompareTag("Player") && action == Action.None) {
            plyCtrl = other.GetComponentInParent<PlayerController>();
            hitPoint = collision.contacts[0].point;
            action = Action.HitPlayer;
        }
    }

    void HitPlayer() {
        plyCtrl.TakeDamage(hitPoint);
        GameManager.DataHandler.SetPlayerHit();
    }


    Vector3 vel;
    void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("PlayerFist")) {
            print("Enemy Should Die");
            vel = (transform.position - other.transform.position) * launchStrength;
            action = Action.HitByFist;
        }
    }

    void DieFromFistAttack() {
        Rigidbody2D rbody = transform.parent.GetComponent<Rigidbody2D>();
        rbody.gravityScale = 0;
        rbody.velocity = vel;
        transform.parent.GetComponent<EnemyGuardGround>().enabled = false;

        Destroy(transform.parent.gameObject, 2);
        Destroy(transform.gameObject);

        foreach (Collider2D col in transform.parent.GetComponents<Collider2D>()) {
            col.enabled = false;
        }
        foreach (Transformer tran in transform.parent.GetComponents<Transformer>()) {
            tran.enabled = true;
        }
    }
}
