using UnityEngine;

/*
 * Circle area of damage around you.
 * Part of the Energy Power class.
 * Strengths: Area of effect, knock-back.
 * Weaknesses: Low damage, charge time, high energy cost per-damage, short disable.
 *
 * This ability uses the animation trigger to tell when to do the burst.
 */
public class EnergyBurst : BurstAbility
{
    [SerializeField] private float damage;
    [SerializeField] private float knockBack;
    
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


    protected override void SetUpParticleColors() {
        var col = blastParticle.colorOverLifetime;
        col.color = ColorGradientBurst();

        var chargeCol = chargeParticle.colorOverLifetime;
        chargeCol.color = ColorGradientCharge();
    }

    protected override void DoHitEffect(Collider2D hit) {
        HitManager.GetTakeDamage(hit.gameObject)?.Damage(damage, parentCharacter.transform.position, knockBack);
    }

    protected override void AdditionalUseAbility() {
        return;
    }

    protected override void AdditionalSetOff() {
        return;
    }
}
