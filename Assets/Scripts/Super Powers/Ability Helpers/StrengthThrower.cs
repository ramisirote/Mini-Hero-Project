using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script goes on the game object that is attached to a character hit by the super strength basic attack.
 * When attacked to a character, moves them in the direction. When collides with a enemy, deals damage to them.
 */
public class StrengthThrower : MonoBehaviour
{
    [SerializeField] private float flyTime;
    [SerializeField] private float throwForce;
    [SerializeField] private float damage;
    private GameObject parent;
    private IManager parentManager;
    private float doneTime;
    private Rigidbody2D rb;
    private Vector3 m_Velocity = Vector3.zero;

    private bool pushed = false;

    [HideInInspector] public Vector2 direction;
    
    private void Awake() {
        parent = transform.parent.gameObject;
        parentManager = parent.GetComponent<IManager>();
        parentManager.DisableManager();
        doneTime = Time.time + flyTime * Time.deltaTime;
        rb = parent.GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (Time.time > doneTime) {
            parentManager.EnableManager();
            Destroy(gameObject);
        }
        else {
            parentManager.DisableManager();
        }
    }

    private void FixedUpdate() {
        if (!pushed) {
            pushed = true;
            rb.AddForce(throwForce*direction/direction.magnitude);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other) {
        var g = other.gameObject;
        int facing;
        facing = direction.x > 0 ? 1 : 0;
        
        // The tag should be changed in the inspector so that in can work on players too.
        // Or not. Could be interesting if enemies could friendly fire with this?
        if (g != parent && g.CompareTag("Enemy")) {
            g.GetComponent<TakeDamage>().Damage(damage, facing);
        }
    }
}
