using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Telekanisis : Ability
{
    [SerializeField] private float heldMoveForce;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private GameObject thrower;
    [SerializeField] private float throwerDamage;

    private Animator _animator;
    private BodyAngler _bodyAngler;
    private Transform effectPointTransform;
    private Transform _arm;
    private float _armDefaultRotationAngle;
    
    private GameObject _heldObject;
    private Rigidbody2D _heldRigidBody;
    private IManager _heldManager;
    private float _heldDefaultGravity;

    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        SetUpArmAngler();
        
    }
    
    void SetUpArmAngler() {
        var ep = parentCharacter.GetComponent<EffectPoints>();
        effectPointTransform = ep.GetPointTransform(Refarences.EBodyParts.ArmR);
        var upperArm = ep.GetJointObject(Refarences.BodyJoints.ArmRUpper);
        if (upperArm) {
            _arm = upperArm.transform.parent;
        }
        if (_arm) _armDefaultRotationAngle = _arm.localRotation.eulerAngles.z;
        _bodyAngler = parentCharacter.GetComponent<BodyAngler>();
        transform.parent = effectPointTransform;
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            _animator.SetBool(AnimRefarences.IsFireingContinues, true);

            AbilityOn = true;
            Manager.DisableActions();

            if (!FindHeldTarget()) {
                SetAbilityOff();
            }
            
            AbilityOnInvoke();
        }
    }

    private bool FindHeldTarget() {
        var directionToTarget = Manager.GetDirectionToTarget();
        var rayHit = Physics2D.Raycast(effectPointTransform.position, directionToTarget,100, targetLayer);
        if (rayHit && rayHit.collider) {
            _heldObject = rayHit.collider.gameObject;
            StartHold();
            return true;
        }

        return false;
    }
    
    public override void UseAbilityRelease(Vector3 direction) {
        if (!AbilityOn) return;
        SetAbilityOff();
    }

    private void StartHold() {
        _heldRigidBody = _heldObject.GetComponent<Rigidbody2D>();
        _heldManager = _heldObject.GetComponent<IManager>();
        _heldManager?.Stunned(true);
        _heldDefaultGravity = _heldRigidBody.gravityScale;
        _heldRigidBody.gravityScale = 0f;
    }

    public override void SetAbilityOff() {
        if (!AbilityOn) return;

        EndHold();
        _animator.SetBool(AnimRefarences.IsFireingContinues, false);
        _bodyAngler.RotatePart(_arm, _armDefaultRotationAngle);
        AbilityOn = false;
        Manager.EnableActions();

        AbilityOffInvoke();
    }

    private void EndHold() {
        if(!_heldObject) return;
        _heldManager?.Stunned(false);
        
        _heldRigidBody.gravityScale = _heldDefaultGravity;

        var throwerInstance = Instantiate(thrower, _heldObject.transform);
        throwerInstance.GetComponent<StrengthThrower>().Init(targetLayer, throwerDamage, 0.3f, 
            heldMoveForce*20, Manager.GetDirectionToTargetFromOther(_heldObject.transform.position), 
            true);
        
        _heldManager = null;
        _heldRigidBody = null;
        _heldObject = null;
    }

    private void Update() {
        if (_heldObject) {
            MoveHeldTarget();
        }
    }

    private void MoveHeldTarget() {
        
        _heldRigidBody.AddForce(Manager.GetDirectionToTargetFromOther(_heldObject.transform.position).normalized*heldMoveForce);
    }
    
    public override void UpdateDirection(Vector3 direction) {
        float angle;
        Manager.FaceTarget();
        angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 )* -Controller.GetFacingMult();
        
        if (_bodyAngler && _arm) {
            _bodyAngler.RotatePart(_arm, angle);
        }
    }
}
