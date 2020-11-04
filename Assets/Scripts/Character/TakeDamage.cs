using System;
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
    
    public event EventHandler OnDeathEvent;
    public event EventHandler<float> OnDamage;

    private float _nextCanBeHitTime = 0f;
    private Coroutine _dotRoutine = null;
    private float _dotTicks;

    private float _dotDamage = 0;

    private IManager _manager;

    private bool _playAnimation = true;

    private void Start() {
        _manager = GetComponent<IManager>();
    }


    public void Damage(float damage, int pushDirectionMult, float pushForceUp = 70f, float pushForceSide = 70f) {
        if (characterStats.IsDead()) return;
        if (Time.time < _nextCanBeHitTime) return;
        
        controller2D.StopHorizontal();
        controller2D.Push(pushForceSide * pushDirectionMult, pushForceUp);

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
    
    public void Damage(float damage, Vector2 push) {
        if (characterStats.IsDead()) return;
        if (Time.time < _nextCanBeHitTime) return;
        
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
        var spriteHandler = GetComponent<SpriteHandler>();
        if (spriteHandler) {
            spriteHandler.ColorizeAllSprites(new Color(1f,0.7f, 0.7f, 1f));
            
            yield return new WaitForSeconds(0.1f);
            
            spriteHandler.ColorizeAllSprites(Color.white);
        }
        yield return null;
    }

    private void Die() {
        OnDeathEvent?.Invoke(gameObject, EventArgs.Empty);
        controller2D.MakeFlying(false);
        controller2D.StopAll();
        controller2D.enabled = false;
        AnimRefarences.ResetAnimatorBools(animator);
        animator.SetBool(AnimRefarences.Dead, true);
        soundManager.PlayAudio(SoundManager.SoundClips.Die);
        this.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Dead");
    }
    
}

