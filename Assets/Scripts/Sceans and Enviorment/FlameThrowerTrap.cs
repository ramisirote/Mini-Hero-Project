using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameThrowerTrap : MonoBehaviour
{
    [SerializeField] private Collider2D collider2D;
    [SerializeField] private ParticleSystem particleSystem;
    [SerializeField] private ParticleSystemRenderer particleSystemRenderer;
    [SerializeField] private Material material;
    [SerializeField] private float DelayOn;
    [SerializeField] private float fireTurnOnTime;
    [SerializeField] private float fireOnTime;
    [SerializeField] private float fireOffTime;
    [SerializeField] private float damage;
    [SerializeField] private float pushForce;
    [SerializeField] private Color[] colors = new Color[3];

    private Coroutine cycleCoroutine;

    private void OnEnable() {
        if(cycleCoroutine!=null) StopCoroutine(cycleCoroutine);
        cycleCoroutine = StartCoroutine(Cycle());
        SetUpParticleSystem();
    }

    private void SetUpParticleSystem() {

        particleSystemRenderer.material = material;
        
        var m = particleSystemRenderer.material;
        m.SetColor("_Color1", colors[0]);
        m.SetColor("_Color2", colors[1]);
        m.SetColor("_Color3", colors[2]);
    }

    private IEnumerator Cycle() {
        yield return new WaitForSeconds(DelayOn);
        while (true) {
            particleSystem.Play();
            yield return new WaitForSeconds(fireTurnOnTime);
            collider2D.enabled = true;
            yield return new WaitForSeconds(fireOnTime);

            collider2D.enabled = false;
            particleSystem.Stop();
            
            yield return new WaitForSeconds(fireOffTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        var damager = HitManager.GetTakeDamage(other.gameObject);
        if (damager != null) {
            Vector2 vectorToOther = other.transform.position - transform.position;
            damager.Damage(damage, vectorToOther.normalized*pushForce);
        }
    }
    
    private void OnTriggerStay2D(Collider2D other) {
        var damager = HitManager.GetTakeDamage(other.gameObject);
        if (damager != null) {
            Vector2 vectorToOther = other.transform.position - transform.position;
            damager.Damage(damage, vectorToOther.normalized*pushForce);
        }
    }

    private void OnDestroy() {
        if(cycleCoroutine!=null) StopCoroutine(cycleCoroutine);
    }
}
