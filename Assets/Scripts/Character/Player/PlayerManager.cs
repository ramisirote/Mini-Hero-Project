using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/*
 * The player manager gets the players inputs and does the appropriate game actions.
 */
public class PlayerManager : MonoBehaviour, IManager
{
    public event EventHandler<int> ActivePowerChange;


    [SerializeField] CharacterController2D controller;
    [SerializeField] private Animator animator;
    [FormerlySerializedAs("stats")] [SerializeField] private CharacterStats characterStats;
    [SerializeField] private float jumpRememberTime;
    [SerializeField] private AttackManagerBase attackManager;
    [SerializeField] private Transform punch;
    [SerializeField] private SoundManager soundManager;

    private float _attackSpeed;
    private float _runSpeed;
    private float _horizontalMove = 0f;
    private float _verticalMove = 0f;
    private float _shouldJumpTime = -1f;
    private bool _shouldCrouch = false;
    // private bool _disabled = false;
    private bool moveDisabled = false;
    private bool actionDisabled = false;
    private bool _hasLanded = true;
    private bool _stunned = false;

    private bool _isBlocking = false;

    private bool _attackRemember = false;
    private bool _blockRemember = false;

    private float _attackRememberTime;
    private float _blockRememberTime;

    private float nextCanBlock;

    private bool _paused = false;

    private Ability _ability1;
    private Ability _ability2;
    private CharacterPowerManager _powersManager;

    private Camera _mainCam;

    private bool canTurn = true;

    // Start is called when scene is loaded.
    public void Start()
    {

        SetAttackSpeed(characterStats.GetCharacterStats().AttackSpeed);
        _runSpeed = characterStats.GetCharacterStats().MoveSpeed;
        _powersManager = gameObject.GetComponent<CharacterPowerManager>();
        _mainCam = Camera.main;
    }

    // Get a trigger from an ability animation. Passes on the trigger to an on ability
    public void AbilityAnimationTrigger()
    {
        if (_ability1 && _ability1.IsAbilityOn())
        {
            _ability1.AnimationTrigger();
        }
        if (_ability2 && _ability2.IsAbilityOn())
        {
            _ability2.AnimationTrigger();
        }
    }

    public float GetRunSpeed()
    {
        return _runSpeed;
    }

    public void SetRunSpeed(float speed)
    {
        _runSpeed = speed;
        animator.SetFloat(AnimRefarences.SpeedMult, speed / characterStats.GetCharacterStats().MoveSpeed);
    }

    public void SetAttackSpeed(float newSpeed)
    {
        _attackSpeed = newSpeed;
        animator.SetFloat(AnimRefarences.AttackSpeed, newSpeed);
        attackManager.SetAttackSpeed(_attackSpeed);
    }

    public float GetAttackSpeed()
    {
        return _attackSpeed;
    }

    // Clears all the active powers from the manager
    public void ClearPowers()
    {
        _ability1 = null;
        _ability2 = null;
        // -1 here means that powers were both cleared.
        ActivePowerChange?.Invoke(this, -1);
    }

    // Gets an ability and and int, sets the ability as the ability in the appropriate place.
    public void SetAbility(Ability newPower, int slot)
    {
        switch (slot)
        {
            case 0:
                _ability1 = newPower;
                ActivePowerChange?.Invoke(this, 0);
                break;
            case 1:
                _ability2 = newPower;
                ActivePowerChange?.Invoke(this, 1);
                break;
        }
    }

    public void SetAttackManager(AttackManagerBase newAttack)
    {
        attackManager = newAttack;
        attackManager.SetPunchTransform(punch);
        attackManager.SetAttackSpeed(_attackSpeed);
    }

    // Gets an array of the abilites.
    public Ability[] GetAbilities()
    {
        return new Ability[] { _ability1, _ability2 };
    }

    public void DisableManager()
    {
        DisableMove();
        DisableActions();
    }

    public void EnableManager()
    {
        EnableMove();
        EnableActions();
        // _disabled = false;
        canTurn = true;
        controller.canTurn = true;
    }

    public void DisableMove()
    {
        // controller.HorizontalDecelarate();
        animator.SetFloat(AnimRefarences.Speed, 0f);
        moveDisabled = true;
    }

    public void TurnOffAbilities()
    {
        if (_ability1 && _ability1.IsAbilityOn())
        {
            _ability1.SetAbilityOff();
        }
        if (_ability2 && _ability2.IsAbilityOn())
        {
            _ability2.SetAbilityOff();
        }
    }

    public void Stunned(bool value = true)
    {
        _stunned = value;
        animator.SetBool(AnimRefarences.Stunned, value);
        if (_stunned) TurnOffAbilities();
        else EnableManager();
    }

    public bool IsStunned()
    {
        return _stunned;
    }

