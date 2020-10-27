using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * DO NOT USE, OLD FUNCTION
 */
public class EnemyAttack : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private CharacterController2D controler;
    [SerializeField] private Transform punch;
    [SerializeField] private LayerMask PlayerLayer;
    [SerializeField] private float hitRadius = 0.04f;
    [SerializeField] float damage = 15;

    private float pushBack = 100f;
    
    private void Start() {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame

    public void Attack() {
        Collider2D[] hits = Physics2D.OverlapCircleAll(punch.position, hitRadius, PlayerLayer);
        foreach (var hit in hits) {
            hit.GetComponent<TakeDamage>().Damage(damage, controler.GetFacingMult());
        }

        if (hits.Length > 0) {
            controler.Push(controler.GetFacingMult()*-20f, 100f);
        }
    }

    public void SetPushbackForce() {
        
    }
    
    void OnDrawGizmosSelected() {
        if (punch == null) return;
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(punch.position, hitRadius);
    }
}
