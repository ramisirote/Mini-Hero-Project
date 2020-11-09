using System.Collections;
using System.IO.Compression;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

/*
 * Character controller is in charge of the physical movement of the character.
 * Handles if the character is grounded, jumping (including animation states related to jumping and falling),
 * moving, flying, flipping looking direction.
 */
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
	private Collider2D[] groundCheckOverlapArray;

	public bool canTurn = true;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	
	private bool m_wasCrouching = false;

	public bool IsGrounded() {
		return m_Grounded || _coyoteTime;
	}

	public bool IsFlying() {
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


	public void Push(float pushForceHoriz, float pushForceVertic) {
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
		
		groundCheckOverlapArray = new Collider2D[2];
	}

	private void FixedUpdate() {
		if (!DoPush()) {
			DoStops();
		}
		
		if (_flying) {
			m_Grounded = false;
		}
		else {
			GroundedCheck();
			GroundStableCheck();
		}
	}

	/*
	 * This check prevents a character from getting stuck in the air where they are not moving on the
	 * y axis, but not grounded, such as stuck on a wall.
	 * If so, adds a small force to the character to try and get them free.
	 */
	private void GroundStableCheck() {
		
		if (!m_Grounded && m_Rigidbody2D.velocity.y == 0) {
			_notGroundedStableCheck++;
			if (_notGroundedStableCheck > 3) {
				_notGroundedStableCheck = 0;
				m_Rigidbody2D.AddForce(new Vector2(0f, 100f));
			}
		}
		else {
			_notGroundedStableCheck = 0;
		}
	}

	/*
	 * Should be run only in fixed update.
	 * Pushes the character based on the values in push horizontal and vertical.
	 * Resets them, since they are already done.
	 */
	private bool DoPush() {
		if (_pushBackHoriz < 0.01f && _pushBackHoriz > -0.01f && _pushBackVertic < 0.01f && _pushBackVertic > -0.01f) {
			return false;
		}
		
		m_Rigidbody2D.AddForce(new Vector2(_pushBackHoriz, _pushBackVertic));
		_pushBackHoriz = 0f;
		_pushBackVertic = 0f;
		return true;
	}

	/*
	 * This should only be run in fixed update.
	 * Stops The character's velocity smoothly in the axis that is set to stop.
	 */
	private void DoStops() {
		if (_stopAll) {
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
	}

	private void GroundedCheck() {
		bool wasGrounded = m_Grounded;
		m_Grounded = false;

		/*
		 * The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		 * This can be done using layers instead but Sample Assets will not overwrite your project settings.
		 */
		for (int i = 0; i < 2; i++) groundCheckOverlapArray[i] = null;
		Physics2D.OverlapCircleNonAlloc(m_GroundCheck.position, k_GroundedRadius, 
														groundCheckOverlapArray, m_WhatIsGround);
		if (groundCheckOverlapArray.Any(colliderOverlapped => 
												colliderOverlapped && colliderOverlapped.gameObject != gameObject)) {
			m_Grounded = true;
			if (!wasGrounded) {
				animator.SetBool(AnimRefarences.Jumping, false);
				OnLandEvent.Invoke();
			}
		}

		if (wasGrounded && !m_Grounded && m_Rigidbody2D.velocity.y <= 0.01f && !_coyoteTime) {
			StartCoroutine(CoyoteTime());
		}

		animator.SetBool(AnimRefarences.Jumping, !m_Grounded);
	}

	private IEnumerator CoyoteTime() {
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
		if (!_flying) {
			Move(horizontalMove, verticalMove<0, verticalMove>0);
			return;
		}

		if (canTurn==true) {
			// If the input is moving the player right and the player is facing left...
			if (canTurn){
				if (horizontalMove > 0 && !m_FacingRight || horizontalMove < 0 && m_FacingRight) {
					Flip();
				}
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

		//only control the player if grounded or airControl is turned on
		if (m_Grounded || _coyoteTime || m_AirControl) {
			if (canTurn){
				if (move > 0 && !m_FacingRight || move < 0 && m_FacingRight) {
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