    public void DisableFlip()
    {
        canTurn = false;
        controller.canTurn = false;
    }

    public void EnableMove()
    {
        moveDisabled = false;
    }

    public void DisableActions()
    {
        actionDisabled = true;
    }

    public void EnableActions()
    {
        actionDisabled = false;
    }

    public void Fly(bool fly)
    {
        controller.MakeFlying(fly);
        animator.SetBool(AnimRefarences.Flying, fly);
        animator.SetBool(AnimRefarences.Jumping, true);
    }

    public bool IsFlying()
    {
        return controller.IsFlying();
    }

    // the target for the player is the mouse.
    public Vector3 GetDirectionToTarget()
    {
        return GetVectorToMouse();
    }

    public Vector3 GetDirectionToTargetFromOther(Vector3 otherPosition)
    {
        var mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 toVec = mousePos - otherPosition;
        return toVec;
    }

    // The update loop check all the input types the player can use and acts accordingly.
    private void Update()
    {
        if (_paused) return;

        CheckActiveAbilityChange();

        if (_stunned) return;

        if (!actionDisabled)
        {
            // Use power input returns true if used. When a power is used the update loop is stopped for that frame.
            if (UseAbilityInput(_ability1, "Ability 1")) return;
            if (UseAbilityInput(_ability2, "Ability 2")) return;
        }

        // update the direction for On abilities
        if (_ability1 && _ability1.IsAbilityOn()) AbilityInputUpdate(_ability1, "Ability 1");
        if (_ability2 && _ability2.IsAbilityOn()) AbilityInputUpdate(_ability2, "Ability 2");


        // Move. (Actual movement is done in fixedUpdate, here the input to move is gotten)
        if (!moveDisabled)
        {
            MoveInput();
            JumpInput();
        }

        // Check attack input.
        AttackInput();

        //Check Block input.
        BlockInput();
    }

