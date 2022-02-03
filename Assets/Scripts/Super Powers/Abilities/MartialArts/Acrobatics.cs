using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acrobatics : Ability2
{
    public float jumpForce;
    public float minCooldown;
    public LayerMask enemyLayer;
    private Rigidbody2D _rigidbody = null;
    private float minNextCanUseTime;

    private Transform groundPiont;

    private bool _wallReset, _resistance, _attackTargte;
    private bool wasReset = false;
    public LayerMask whatIsGround;
    public float iFramesResistance;
    private Transform hitTarget;
    private AttackManagerBase attackManager;

    protected override void AdditionalInit()
    {
        this._rigidbody = parentCharacter.GetComponent<Rigidbody2D>();
        groundPiont = effectoints.GetPointTransform(Refarences.EBodyParts.ground);
        attackManager = parentCharacter.GetComponent<AttackManagerBase>();
    }

    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _wallReset = upgrades[1];
        _resistance = upgrades[2];
        _attackTargte = upgrades[3];
    }

    public override void AdditionalUseAbility(){
        // start animation
        Vector2 direction = Manager.GetDirectionToTarget();
        direction.Normalize();
        Controller.Push(direction * jumpForce);
        Animator.SetBool(AnimRefarences.Flipping, true);
        wasReset = false;
        Manager.DisableManager();
        Manager.DisableFlip();
        Manager.FaceTarget();
        if(_resistance) CharacterStats.GetCharacterStats().SetMultBuff(CharacterStatsData.StatFields.Resistnce, iFramesResistance);
        if(_attackTargte) findAttackTarget();
    }

    public override void UseAbility(Vector3 direction)
    {
        base.UseAbility();
    }

    protected override void AditionalAbilityOff(){
        Animator.SetBool(AnimRefarences.Flipping, false);
        minNextCanUseTime = Time.time + minCooldown;
        if(_resistance) CharacterStats.GetCharacterStats().SetMultBuff(CharacterStatsData.StatFields.Resistnce, iFramesResistance, remove: true);
    }

    private bool CheckCooldownReset(){
        if(curentState != states.off || wasReset) return false;
        if(Controller.IsGrounded()) return true;

        if(_wallReset){
            RaycastHit2D rayHit = Physics2D.Raycast(groundPiont.position, Controller.GetFacingMult() * Vector2.right, 1f, whatIsGround);
            if(rayHit.collider){
                print("wall detected");
                return true;
            }
        }

        return false;
    }

    private void findAttackTarget(){
        hitTarget = null;
        var targetPosition = transform.position + Manager.GetDirectionToTarget();
        var hit = Physics2D.OverlapCircle(targetPosition, 0.3f, enemyLayer);
        if(hit) hitTarget = hit.transform;
    }

    private void Update() {
        if(CheckCooldownReset()){
            wasReset = true;
            NextCanUse = minNextCanUseTime;
        }
        if(AbilityOn && hitTarget && Vector2.Distance(hitTarget.position, transform.position) < 0.7f){
            Controller.StopAll();
            SetAbilityOff();
            attackManager.Attack(forceAttack: true, animation: AnimRefarences.Punch05);
        }
    }
}
