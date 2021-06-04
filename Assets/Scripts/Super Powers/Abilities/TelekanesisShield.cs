using UnityEngine;

public class TelekanesisShield : Ability
{
    [SerializeField] private GameObject shieldObject;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float distance;

    private BodyAngler _angler;
    private Animator _animator;
    private ProjectileShield shield;
    private Refarences.EBodyParts _bodyPart = Refarences.EBodyParts.ArmR;
    
    // Upgrades
    [Header("Upgrades")] 
    [SerializeField] private float reducedEnergy;
    [SerializeField] private float reducedCooldown;
    [SerializeField] private float reducedTimer;
    private bool _reducedEnergy;
    private bool _reducedCooldown;
    private bool _staticShields = false;

    protected override void UnlockAbilityMap() {
        _reducedEnergy = upgrades[0];
        _reducedCooldown = upgrades[1];
        _staticShields = upgrades[2];
        abilityCooldown = _reducedCooldown ? reducedCooldown : abilityCooldown;
        energyRequired = _reducedEnergy ? reducedEnergy : energyRequired;
    }

    protected override void OnDamageTaken(object o, float damageAmount) {
        if (AbilityOn) {
            SetAbilityOff();
        }
    }

    protected override void AdditionalInit() {
        _angler = parentCharacter.GetComponent<BodyAngler>();
        _animator = parentCharacter.GetComponent<Animator>();
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            AbilityOn = true;
            Manager.DisableActions();
            InitShield();
            _animator.SetBool(AnimRefarences.IsFireingContinues, true);
            AbilityOnInvoke();
        }
    }

    private void InitShield() {
        var shieldGo = Instantiate(shieldObject);
        if (shield) {
            DisableShield();
        }
        shield = shieldGo.GetComponent<ProjectileShield>();
        Manager.FaceTarget();
        var directionToTarget = Manager.GetDirectionToTarget();
        var pos = parentCharacter.transform.position + directionToTarget.normalized*distance;
        var angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
        shield.Init(layer, pos, Vector3.forward*angle);
    }

    private float GetAngleToTarget(Vector3 direction) {
        var angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90 )* -Controller.GetFacingMult();
        return angle;
    }

    private void DisableShield() {
        shield.Disable();
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;

        _animator.SetBool(AnimRefarences.IsFireingContinues, false);
        _angler.ResetAngle(_bodyPart);
        
        Manager.EnableActions();
        DisableShield();
        AbilityOn = false;
        AbilityOffInvoke();
    }

    public override void UseAbilityRelease(Vector3 direction) {
        if (AbilityOn) {
            SetAbilityOff();
        }
    }
    
    public override void UpdateDirection(Vector3 direction) {
        if (!shield) return;
        Manager.FaceTarget(shield.transform);
        var position = parentCharacter.transform.position;
        float angle = GetAngleToTarget(shield.transform.position - position);
        var shieldPos = position + direction.normalized * distance;
        var shieldAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        shield.UpdateLocation(shieldPos, Vector3.forward*shieldAngle);

        // var transformLocalRotation = transform.localRotation;
        // transformLocalRotation.eulerAngles = new Vector3(0,0,angle);
        
        if (_angler) {
            _angler.RotatePart(_bodyPart, angle);
        }
    }
}
