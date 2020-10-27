using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float pushForce;


    private void OnCollisionEnter2D(Collision2D other) {
        var damager = other.gameObject.GetComponent<TakeDamage>();
        if (damager) {
            Vector2 vectorToOther = other.transform.position - transform.position;
            damager.Damage(damage, vectorToOther.normalized*pushForce);
        }
    }
}
