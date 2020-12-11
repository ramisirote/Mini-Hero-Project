using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Character manager interface. Any character uses this interface to manage the actions of the character
 */
public interface IManager
{
    // Disables all actions and movement of the character
    void DisableManager();

    // Disables all actions if the character, but not movement (attack and abilities are actions)
     void DisableActions();

     // Disables movement of the character (from internal forces, not external)
     void DisableMove();

     // Stops the character from flipping looking direction.
     void DisableFlip();

     // Enable all actions and movement of the character
     void EnableManager();
     
     // Enable all actions if the character, but not movement (attack and abilities are actions)
     void EnableActions();

     // Enable movement of the character
     void EnableMove();

     // Get a trigger from an ability animation, to be passed on to the ability
     void AbilityAnimationTrigger();

     // // 
     // void SetAbility(SuperPower ability);

     // Set the attack manager to handle the character's basic attacks.
     void SetAttackManager(AttackManagerBase newAttack);

     // Set the run speed of the character (on the manager level)
     void SetRunSpeed(float speed);

     // Get the run speed of the character (on the manager level)
     float GetRunSpeed();

     // Set the attack speed of the character (on the attack level)
     void SetAttackSpeed(float attackSpeed);
     
     // Get the attack speed of the character (on the manager level)
     float GetAttackSpeed();

     // Gets a bool. If its true, set the manager to flying, otherwise set to not flying.
     void Fly(bool shouldFly);

     // Return if the manager is in flying mode.
     bool IsFlying();

     // Get the direction to the target if one exists. Otherwise returns facing forward;
     Vector3 GetDirectionToTarget();

     // Face the target. If there is not target, do nothing.
     void FaceTarget();

     // Get a trigger from the animator that a basic attack is triggered to do the hit.
     void AttackAnimationTrigger();

     void PermanentDisable();
}
