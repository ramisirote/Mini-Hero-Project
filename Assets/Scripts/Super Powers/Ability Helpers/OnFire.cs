using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSys;
    [SerializeField] private ParticleSystemRenderer particleSystemRenderer;
    [SerializeField] private AudioSource audioSource;
    private float _damageAmount;
    private GameObject parent;
    private TakeDamage _damager;
    
    
    private void Awake() {
        parent = transform.parent.gameObject;
        _damager = parent.GetComponent<TakeDamage>();
    }

    public void ExtendFireDuration(float ticks) {
        if (_damager && _damager.isActiveAndEnabled) {
            particleSys.Play();
            audioSource.Play();
            _damager.DamageOverTime(_damageAmount, ticks);
        }
    }

    public void SetDamageAmount(float newAmount) {
        if (_damager) {
            _damageAmount = newAmount;
        }
    }

    public void AddDamageAmount(float addDamage) {
        _damageAmount += addDamage;
    }

    private void Update() {
        if (!_damager || !_damager.IsDotTicking() || !_damager.isActiveAndEnabled) {
            particleSys.Stop();
            audioSource.Stop();
        }

        transform.position = parent.transform.position;
    }


    public void SetFireMaterial(Material m) {
        particleSystemRenderer.material = m;
    }

    public bool IsTicking() {
        return _damager && _damager.IsDotTicking();
    }
}
