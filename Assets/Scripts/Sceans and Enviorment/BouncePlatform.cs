using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncePlatform : MonoBehaviour
{
    [SerializeField] private float bounceForce;
    [SerializeField] private Animator animator;

    private bool _active;

    private void OnCollisionEnter2D(Collision2D other) {
        if (!_active) return;
        
        CharacterController2D controler = other.gameObject.GetComponent<CharacterController2D>();
        if (controler) {
            animator.SetTrigger("Bounce");
            controler.Push(0f, bounceForce);
            _active = false;
        }
    }

    public void MakeActive() {
        _active = true;
    }
}
