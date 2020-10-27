using System.Collections;
using System.IO.Compression;
using UnityEngine;
using UnityEngine.Events;

public class CharacterController2D : MonoBehaviour
{
	[SerializeField] private float m_JumpForce;							// Amount of force added when the player jumps.
	[Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;			// Amount of maxSpeed applied to crouching movement. 1 = 100%
	[Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;	// How much to smooth out the movement
	[SerializeField] private bool m_AirControl = false;							// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask m_WhatIsGround;							// A mask determining what is ground to the character
	[SerializeField] private Transform m_GroundCheck;							// A position marking where to check if the player is grounded.
	[SerializeField] private Transform m_CeilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D m_CrouchDisableCollider;
	[SerializeField] private Transform m_AttackCenter;
	[SerializeField] private float flyingDrag;
	[SerializeField] private bool baseFlying = false;
	private Animator animator;

	private bool _flying = false;
	private bool inAir = false;
	
	// A collider that will be disabled when crouching

	const float k_GroundedRadius = 0.2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.
	private Vector3 m_Velocity = Vector3.zero;
	private float reEnableTime = 0;
	private bool _enabled = true;
	private float _defaultGravity;
	private float _defaultDrag;
	private bool _coyoteTime = false;
	private bool _stopHoriz;
	private float _pushBackHoriz;
	private float _pushBackVertic;
	private int _notGroundedStableCheck = 0;

	private bool _stopAll;
	private bool _stopVertical;

	public bool canTurn = true;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	
	private bool m_wasCrouching = false;

	public bool IsGrounded() {
		if (m_Grounded) {
			return true;
		}

		if (_coyoteTime) {
			return true;
		}

		return false;
	}

	public bool isFlying() {
		return _flying;
	}

	public void MakeFlying(bool shouldFly) {
		if (_flying && !shouldFly) {
			_flying = false;
			m_Rigidbody2D.gravityScale = _defaultGravity;
			m_Rigidbody2D.drag = _defaultDrag;
		}
		else if(!_flying && shouldFly){
			
			_flying = true;
			m_Rigidbody2D.gravityScale = 0;
			m_Rigidbody2D.drag = flyingDrag;
		}
	}
	
	public void HorizontalDecelarate() {
		_stopHoriz = true;
		// if(_flying){return;}
		//
		// Vector2 velocity = m_Rigidbody2D.velocity;
		// Vector3 targetVelocity = new Vector2(0f, velocity.y);
		// // And then smoothing it out and applying it to the character
		// m_Rigidbody2D.velocity = Vector3.SmoothDamp(velocity, targetVelocity, ref m_Velocity, 0.01f);
	}


	public void Push(float pushForceHoriz, float pushForceVertic) {
		// m_Rigidbody2D.AddForce(new Vector2(pushForce, 0));
		_pushBackHoriz = pushForceHoriz;
		_pushBackVertic = pushForceVertic;
	}

	public void Push(Vector2 pushVec) {
		_pushBackHoriz = pushVec.x;
		_pushBackVertic = pushVec.y;
	}

	public int GetFacingMult() {
		if (m_FacingRight) {
			return 1;
		}

		return -1;
	}

	private void Awake()
	{
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();

		_defaultGravity = m_Rigidbody2D.gravityScale;
		_defaultDrag = m_Rigidbody2D.drag;

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
		
		
		MakeFlying(baseFlying);
	}

	private void FixedUpdate() {
		if (_pushBackHoriz > 0.01f || _pushBackHoriz < -0.01f || _pushBackVertic > 0.01f || _pushBackVertic < -0.01f) {
			m_Rigidbody2D.AddForce(new Vector2(_pushBackHoriz, _pushBackVertic));
			_pushBackHoriz = 0f;
			_pushBackVertic = 0f;
		}
		else if (_stopAll) {
			Vector3 targetVelocity = new Vector2(0, 0);
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing/4);
			_stopAll = false;
			_stopHoriz = false;
			_stopVertical = false;
		}
		else {
			if (_stopHoriz){
				Vector3 targetVelocity = new Vector2(0, m_Rigidbody2D.velocity.y);
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing/4);
				_stopHoriz = false;
			}

			if (_stopVertical) {
				Vector3 targetVelocity = new Vector2(m_Rigidbody2D.velocity.x, 0f);
				m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing/4);
				_stopVertical = false;
			}
		}
		
