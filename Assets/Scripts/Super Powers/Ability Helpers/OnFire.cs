using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * When a fire power sets someone on fire, this game object is added to them.
 * This deals damage over time, and can extend its duration and increase the damage amount.
 * While dealing damage over time, plays a fire particle.
 *
 * Can search for this script in the children of a character to tell if they are already have one.
 * Can tell if this is dealing damage by calling IsTicking().
 */
public class OnFire : MonoBehaviour
{
    [SerializeField] private ParticleSystem particleSys;
    [SerializeField] private ParticleSystemRenderer particleSystemRenderer;
    [SerializeField] private AudioSource audioSource;
    private float _damageAmount;
    private GameObject parent;
    private TakeDamage _damager;

    public static void MakeOnFire(GameObject otherGameObject, float dotTicks, GameObject onFireGO,
            float extraDamagePerSecond, float damageAmountOverTime, Material material) {
        var takeDamage = HitManager.GetTakeDamage(otherGameObject);
        if (takeDamage==null) return;
        var onFire = otherGameObject.GetComponentInChildren<OnFire>();
        if (onFire) {
            if (onFire.IsTicking()) {
                onFire.ExtendFireDuration(Time.fixedDeltaTime*2);
                onFire.AddDamageAmount(extraDamagePerSecond*Time.fixedDeltaTime*2);
            }
            else {
                onFire.ExtendFireDuration(dotTicks);
                onFire.SetDamageAmount(damageAmountOverTime);
            }
        }
        else if(onFireGO){
            var onFireGOInst = Instantiate(onFireGO, otherGameObject.transform);
            onFire = onFireGOInst.GetComponent<OnFire>();
            onFire.SetDamageAmount(damageAmountOverTime);
            onFire.ExtendFireDuration(dotTicks);
            onFire.SetFireMaterial(material);
        }
    }


    private void Awake() {
        parent = transform.parent.gameObject;
        _damager = (TakeDamage)HitManager.GetTakeDamage(parent.gameObject);
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
