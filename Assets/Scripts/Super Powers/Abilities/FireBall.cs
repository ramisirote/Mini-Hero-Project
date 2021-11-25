using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : Ability
{
    [SerializeField] private GameObject fireBallProjectile;
    [SerializeField] private Material material;

    [Header("Projectile")] 
    [SerializeField] private float speed;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float hitDamage;
    [SerializeField] private float dotTicks;
    [SerializeField] private float damageAmountOverTime;
    [SerializeField] private float extraDamagePerSecond;
    [SerializeField] private float radius;
    [SerializeField] private float bonFireLifeTime;
    
    private Transform armEffect;
    private Vector3 directionToTarget;
    private BodyAngler bodyAngler;
    private Animator _animator;
    private bool throwing;

    protected override void OnDamageTaken(object o, float damageAmount) {
        if(AbilityOn) SetAbilityOff();
    }


    protected override void AdditionalInit() {
        armEffect = parentCharacter.GetComponent<EffectPoints>().GetPointTransform(Refarences.EBodyParts.ArmR);
        bodyAngler = parentCharacter.GetComponent<BodyAngler>();
        _animator = parentCharacter.GetComponent<Animator>();
    }

    public override void UseAbility(Vector3 direction) {

        if (!AbilityOn) {
            AbilityOn = true;
            AbilityOnInvoke();
            _animator.SetTrigger(AnimRefarences.Blast);
            directionToTarget = Manager.GetDirectionToTarget();
            Manager.FaceTarget();
            Manager.DisableFlip();
            direction = Manager.GetDirectionToTarget();
            var angle = -Controller.GetFacingMult()* Mathf.Atan2(direction.y, direction.x) *
                Mathf.Rad2Deg + 90 + 90*Controller.GetFacingMult();
            bodyAngler.RotatePart(Refarences.EBodyParts.ArmR, angle);
            throwing = true;
        }
    }

    public override void AnimationTrigger() {
        if (throwing) {
            if(Controller.IsGrounded())Controller.StopHorizontal();
            var fireBallInstance = Instantiate(fireBallProjectile);
            fireBallInstance.transform.position = armEffect.position;
            var bolt = fireBallInstance.GetComponent<FireBallProjectile>();
            bolt.Init(directionToTarget, speed, layerMask, hitDamage, dotTicks, radius, damageAmountOverTime, 
                extraDamagePerSecond, material, Colors, bonFireLifeTime);
            throwing = false;
        }
        else {
            SetAbilityOff();
        }
    }

    public override void SetAbilityOff() {
        bodyAngler.ResetAngle(Refarences.EBodyParts.ArmR);
        AbilityOn = false;
        NextCanUse = Time.time + abilityCooldown;
        throwing = false;
        AbilityOffInvoke();
    }
}
