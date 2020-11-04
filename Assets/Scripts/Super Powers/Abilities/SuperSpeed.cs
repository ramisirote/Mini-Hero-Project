using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Increase the character's move speed and attack speed for a time.
 * While on the time scale of the game is slowed a bit to allow for more control with higher speed.
 */
public class SuperSpeed : Ability
{

    [SerializeField] private float onTime;
    [SerializeField] private float attackSpeed;
    [SerializeField] private float runSpeedMult;
    [SerializeField][Range(0.1f, 1)] private float timeSlow;
    private float originalSpeed;
    private float originalAttackSpeed;
    private TrailRenderer speedTrail;
    private TrailRenderer _handLTrail;
    private TrailRenderer _handRTrail;
    private float _turnOffTime;

    private GameObject trail;

    private Gradient ColorGradient() {
        var gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = _color1;
        colorKey[0].time = 0.0f;
        colorKey[1].color = _color2;
        colorKey[1].time = 1.0f;

        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 0.8f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;
        
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }

    private void InitHandTrails() {
        var armLEffectPoint = parentCharacter.GetComponent<EffectPoints>().GetPointObject(Refarences.EBodyParts.ArmL);
        _handLTrail = armLEffectPoint.GetComponent<TrailRenderer>();
        if (!_handLTrail) {
            _handLTrail = armLEffectPoint.AddComponent<TrailRenderer>();
        }
        
        var armREffectPoint = parentCharacter.GetComponent<EffectPoints>().GetPointObject(Refarences.EBodyParts.ArmR);
        _handRTrail = armREffectPoint.GetComponent<TrailRenderer>();
        if (!_handRTrail) {
            _handRTrail = armREffectPoint.AddComponent<TrailRenderer>();
        }
        
        _handRTrail.materials = speedTrail.materials;
        _handLTrail.materials = speedTrail.materials;

        _handLTrail.startWidth = 0.2f;
        _handRTrail.startWidth = 0.2f;
        _handLTrail.endWidth = 0f;
        _handRTrail.endWidth = 0f;

        _handLTrail.time = 0.4f;
        _handRTrail.time = 0.4f;

        _handLTrail.colorGradient = ColorGradient();
        _handRTrail.colorGradient = ColorGradient();
        _handLTrail.enabled = false;
        _handRTrail.enabled = false;
        
    }
    
    protected override void AdditionalInit() {
        speedTrail = GetComponent<TrailRenderer>();
        speedTrail.colorGradient = ColorGradient();
        
        speedTrail.enabled = false;

        InitHandTrails();
    }

    private void Update() {
        // Check if the power is done. revert to normal settings
        if (AbilityOn && Time.time >= _turnOffTime) {
            SetAbilityOff();
        }
    }

    public override void UseAbility(Vector3 direction) {
        if (!AbilityOn) {
            Time.timeScale = timeSlow;
            AbilityOn = true;
            NextCanUse = Time.time + abilityCooldown * Time.deltaTime;
            originalSpeed = Manager.GetRunSpeed();
            Manager.SetRunSpeed(originalSpeed * runSpeedMult);
            Manager.SetAttackSpeed(attackSpeed);
            speedTrail.enabled = true;
            _handLTrail.enabled = true;
            _handRTrail.enabled = true;
            _turnOffTime = Time.time + onTime;
            
            AbilityOnInvoke();
        }
    }

    public override void SetAbilityOff() {
        if (!AbilityOn) return;
        
        Time.timeScale = 1f;
        Manager.SetRunSpeed(originalSpeed);
        Manager.SetAttackSpeed(1f);
        AbilityOn = false;
        speedTrail.enabled = false;
        speedTrail.Clear();
        _handLTrail.enabled = false;
        _handLTrail.Clear();
        _handRTrail.enabled = false;
        _handRTrail.Clear();
            
        AbilityOffInvoke();
    }
}
