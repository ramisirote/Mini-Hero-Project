using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Teleport instantly in the direction given.
 * If the target location is in the ground, teleport to the nearest
 * spot between you and the target that is not in a wall.
 */
public class Teleportation : Ability
{

    [SerializeField] private ParticleSystem teleportEffect;
    [SerializeField] private float teleportDistance;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float maxTeleportDistance;

    private ParticleSystem _startTelEffect;
    private ParticleSystem _endTelEffect;

    private Transform _feetTransform;
    
    public override void UseAbility(Vector3 direction) {
        // Vector3 facingVec = direction;
        // if (parentCharacter.transform.localScale.x > 0) {
        //     facingVec.x = -facingVec.x;
        // }
        _endTelEffect.transform.position = parentCharacter.transform.position;
        _endTelEffect.Play();
        AbilityOnInvoke();
        audioSource.Play();
        
        Manager.FacePowerTarget();

        parentCharacter.transform.position += GetSafeTargetTeleportation(direction);
        
        _startTelEffect.transform.position = parentCharacter.transform.position;
        _startTelEffect.Play();

        StartCoroutine(PowerOffDeley());
    }

    IEnumerator PowerOffDeley() {
        yield return new WaitForSeconds(0.2f);
        
        AbilityOffInvoke();
    }

    private Vector3 GetSafeTargetTeleportation(Vector3 direction) {
        direction.z = 0;
        if (direction.magnitude > teleportDistance) {
            direction = (direction / direction.magnitude) * teleportDistance;
        }
        Vector3 targetDistance = direction;
        Vector3 feetPosition = _feetTransform.position;

        int loopMax = 30;
        
        while (IsInGround(feetPosition + targetDistance) && loopMax>0) {
            targetDistance -= direction/2f;
            loopMax--;
        }

        return loopMax != 0 ? targetDistance: Vector3.zero;
    }

    private bool IsInGround(Vector3 pos) {
        RaycastHit2D ray = Physics2D.Raycast(pos + Vector3.up * 0.5f, Vector2.up, 0.3f, ground);
        Debug.DrawRay(pos + Vector3.up * 0.1f, Vector2.up, Color.red);
        return ray.collider != null;
    }

    protected override void AdditionalInit() {
        _feetTransform = parentCharacter.GetComponent<EffectPoints>().GetPointTransform(Refarences.EBodyParts.LegR);
        ParticleInstant();
    }
    
    private void ParticleInstant() {
        _startTelEffect = Instantiate(teleportEffect);
        var mainModule = _startTelEffect.main;
        mainModule.startColor = new Color(_color1.r, _color1.g, _color1.b, 0.7f);
        _startTelEffect.transform.position = transform.position;
        _endTelEffect = Instantiate(teleportEffect);
        mainModule = _endTelEffect.main;
        mainModule.startColor = new Color(_color2.r, _color2.g, _color2.b, 0.7f);
        _endTelEffect.Stop();
        _startTelEffect.Stop();
    }

    public override void SetAbilityOff() {
        if (AbilityOn) AbilityOn = false;
    }
}
