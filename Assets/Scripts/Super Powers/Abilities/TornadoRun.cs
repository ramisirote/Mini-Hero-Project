using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoRun : Ability
{
    [SerializeField] private BoxCollider2D areaCollider;
    [SerializeField] private Collider2D tornadoCollider;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private GameObject trailObject;
    [SerializeField] private GameObject tornadoImageObject;
    [SerializeField] private SpriteRenderer tornadoSprite;
    [SerializeField] private float minOnForTime = 0.5f;
    [SerializeField] private float damage;
    [SerializeField] private Vector2 push;
    [SerializeField] private LayerMask whatToHit;

    private float fadeTime = 0.1f;
    
    private Animator _animator;
    private float abilityOffTimer;
    private TakeDamage _takeDamage;
    private bool _canTurnOff;
    private float _rayLength;
    private TrailRenderer _trailRenderer;
    private Transform _chestEffectPoint;

    private float _move;

    private bool abilityReleased;
    
    
    protected override void AdditionalInit() {
        areaCollider.enabled = false;
        _animator = parentCharacter.GetComponent<Animator>();
        _takeDamage = parentCharacter.GetComponent<TakeDamage>();
        _rayLength = areaCollider.size.x / 2;
        
        _chestEffectPoint = parentCharacter.GetComponent<EffectPoints>().GetPointTransform(Refarences.EBodyParts.Chest);
        trailObject.transform.parent = _chestEffectPoint;
        trailObject.transform.position = _chestEffectPoint.position;
        
        _trailRenderer = trailObject.GetComponent<TrailRenderer>();
        _trailRenderer.enabled = false;
        _trailRenderer.colorGradient = Utils.CreateGradient(new[] {_color1, _color2, _color3},
            new[] {0.8f, 0.7f, 0.0f});
        
        tornadoImageObject.SetActive(false);
        Utils.SetUpSpriteRenderedShaderColors(tornadoSprite, new[] {_color1, _color2, _color3});

        tornadoCollider.enabled = false;
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired)) {
            AbilityOn = true;
            Manager.DisableActions();
            Manager.DisableFlip();
            areaCollider.enabled = true;
            abilityOffTimer = Time.time + minOnForTime;
            _animator.SetBool(AnimRefarences.TornadoRun, true);
            _takeDamage.SetPlayAnimatoin(false);
            AbilityOnInvoke();
            tornadoImageObject.SetActive(true);
            _trailRenderer.enabled = true;
            tornadoCollider.enabled = true;
            CharacterStats.DisableEnergyRecharge(true);
            abilityReleased = false;
            StartCoroutine(FadeAlphaIn());
        }
        
    }

    private IEnumerator FadeAlphaIn() {
        var clear = new Color(1,1,1,0);
        tornadoSprite.color = clear;
        for(float t=0.01f; t<fadeTime; t+=0.01f)
        {
            tornadoSprite.color = Color.Lerp(clear, Color.white, t/fadeTime);
            yield return null;
        }
    }

    private IEnumerator FadeAlphaOut() {
        var clear = new Color(1,1,1,0);
        tornadoSprite.color = Color.white;
        for(float t=0.01f; t<fadeTime; t+=0.01f)
        {
            tornadoSprite.color = Color.Lerp(Color.white, clear , t/fadeTime);
            yield return null;
        }
        
        tornadoImageObject.SetActive(false);
    }

    public override void UseAbilityRelease(Vector3 direction) {
        abilityReleased = true;
    }

    private void Update() {
        if (AbilityOn && Time.time > abilityOffTimer && abilityReleased) {
            _canTurnOff = true;
        }

        if (AbilityOn) {
            if (!CharacterStats.UseEnergy(energyRequired*Time.deltaTime)) {
                _canTurnOff = true;
                return;
            }
            
            // Make sure that the player isn't in a wall when running
            var hit = Physics2D.Raycast(transform.position, Vector2.right, _rayLength, whatIsGround);
            if (hit.collider) {
                _move = -5f;
            }
            else {
                hit = Physics2D.Raycast(transform.position, Vector2.left, _rayLength, whatIsGround);
                if (hit.collider) {
                    _move = 5f;
                }
                else {
                    _move = 0;
                }
            }
        }
    }

    public override void AnimationTrigger() {
        if(_canTurnOff) SetAbilityOff();
    }

    private void FixedUpdate() {
        if (AbilityOn) {
            Controller.Move(_move, false, false);
        }
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        NextCanUse = Time.time + abilityCooldown;
        AbilityOffInvoke();
        StartCoroutine(FadeAlphaOut());
        _canTurnOff = false;
        Manager.EnableManager();
        _animator.SetBool(AnimRefarences.TornadoRun, false);
        _takeDamage.SetPlayAnimatoin(true);
        AbilityOn = false;
        areaCollider.enabled = false;
        _trailRenderer.enabled = false;
        
        tornadoCollider.enabled = false;
        CharacterStats.DisableEnergyRecharge(false);
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (Utils.IsObjectInLayerMask(other.gameObject, whatToHit)) {
            var pushAway = Vector2.one;
            pushAway.x = other.transform.position.x > transform.position.x ? -1 : 1;
            other.gameObject.GetComponent<ITakeDamage>()?.Damage(damage, push*pushAway);
        }
    }
}
