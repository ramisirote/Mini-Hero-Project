using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/*
 * Abilities are the Abilities the player and enemies can use during the game.
 * Each ability inherits from this class.
 */
public abstract class Ability: MonoBehaviour
{
    public event EventHandler AbilitySetupEvent;
    public event EventHandler AbilityOnEvent;
    public event EventHandler AbilityOffEvent;
    
    [SerializeField] protected GameObject parentCharacter = null;
    [SerializeField] protected float abilityCooldown;
    [SerializeField] protected float energyRequired;

    protected float NextCanUse = 0;
    protected bool AbilityOn = false;

    [SerializeField] protected Color _color1;
    [SerializeField] protected Color _color2;
    [SerializeField] protected Color _color3;
    
    
    [FormerlySerializedAs("powerIcon")] [SerializeField] protected Sprite abilityIcon;
    [SerializeField] protected AudioSource audioSource;
    
    protected IManager Manager;
    protected CharacterController2D Controller;
    protected CharacterStats CharacterStats;

    protected bool[] unlocks = new bool[4];

    // Uses the start function if ability isn't Given through the instance class
    public void Start() {
        if (!parentCharacter) {
            return;
        }
        
        transform.parent = parentCharacter.transform;
        Manager = parentCharacter.GetComponent<IManager>();
        Controller = parentCharacter.GetComponent<CharacterController2D>();
        CharacterStats = parentCharacter.GetComponent<CharacterStats>();
        var takeDamage = parentCharacter.GetComponent<TakeDamage>();
        if (takeDamage) { takeDamage.OnDamage += OnDamageTaken; }
        
        AdditionalInit();
    }

    // When an ability is added to the player (first time or on load) its init is called to set it up.
    public void Init(GameObject parent, Color color1, Color color2, Color color3) {
        _color1 = color1;
        _color2 = color2;
        _color3 = color3;
        
        if (parentCharacter) return;
        parentCharacter = parent;
        
        Manager = parentCharacter.GetComponent<PlayerManager>();
        
        AbilitySetupEvent?.Invoke(this, EventArgs.Empty);
        
        transform.parent = parentCharacter.transform;
        
    }

    // Init, but with unlockables.
    // Unlockables have to yet been implemented.
    public void Init(GameObject parent, Color color1, Color color2, Color color3, bool[] otherUnlocks) {
        for (int i = 0; i < unlocks.Length && i<otherUnlocks.Length; i++) {
            unlocks[i] = otherUnlocks[i];
        }
        Init(parent, color1, color2, color3);
    }

    // Init, but without changing the color.
    public void Init(GameObject parent) {
        Init(parent, _color1, _color2, _color3);
    }
    
    // Run right after Init
    protected abstract void AdditionalInit();

    // Return if the ability is currently on
    public bool IsAbilityOn() {
        return AbilityOn;
    }

    // Get the ability icon
    public Sprite GetIconImage() {
        return abilityIcon;
    }

    
    // Uses the ability
    public abstract void UseAbility(Vector3 direction);

    // Start input to use ability
    public virtual void UseAbilityStart(Vector3 direction) {
        UseAbility(direction);
    }

    // Updates the direction input of the ability
    public virtual void UpdateDirection(Vector3 direction) {
        return;
    }

    // Release input of the ability
    public virtual void UseAbilityRelease(Vector3 direction) {
        return;
    }

    // Run when damage is taken by the player. (Usually turns off the power)
    protected virtual void OnDamageTaken(object o, float damageAmount) {
        return;
    }

    // Returns if the cooldown is done.
    public virtual bool CanUseAbilityAgain() {
        return Time.time >= NextCanUse;
    }

    // Gets a trigger from the animation of an ability.
    public virtual void AnimationTrigger() {
        return;
    }

    // Returns the energy requirement of the ability.
    public virtual float EnergyRequired() {
        if (!AbilityOn) {
            return energyRequired;
        }

        return 0f;
    }

    // Sets the ability as off.
    public abstract void SetAbilityOff();

    // Event that the ability turned on
    protected void AbilityOnInvoke() {
        AbilityOnEvent?.Invoke(this, EventArgs.Empty);
    }
    
    // Event that the ability turned off
    protected void AbilityOffInvoke() {
        AbilityOffEvent?.Invoke(this, EventArgs.Empty);
    }
    
    // Event when the ability is first set up
    protected void AbilitySetupInvoke() {
        AbilitySetupEvent?.Invoke(this, EventArgs.Empty);
    }
}
