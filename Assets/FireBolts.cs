using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBolts : Ability
{
    [SerializeField] private GameObject fireBolt;
    [SerializeField] private int numberOfBolts;
    [SerializeField] private Material material;

    [Header("Projectile")] 
    [SerializeField] private float speed;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float hitDamage;
    [SerializeField] private float dotTicks;
    [SerializeField] private float damageAmountOverTime;
    [SerializeField] private float extraDamagePerSecond;

    private int _boltsThrown;
    private Transform armEffect;
    private Vector3 directionToTarget;
    private BodyAngler bodyAngler;
    private Animator _animator;
    private bool throwing;
    
    
    protected override void AdditionalInit() {
        armEffect = parentCharacter.GetComponent<EffectPoints>().GetPointTransform(Refarences.EBodyParts.ArmR);
        bodyAngler = parentCharacter.GetComponent<BodyAngler>();
        _animator = parentCharacter.GetComponent<Animator>();
    }

    public override void UseAbility(Vector3 direction) {

        if (!AbilityOn) {
            AbilityOn = true;
            AbilityOnInvoke();
        }
        if (AbilityOn && _boltsThrown < numberOfBolts && !throwing) {
            Controller.StopHorizontal();
            _animator.SetTrigger(AnimRefarences.Blast);
            directionToTarget = Manager.GetDirectionToTarget();
            Manager.FaceTarget();
            Manager.DisableFlip();
            direction = Manager.GetDirectionToTarget();
            var angle = -Controller.GetFacingMult()* Mathf.Atan2(direction.y, direction.x) *
                                                         Mathf.Rad2Deg + 90 + 90*Controller.GetFacingMult();
            Debug.Log(angle);
            bodyAngler.RotatePart(Refarences.EBodyParts.ArmR, angle);
            _boltsThrown++;
            throwing = true;
        }
    }

    public override void AnimationTrigger() {
        if (throwing) {
            Controller.StopHorizontal();
            var fireBoltInstance = Instantiate(fireBolt);
            fireBoltInstance.transform.position = armEffect.position;
            var bolt = fireBoltInstance.GetComponent<FireBolt>();
            bolt.Init(directionToTarget, speed, layerMask, hitDamage, dotTicks, damageAmountOverTime, 
                extraDamagePerSecond, material, Colors);
            throwing = false;
        }
        else {
            bodyAngler.ResetAngle(Refarences.EBodyParts.ArmR);
            if (_boltsThrown >= numberOfBolts) {
                SetAbilityOff();
            }
        }
    }

    public override void SetAbilityOff() {
        AbilityOn = false;
        _boltsThrown = 0;
        NextCanUse = Time.time + abilityCooldown;
        throwing = false;
        AbilityOffInvoke();
    }
}
