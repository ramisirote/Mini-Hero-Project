using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TelekaneticBurst : Ability
{
    private bool powerActive;
    private Animator _animator;
    [SerializeField] private float damage;
    [SerializeField] private float knockBack;
    [SerializeField] private float blastRadius;
    // [SerializeField] private ParticleSystem blastParticle;
    // [SerializeField] private ParticleSystem chargeParticle;
    [SerializeField] private LayerMask layersToHit;
    [SerializeField] private float cameraShake;
    [SerializeField] private float levitateAmount;
    [SerializeField] private float levitateTime;
    
    private Dictionary<GameObject, float> objectDefGrav = new Dictionary<GameObject, float>();
    
    protected override void AdditionalInit() {
        _animator = parentCharacter.GetComponent<Animator>();
        abilityCooldown = Mathf.Max(abilityCooldown, levitateTime);
    }

    public override void AnimationTrigger() {
        // if (!blastParticle) return;

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
        // chargeParticle.Stop();
        // blastParticle.Play();
        
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
            toHitVec = toHitVec.magnitude!=0 ?  knockBack * toHitVec / toHitVec.magnitude : Vector3.zero;
            HitManager.GetTakeDamage(hit.gameObject)?.Damage(damage,toHitVec);
        }

        var hitsGO = new GameObject[hits.Length];
        for (int i=0; i<hits.Length; i++) {
            hitsGO[i] = hits[i].gameObject;
        }

        StartCoroutine(MakeHitFloat(hitsGO));
    }

    IEnumerator MakeHitFloat(GameObject[] objects) {
        var objectManagers = new Dictionary<GameObject, IManager>();
        var objectRigidBodys = new Dictionary<GameObject, Rigidbody2D>();
        foreach (var obj in objects) {
            objectManagers[obj] = obj.GetComponent<IManager>();
            objectRigidBodys[obj] = obj.GetComponent<Rigidbody2D>();
            
            objectManagers[obj].Stunned();
            objectDefGrav[obj] = objectDefGrav.ContainsKey(obj) ? objectDefGrav[obj] : objectRigidBodys[obj].gravityScale;
            objectRigidBodys[obj].gravityScale = 0f;
            objectRigidBodys[obj].AddForce(Vector2.up*levitateAmount);
        }

        var timer = levitateTime;
        while (timer > 0) {
            foreach (var obj in objects) {
                objectManagers[obj].Stunned();
                objectRigidBodys[obj].gravityScale = 0f;
            }
            yield return null;
            timer -= Time.deltaTime;
        }
        
        foreach (var obj in objects) {
            objectManagers[obj].Stunned(false);
            objectRigidBodys[obj].gravityScale = objectDefGrav[obj];
        }
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn && CharacterStats.UseEnergy(energyRequired) && CanUseAbilityAgain()) {
            
            Controller.StopHorizontal();
            _animator.SetTrigger(AnimRefarences.Burst);
            
            // chargeParticle.Play();

            AbilityOn = true;
            AbilityOnInvoke();
        }
        else {
            AbilityOn = false;
        }
    }

    public override void SetAbilityOff() {
        if(!AbilityOn) return;

        // chargeParticle.Stop();
        // chargeParticle.Clear();
        NextCanUse = Time.time + abilityCooldown;
        
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
