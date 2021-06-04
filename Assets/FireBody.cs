using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBody : Ability
{
    [SerializeField] private float timer;
    [SerializeField] private ParticleSystem fireParticles;
    [SerializeField] private float energyRequierdPerSecond;
    [SerializeField] private float damageAmountOverTime;
    [SerializeField] private float extraDamagePerSecond;
    [SerializeField] private int dotTicks;
    [SerializeField] private Material material;
    [SerializeField] private ParticleSystemRenderer particleSystemRenderer;
    [SerializeField] private string[] hitTags;
    [SerializeField] private GameObject onFireGameObject;
    private Collider2D _collider2D;
    private SpriteHandler _spriteHandler;

    protected override void AdditionalInit() {
        SetUpParticles();
        _spriteHandler = parentCharacter.GetComponent<SpriteHandler>();
    }

    private void SetUpParticles() {
        particleSystemRenderer.material = material;
        
        var m = particleSystemRenderer.material;
        m.SetColor("_Color1", _color1);
        m.SetColor("_Color2", _color2);
        m.SetColor("_Color3", _color3);
        
        fireParticles.Stop();
    }

    public override void OnAbilitySwitchIn() {
        if(AbilityOn) AbilityOnInvoke();
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            AbilityOn = true;
            fireParticles.Play();
            StartCoroutine(TurnOffAfterTime());
            AbilityOnInvoke();
        }
    }

    IEnumerator TurnOffAfterTime() {
        yield return new WaitForSeconds(timer);
        AbilityOff();
    }

    private void AbilityOff() {
        if (!AbilityOn) return;
        AbilityOn = false;
        fireParticles.Stop();
        _spriteHandler.ColorizeAllSprites(Color.white);
        AbilityOffInvoke();
    }

    public override void SetAbilityOff() {
        AbilityOffInvoke();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!AbilityOn) { return; }
        GameObject otherGameObject = other.gameObject;
        bool hasHitTag = false;
        foreach (var tags in hitTags) {
            if (otherGameObject.CompareTag(tags)) {
                hasHitTag = true;
                break;
            }
        }
        if(!hasHitTag) return;

        OnFire.MakeOnFire(otherGameObject, dotTicks, onFireGameObject, extraDamagePerSecond, 
            damageAmountOverTime, particleSystemRenderer.material);
    }
    
    private void OnTriggerStay2D(Collider2D other) {
        GameObject otherGameObject = other.gameObject;
        bool hasHitTag = false;
        foreach (var tags in hitTags) {
            if (otherGameObject.CompareTag(tags)) {
                hasHitTag = true;
                break;
            }
        }
        if(!hasHitTag) return;
        
        OnFire.MakeOnFire(otherGameObject, dotTicks, onFireGameObject, extraDamagePerSecond, 
            damageAmountOverTime, particleSystemRenderer.material);
    }

    // Update is called once per frame
    void Update()
    {
        if(AbilityOn) _spriteHandler.ColorizeAllSprites(_color2);
    }
}
