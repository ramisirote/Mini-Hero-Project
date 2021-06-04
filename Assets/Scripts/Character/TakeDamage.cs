﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class TakeDamage : MonoBehaviour, ITakeDamage
{
    [FormerlySerializedAs("stats")] [SerializeField] private CharacterStats characterStats;
    [SerializeField] private CharacterController2D controller2D;
    [SerializeField] private float pushBackForce = 20f;
    [SerializeField] private float hitInvonerableTime;
    [SerializeField] private Animator animator;
    [SerializeField] private SoundManager soundManager;
    
    public event EventHandler<IManager> OnDeathEvent;
    public event EventHandler<float> OnDamage;

    private float _nextCanBeHitTime = 0f;
    private Coroutine _dotRoutine = null;
    private float _dotTicks;

    private float _dotDamage = 0;

    private IManager _manager;

    private bool _playAnimation = true;

    private SpriteHandler _spriteHandler;

    private void Start() {
        HitManager.AddTakeDamage(gameObject, this);
        _manager = GetComponent<IManager>();
        _spriteHandler = GetComponent<SpriteHandler>();
    }


    public void Damage(float damage, int pushDirectionMult, float pushForceUp = 70f, float pushForceSide = 70f,
        bool ignoreInvonerable = false) {
        if (characterStats.IsDead()) return;
        if (!ignoreInvonerable && Time.time < _nextCanBeHitTime) return;
        
        controller2D.StopHorizontal();
        controller2D.Push(pushForceSide * pushDirectionMult, pushForceUp);

        characterStats.GetCharacterStats()?.ChangeHpBy(-1*damage);
        StartCoroutine(HitRecolor());
        if(gameObject.CompareTag("Player")) CinemachineShake.Instance.ShakeCamera();
        soundManager.PlayAudio(SoundManager.SoundClips.TakeDamage);
        
        OnDamage?.Invoke(this, damage);
            
        if (characterStats.IsDead()) {
            Die();
        }
        else if(_playAnimation && !_manager.IsStunned()){
            animator.SetTrigger(AnimRefarences.Hit);
            _manager.DisableManager();
        }
            
        _nextCanBeHitTime = Time.time + hitInvonerableTime;
    }
    
    public void Damage(float damage, Vector2 awayFromPos, float pushAmount, bool ignoreInvonerable = false) {
        Vector2 pos = transform.position;
        var toHitVec = (pos - awayFromPos).normalized * pushAmount;
        Damage(damage, toHitVec, ignoreInvonerable);
    }

    public void Damage(float damage, Vector2 push, bool ignoreInvonerable = false) {
        if (characterStats.IsDead()) return;
        if (!ignoreInvonerable && Time.time < _nextCanBeHitTime) return;
        
        controller2D.StopHorizontal();
        controller2D.Push(push);

        characterStats.GetCharacterStats()?.ChangeHpBy(-1*damage);
        StartCoroutine(HitRecolor());
        soundManager.PlayAudio(SoundManager.SoundClips.TakeDamage);
        
        OnDamage?.Invoke(this, damage);
            
        if (characterStats.IsDead()) {
            Die();
        }
        else if(_playAnimation){
            animator.SetTrigger(AnimRefarences.Hit);
            _manager.DisableManager();
        }
            
        _nextCanBeHitTime = Time.time + hitInvonerableTime;
    }

    public void DamageOverTime(float damage, float ticks) {
        if (characterStats.IsDead()) return;
        
        _dotTicks += ticks;
        _dotDamage = damage;
        
        if (_dotRoutine == null) {
            _dotRoutine = StartCoroutine(EDamageOverTime());
        }
    }

    IEnumerator EDamageOverTime() {
        while (_dotTicks >= 1) {
            _dotTicks--;
            characterStats.GetCharacterStats()?.ChangeHpBy(-1*_dotDamage);
            StartCoroutine(HitRecolor());
            
            if (characterStats.IsDead()) {
                _dotTicks = 0;
                Die();
                break;
            }
            yield return new WaitForSeconds(0.5f);
        }

        _dotDamage = 0;
        _dotRoutine = null;
    }

    private void Update() {
        if (_dotTicks >= 1) {
            
        }
    }

    public bool IsDotTicking() {
        return _dotTicks >= 1;
    }

    public void SetPlayAnimatoin(bool shouldPlay) {
        _playAnimation = shouldPlay;
    }

    IEnumerator HitRecolor() {
        if (_spriteHandler) {
            _spriteHandler.ColorizeAllSprites(new Color(1f,0.7f, 0.7f, 1f));
            
            yield return new WaitForSeconds(0.1f);
            
            _spriteHandler.ColorizeAllSprites(Color.white);
        }
        yield return null;
    }

    private void Die() {
        var ob = gameObject;
        OnDeathEvent?.Invoke(ob, _manager);
        controller2D.MakeFlying(false);
        controller2D.StopAll();
        controller2D.enabled = false;
        AnimRefarences.ResetAnimatorBools(animator);
        animator.SetBool(AnimRefarences.Dead, true);
        soundManager.PlayAudio(SoundManager.SoundClips.Die);
        enabled = false;
        _manager.PermanentDisable();
        ob.layer = LayerMask.NameToLayer("Dead");
        
        HitManager.RemoveTakeDamage(ob);
    }
    
}

