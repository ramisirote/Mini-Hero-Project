using UnityEngine;

/*
 * Replaces the basic attack with a super strength one.
 * This new attack deals more damage and pushes enemies a bunch.
 * Enemies pushed this way deal damage to enemies they hit on the way.
 */
public class SuperStrength : Ability
{
    [SerializeField] private float damageMult;
    [SerializeField] private float sizeMult;
    [SerializeField] private float headDownSet;
    [SerializeField] private GameObject thrower;
    [SerializeField] private SuperStrangthAttack strengthAttack;
    [SerializeField] private float attackCooldown;
    [SerializeField] private float cameraShake;
    private EffectPoints _effectPoints;
    private Transform torso;
    private Transform head;
    private AttackManagerBase _attackDefault;
    
    protected override void AdditionalInit() {
        _effectPoints = parentCharacter.GetComponent<EffectPoints>();
        if (_effectPoints) {
            torso = _effectPoints.GetJointObject(Refarences.BodyJoints.ChestLower).transform.parent;
            head = _effectPoints.GetJointObject(Refarences.BodyJoints.Head).transform.parent;
        }

        _attackDefault = parentCharacter.GetComponent<AttackManagerBase>();
        strengthAttack.Init(parentCharacter.GetComponent<Animator>(), Controller, CharacterStats,
                            Manager, attackCooldown, thrower, damageMult, cameraShake, IsPlayer);
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            SetPowerOn();
        }
        else {
            SetAbilityOff();
        }
    }

    private void SetPowerOn() {
        AbilityOn = true;
        if (torso) {
            torso.localScale *= sizeMult;
            
            head.localScale /= sizeMult;
            
            var torsoPos = torso.transform.position;
            torsoPos.y += headDownSet;
            torso.transform.position = torsoPos;
            
            var headPos = head.transform.position;
            headPos.y -= headDownSet;
            head.transform.position = headPos;
        }

        Manager.SetAttackManager(strengthAttack);
        AbilityOnInvoke();
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;
        AbilityOn = false;
        if (torso) {
            var headPos = head.transform.position;
            headPos.y += headDownSet;
            head.transform.position = headPos;
            
            var torsoPos = torso.transform.position;
            torsoPos.y -= headDownSet;
            torso.transform.position = torsoPos;
            
            head.localScale *= sizeMult;
            
            torso.localScale /= sizeMult;
        }
        
        Manager.SetAttackManager(_attackDefault);
        AbilityOffInvoke();
    }
}