		if (_flying) {
			m_Grounded = false;
		}
		else {
			GroundedCheck();
			// If the object is not moving on the Y axis for 3 frames, its probably grounded.
			if (!m_Grounded && m_Rigidbody2D.velocity.y == 0) {
				_notGroundedStableCheck++;
				if (_notGroundedStableCheck > 3) {
					_notGroundedStableCheck = 0;
					m_Rigidbody2D.AddForce(new Vector2(0f, 100f));
					OnLandEvent.Invoke();
				}
			}
			else {
				_notGroundedStableCheck = 0;
			}
		}
	}

	private void GroundedCheck() {
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				m_Grounded = true;
				if (!wasGrounded) {
					animator.SetBool(AnimRefarences.Jumping, false);
					OnLandEvent.Invoke();
				}
				break;
			}
		}

		if (wasGrounded && !m_Grounded && m_Rigidbody2D.velocity.y <= 0.01f && !_coyoteTime) {
			StartCoroutine(CoyoteTime());
		}

		if (!m_Grounded) {
			animator.SetBool(AnimRefarences.Jumping, true);
		}
		else {
			animator.SetBool(AnimRefarences.Jumping, false);
		}
	}

	IEnumerator CoyoteTime() {
		_coyoteTime = true;
		yield return new WaitForSeconds(0.07f);
		
		animator.SetBool(AnimRefarences.Jumping, true);
		_coyoteTime = false;
	}

	public void StopHorizontal() {
		_stopHoriz = true;
		if (_stopVertical) {
			_stopAll = true;
		}
	}
	
	public void StopVertical() {
		_stopVertical = true;
		if (_stopHoriz) {
			_stopAll = true;
		}
	}

	public void StopAll() {
		_stopAll = true;
		_stopVertical = true;
		_stopHoriz = true;
	}

	public void FlyingMove(float horizontalMove, float verticalMove) {
		if (_stopAll) {
			Vector3 targetVelocity = new Vector2(0, 0);
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, 0);
			return;
		}
		if (!_flying) {
			Move(horizontalMove, verticalMove<0, verticalMove>0);
			return;
		}

		if (canTurn==true) {
			// If the input is moving the player right and the player is facing left...
			if (horizontalMove > 0 && !m_FacingRight)
			{
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (horizontalMove < 0 && m_FacingRight)
			{
				Flip();
			}
		}
		m_Rigidbody2D.AddForce(new Vector2(horizontalMove, verticalMove));
	}

	public void Move(float move, bool crouch, bool jump){
		if (_flying) {
			if (jump) {
				FlyingMove(move, 1);
			}
			else if (crouch) {
				FlyingMove(move, -1);
			}

			return;
		}

		// If uncrouching, check to see if the character can stand up
		if (!crouch)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
			{
				crouch = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || _coyoteTime || m_AirControl)
		{

			// // If crouching
			// if (crouch)
			// {
			// 	if (!m_wasCrouching)
			// 	{
			// 		m_wasCrouching = true;
			// 		OnCrouchEvent.Invoke(true);
			// 	}
			//
			// 	// Reduce the speed by the crouchSpeed multiplier
			// 	// move *= m_CrouchSpeed;
			//
			// 	// Disable one of the colliders when crouching
			// 	if (m_CrouchDisableCollider != null)
			// 		m_CrouchDisableCollider.enabled = false;
			// } else
			// {
			// 	// Enable the collider when not crouching
			// 	if (m_CrouchDisableCollider != null)
			// 		m_CrouchDisableCollider.enabled = true;
			//
			// 	if (m_wasCrouching)
			// 	{
			// 		m_wasCrouching = false;
			// 		OnCrouchEvent.Invoke(false);
			// 	}
			// }

			// Move the character by finding the target velocity
			
			// If the input is moving the player right and the player is facing left...
			if (canTurn==true) {
				if (move > 0 && !m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (move < 0 && m_FacingRight)
				{
					// ... flip the player.
					Flip();
				}
			}
			
			Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);

		}
		// If the player should jump...
		if ((m_Grounded || _coyoteTime) && jump) {
			_coyoteTime = false;
			m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
		}
	}


	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}


	public void SetJumpForce(float newJumpForce) {
		m_JumpForce = newJumpForce;
	}
}
