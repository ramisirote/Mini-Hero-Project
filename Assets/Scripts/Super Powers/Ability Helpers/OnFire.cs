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
    [SerializeField] private float damagePerSecond;
    [SerializeField] private float seconds;
    [SerializeField] private float stackMultiplier;
    [SerializeField] private ParticleSystem particleSys;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Material material;
    
    private float _damageAmount;
    private GameObject parent;
    private TakeDamage _damager;
    private ParticleSystemRenderer particleSystemRenderer;

    public static void MakeOnFire(GameObject target, GameObject onFirePrefab, Color[] colors, bool scale=false) {
        var takeDamage = HitManager.GetTakeDamage(target);
        if (takeDamage==null) return;
        var mult = scale ? Time.deltaTime*2 : 1;
        var onFire = target.GetComponentInChildren<OnFire>();
        if (onFire) {
            if (onFire.IsTicking()) {
                onFire.ExtendFireDuration(onFire.seconds*mult);
                onFire.AddDamageAmount(onFire._damageAmount*onFire.stackMultiplier*mult);
            }
            else {
                onFire.SetDamageAmount(onFire.damagePerSecond);
                onFire.ExtendFireDuration(onFire.seconds);
            }
        }
        else if(onFirePrefab){
            var onFireGOInst = Instantiate(onFirePrefab, target.transform);
            onFire = onFireGOInst.GetComponent<OnFire>();
            if(!onFire) return;
            onFire.SetDamageAmount(onFire.damagePerSecond);
            onFire.ExtendFireDuration(onFire.seconds);
            onFire.SetFireColors(colors);
        }
    }


    private void Awake() {
        parent = transform.parent.gameObject;
        _damager = (TakeDamage)HitManager.GetTakeDamage(parent.gameObject);
        _damageAmount = damagePerSecond;
        particleSystemRenderer = particleSys.GetComponent<ParticleSystemRenderer>();
        particleSystemRenderer.material = material;
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


    public void SetFireColors(Color[] colors) {
        Utils.SetUpSpriteRenderedShaderColors(particleSystemRenderer, colors);
    }

    public bool IsTicking() {
        return _damager && _damager.IsDotTicking();
    }
}
