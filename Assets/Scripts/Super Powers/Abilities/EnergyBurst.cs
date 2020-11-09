using UnityEngine;

/*
 * Circle area of damage around you.
 * Part of the Energy Power class.
 * Strengths: Area of effect, knock-back.
 * Weaknesses: Low damage, charge time, high energy cost per-damage, short disable.
 *
 * This ability uses the animation trigger to tell when to do the burst.
 */
public class EnergyBurst : Ability
{

    private bool powerActive;
    private Animator _animator;
    [SerializeField] private float damage;
    [SerializeField] private float knockBack;
    [SerializeField] private float blastRadius;
    [SerializeField] private ParticleSystem blastParticle;
    [SerializeField] private ParticleSystem chargeParticle;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private float cameraShake;
    
    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        SetUpParticleColors();
    }
    
    private Gradient ColorGradientBurst() {
        var gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = _color1;
        colorKey[0].time = 0.35f;
        colorKey[1].color = _color2;
        colorKey[1].time = 0.5f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 0.8f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;
        
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }
    
    private Gradient ColorGradientCharge() {
        var gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = _color2;
        colorKey[0].time = 0.35f;
        colorKey[1].color = _color1;
        colorKey[1].time = 0.5f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 0.5f;
        alphaKey[0].time = 1.0f;
        alphaKey[1].alpha = 0.8f;
        alphaKey[1].time = 0.0f;
        
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }


    private void SetUpParticleColors() {
        var col = blastParticle.colorOverLifetime;
        col.color = ColorGradientBurst();

        var chargeCol = chargeParticle.colorOverLifetime;
        chargeCol.color = ColorGradientCharge();
    }
    
    public override void AnimationTrigger() {
        if (!blastParticle) return;

        if (powerActive) {
            SetAbilityOff();
        }
        else {
            ActivateBlast();
        }
    }
    
    private void ActivateBlast() {
        powerActive = true;
        Controller.StopHorizontal();
        chargeParticle.Stop();
        blastParticle.Play();
        
        if(audioSource) audioSource.Play();
        if(IsPlayer) CinemachineShake.Instance.ShakeCamera(cameraShake, 0.5f);
        
        BlastHit();
        
        SetAbilityOff();
        NextCanUse = Time.time + abilityCooldown * Time.deltaTime;
    }

    private void BlastHit() {
        var pos = transform.position;

        var hits = Physics2D.OverlapCircleAll(pos, blastRadius, layersToHit);

        foreach (var hit in hits) {
            var toHitVec = hit.transform.position - pos;
            toHitVec.z = 0;
            toHitVec = (toHitVec / toHitVec.magnitude) * knockBack;
            hit.GetComponent<TakeDamage>()?.Damage(damage, 1, toHitVec.y, toHitVec.x);
        }
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired)) {
            
            Controller.StopHorizontal();
            _animator.SetTrigger(AnimRefarences.Burst);
            
            chargeParticle.Play();

            AbilityOn = true;
            AbilityOnInvoke();
        }
        else {
            AbilityOn = false;
        }
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;

        chargeParticle.Stop();
        chargeParticle.Clear();
        
        powerActive = false;
        
        AbilityOn = false;
        
        AbilityOffInvoke();
    }

    protected override void OnDamageTaken(object o, float damageAmount) {
        SetAbilityOff();
    }

    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }
}
