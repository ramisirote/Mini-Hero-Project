using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBurst : BurstAbility
{
    [SerializeField] private float damage;
    [SerializeField] private GameObject onFire;
    [SerializeField] private float pushAmount;
        
    protected override void SetUpParticleColors() {
        var particleSystemRendererBlast = blastParticle.GetComponent<ParticleSystemRenderer>();
        Utils.SetUpSpriteRenderedShaderColors(particleSystemRendererBlast, Colors);
        var particleSystemRendererCharge = chargeParticle.GetComponent<ParticleSystemRenderer>();
        Utils.SetUpSpriteRenderedShaderColors(particleSystemRendererCharge, Colors);
    }

    protected override void DoHitEffect(Collider2D hit) {
        HitManager.GetTakeDamage(hit.gameObject)?.Damage(damage, parentCharacter.transform.position, pushAmount);
        OnFire.MakeOnFire(hit.gameObject, onFire, Colors);
    }

    protected override void AdditionalUseAbility() {
        return;
    }

    protected override void AdditionalSetOff() {
        return;
    }
}