    private void CheckActiveAbilityChange()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _powersManager.RotateAbility(0);
            _ability1.OnAbilitySwitchIn();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            _powersManager.RotateAbility(1);
            _ability2.OnAbilitySwitchIn();
        }

        // for debug only, a new system will be impelmented
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _powersManager.UpgradeAbility(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _powersManager.UpgradeAbility(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _powersManager.UpgradeAbility(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _powersManager.UpgradeAbility(3);
        }
    }

    // Update the direction of the ability. Also check if the ability button was released.
    private void AbilityInputUpdate(Ability ability, string buttonName) {
        if (ability.IsAbilityOn())
        {
            if (Input.GetButtonUp(buttonName))
            {
                ability.UseAbilityRelease(GetVectorToMouse());
            }
            else
            {
                ability.UpdateDirection(GetVectorToMouse());
            }
        }
    }

    // Checks if the ability button was pressed. If so use the ability.
    // Returns if the ability was used.
    private bool UseAbilityInput(Ability ability, string buttonName)
    {
        if (!ability) return false;

        bool wasPowerUsed = false;
        if (Input.GetButtonDown(buttonName) && ability.CanUseAbilityAgain())
        {
            bool wasOn = ability.IsAbilityOn();
            ability.UseAbility(GetVectorToMouse());
            wasPowerUsed = true;

        }
        // Check if the button was released in the same frame.
        if (Input.GetButtonUp(buttonName))
        {
            ability.UseAbilityRelease(GetVectorToMouse());
        }

        return wasPowerUsed;
    }

    // Returns the vector from the player to the mouse
    private Vector3 GetVectorToMouse()
    {
        var mousePos = _mainCam.ScreenToWorldPoint(Input.mousePosition);

        Vector3 lookDir = mousePos - transform.position;
        return lookDir;
    }


    // Flip the player to face the mouse
    public void FaceTarget(Transform target = null)
    {
        var targetPos = target ? target.position : _mainCam.ScreenToWorldPoint(Input.mousePosition);
        if (controller.GetFacingMult() > 0)
        {
            if (targetPos.x < transform.position.x)
            {
                controller.Flip();
            }
        }
        else
        {
            if (targetPos.x > transform.position.x)
            {
                controller.Flip();
            }
        }
    }

    private void Block(bool enabled = true)
    {
        _isBlocking = enabled;
        ((TakeDamage)HitManager.GetTakeDamage(gameObject)).SetPlayAnimatoin(!enabled);
        if (!moveDisabled) controller.StopHorizontal();
        if (enabled) DisableManager(); else EnableManager();
        animator.SetBool(AnimRefarences.Block, enabled);
        var stats = characterStats.GetCharacterStats();
        stats.SetAddBuff(CharacterStatsData.StatFields.Resistnce, stats.BlockAmount, remove: !enabled);
    }

    // Remember that the player input an attack but could not do one now.
    private void RememberAttackInput()
    {
        if (Input.GetButtonDown("Punch"))
        {
            _attackRemember = true;
            _attackRememberTime = Time.time + jumpRememberTime;
        }
        if (Time.time > _attackRememberTime)
        {
            _attackRemember = false;
        }
    }

    // If action is disabled or the attack manager cant attack again, remember the input
    private void AttackInput()
    {
        if (Input.GetButtonDown("Punch") || _attackRemember)
        {
            if (!actionDisabled && attackManager.CanAttack())
            {
                attackManager.Attack();
            }
            else
            {
                RememberAttackInput();
            }
        }
    }

    private void RememberBlockInput()
    {
        if (Input.GetButtonDown("Block"))
        {
            _blockRemember = true;
            _blockRememberTime = Time.time + jumpRememberTime;
        }
        if (Time.time > _blockRememberTime)
        {
            _blockRemember = false;
        }
    }

    // If action is disabled or the attack manager cant attack again, remember the input
    private void BlockInput()
    {
        if (Input.GetButtonDown("Block") || _blockRemember)
        {
            if (!actionDisabled && controller.IsGrounded() && !_isBlocking && Time.time >= nextCanBlock)
            {
                Block();
            }
            else
            {
                RememberBlockInput();
            }
        }
        else if (!Input.GetButton("Block") && _isBlocking)
        {
            nextCanBlock = Time.time + 0.2f;
            Block(false);
        }
    }

    public void AttackAnimationTrigger()
    {
        attackManager.AttackTrigger();
    }

    // Check if horizontal input, update the animator and the horizontal move variable.
    // Actual moving is done in fixed update.
    private void MoveInput()
    {
        _horizontalMove = Input.GetAxisRaw("Horizontal") * _runSpeed;
        animator.SetFloat(AnimRefarences.Speed, Math.Abs(_horizontalMove));
        if (!canTurn && controller.GetFacingMult() * _horizontalMove < 0)
        {
            animator.SetBool(AnimRefarences.WalkReverse, true);
        }
        else if (!canTurn)
        {
            animator.SetBool(AnimRefarences.WalkReverse, false);
        }
    }

    // Check if vertical input. if flying, get the raw amount, otherwise, just get bool to jump or nor.
    // Actual moving is done in fixed update.
    private void JumpInput()
    {
        if (controller.IsFlying())
        {
            _verticalMove = Input.GetAxisRaw("Vertical") * _runSpeed;
        }
        else if (Input.GetButtonDown("Jump"))
        {
            _shouldJumpTime = Time.time + jumpRememberTime;
        }
    }

    // Fixed update handles the moving. Uses the character controller.
    private void FixedUpdate()
    {
        if (_paused || moveDisabled || _stunned)
        {
            soundManager.StopAudio(SoundManager.SoundClips.Walk);
            return;
        }

        if (!controller.IsFlying())
        {
            // not flying move
            if (Time.time <= _shouldJumpTime && controller.IsGrounded() && _hasLanded)
            {
                // Jumping
                controller.Move(_horizontalMove * Time.fixedDeltaTime, _shouldCrouch, true);
                animator.SetBool(AnimRefarences.Jumping, true);
                soundManager.PlayAudio(SoundManager.SoundClips.Jump);
                soundManager.StopAudio(SoundManager.SoundClips.Walk);
                _shouldJumpTime = 0;
            }
            else
            {
                // Not Jumping
                GroundMove(false);
            }
        }
        else
        {
            // flying move
            controller.FlyingMove(_horizontalMove, _verticalMove);
            soundManager.StopAudio(SoundManager.SoundClips.Walk);
        }
    }

    // Move when not flying. Run in fixed update.
    private void GroundMove(bool jump)
    {
        controller.Move(_horizontalMove * Time.fixedDeltaTime, _shouldCrouch, jump);
        if ((_horizontalMove > 0.01 || _horizontalMove < -0.01) && controller.IsGrounded())
        {
            soundManager.PlayAudio(SoundManager.SoundClips.Walk);
        }
        else
        {
            soundManager.StopAudio(SoundManager.SoundClips.Walk);
        }
    }

    // When the player lands (was not grounded, now is grounded) runs this
    public void OnLanding()
    {
        animator.SetBool(AnimRefarences.Jumping, false);
        StartCoroutine(LandWait());
    }

    // After landing, wait some time before can jump again. Stops it from looking weird.
    private IEnumerator LandWait()
    {
        _hasLanded = false;
        yield return new WaitForSeconds(0.05f);
        _hasLanded = true;
    }


    public void OnCrouch(bool isCrouching)
    {
        animator.SetBool(AnimRefarences.Crouching, isCrouching);
    }

    // Pause the player. As in pause menu is on.
    public void SetPause(bool pause)
    {
        Debug.Log("Pause set to " + pause);
        _paused = pause;
    }

    public void PermanentDisable()
    {
        enabled = false;
    }
}
