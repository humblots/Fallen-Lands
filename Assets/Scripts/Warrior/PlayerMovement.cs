/*
	Created by @DawnosaurDev at youtube.com/c/DawnosaurStudios
	Adapted to my personal needs 
 */

using System;
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public PlayerData Data;

	#region COMPONENTS
    public Rigidbody2D RB { get; private set; }
    public Animator Animator { get; private set; }
	[SerializeField] private GameObject TRObject;
    private TrailRenderer TR;
    #endregion

	#region STATE PARAMETERS
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsWallSliding { get; private set; }

	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }

	private bool _isJumpCut;
	private bool _isJumpFalling;

	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;
	#endregion

	#region INPUT PARAMETERS
	private Vector2 _moveInput;
	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }
	#endregion

	#region CHECK PARAMETERS
	[Header("Checks")] 
	[SerializeField] private Transform _groundCheckPoint;
	[SerializeField] private float _groundCheckRadius = .2f;
	[Space(5)]
	[SerializeField] private Transform _wallCheckPoint;
	[SerializeField] private float _wallCheckRadius = .2f;
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	[SerializeField] private LayerMask _wallLayer;
	#endregion

	private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		Animator = GetComponent<Animator>();
		TR = TRObject.GetComponent<TrailRenderer>();
	}

	private void Start()
	{
		SetGravityScale(Data.gravityScale);
		IsFacingRight = true;
	}

	private void Update()
	{
		SetTimers();
		Inputs();
		CollisionsCheck();
		WallSlideLogic();
		JumpsLogic();
		DashLogic();
		GravityLogic();
	}

    private void FixedUpdate()
	{
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(Data.wallJumpRunLerp);
			else
			{
				Run(1);
				Animator.SetFloat("xVelocity", Mathf.Abs(RB.velocity.x));
				Animator.SetFloat("yVelocity", RB.velocity.y); 
			}
		}
		else if (_isDashAttacking)
		{
			Run(Data.dashEndRunLerp);
		}
    }

    #region TIMERS
    private void SetTimers()
    {
	    LastOnGroundTime -= Time.deltaTime;
	    LastOnWallTime -= Time.deltaTime;

	    LastPressedJumpTime -= Time.deltaTime;
	    LastPressedDashTime -= Time.deltaTime;
    }
    #endregion

    #region INPUTS
    private void Inputs()
    {
	    _moveInput.x = Input.GetAxisRaw("Horizontal");
	    _moveInput.y = Input.GetAxisRaw("Vertical");

	    if (_moveInput.x != 0)
		    CheckDirectionToFace(_moveInput.x > 0);

	    if(Input.GetButtonDown("Jump"))
	    {
		    OnJumpInput();
	    }

	    if (Input.GetButtonUp("Jump"))
	    {
		    OnJumpUpInput();
	    }

	    if (Input.GetKeyDown(KeyCode.LeftShift))
	    {
		    OnDashInput();
	    }
    }
    #endregion

    #region INPUT CALLBACKS
    public void OnJumpInput()
	{
		LastPressedJumpTime = Data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		LastPressedDashTime = Data.dashInputBufferTime;
	}
    #endregion

    #region GRAVITY METHODS
    public void SetGravityScale(float scale)
    {
	    RB.gravityScale = scale;
    }    

    private void GravityLogic()
    {
	    if (!_isDashAttacking)
	    {
		    if (RB.velocity.y < 0 && _moveInput.y < 0)
		    {
			    SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
			    RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
		    }
		    else if (_isJumpCut)
		    {
			    SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
			    RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
		    }
		    else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		    {
			    SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
		    }
		    else if (RB.velocity.y < 0)
		    {
			    SetGravityScale(Data.gravityScale * Data.fallGravityMult);
			    RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
		    }
		    else
		    {
			    SetGravityScale(Data.gravityScale);
		    }
	    }
	    else
	    {
		    SetGravityScale(0);
	    }
    }
    #endregion
    
    #region GENERAL METHODS
	private void Sleep(float duration)
    {
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration);
		Time.timeScale = 1;
	}
    #endregion

    #region RUN METHODS
    private void Run(float lerpAmount)
	{
		float targetSpeed = _moveInput.x * Data.runMaxSpeed;
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		float accelRate;

		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;

		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}

		if(Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			accelRate = 0; 
		}

		float speedDif = targetSpeed - RB.velocity.x;
		float movement = speedDif * accelRate;

		RB.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}

	private void Turn()
	{
		Vector3 scale = transform.localScale; 
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
    #endregion

    #region JUMP METHODS
    private void JumpsLogic()
    {
	    if (IsJumping && RB.velocity.y < 0)
	    {
		    Animator.SetBool("isJumping", false);
		    IsJumping = false;
		    _isJumpFalling = true;
	    }

	    if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
	    {
		    Animator.SetBool("isWallJumping", false);
		    IsWallJumping = false;
	    }

	    if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
	    {
		    _isJumpCut = false;
		    _isJumpFalling = false;
	    }

	    if (!IsDashing)
	    {
		    if (CanJump() && LastPressedJumpTime > 0)
		    {
			    Animator.SetBool("isJumping", true);
			    IsJumping = true;
			    IsWallJumping = false;
			    _isJumpCut = false;
			    _isJumpFalling = false;
			    Jump();
		    }
		    else if (CanWallJump() && LastPressedJumpTime > 0)
		    {
			    Animator.SetBool("isWallJumping", true);
			    IsWallJumping = true;
			    IsJumping = false;
			    _isJumpCut = false;
			    _isJumpFalling = false;

			    _wallJumpStartTime = Time.time;
			    _lastWallJumpDir = IsFacingRight ? -1 : 1;

			    WallJump(_lastWallJumpDir);
		    }
	    }
    }

    private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		float force = Data.jumpForce;
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
	}

	private void WallJump(int dir)
	{
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallTime = 0;

		Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
		force.x *= dir;

		if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
			force.x -= RB.velocity.x;

		if (RB.velocity.y < 0) 
			force.y -= RB.velocity.y;

		RB.AddForce(force, ForceMode2D.Impulse);
		if (Data.doTurnOnWallJump)
			Turn();
	}
	#endregion

	#region DASH METHODS
	private void DashLogic()
	{
		if (CanDash() && LastPressedDashTime > 0)
		{
			Sleep(Data.dashSleepTime); 

			if (_moveInput != Vector2.zero)
				_lastDashDir = _moveInput;
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;

			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
	}
	
	private IEnumerator StartDash(Vector2 dir)
	{
		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;

		TR.emitting = true;
		SetGravityScale(0);
		Animator.SetBool("isDashing", true);
		while (Time.time - startTime <= Data.dashAttackTime)
		{
			RB.velocity = dir.normalized * Data.dashSpeed;
			yield return null;
		}

		startTime = Time.time;

		_isDashAttacking = false;

		SetGravityScale(Data.gravityScale);
		RB.velocity = Data.dashEndSpeed * dir.normalized;

		while (Time.time - startTime <= Data.dashEndTime)
		{
			yield return null;
		}

		IsDashing = false;
		Animator.SetBool("isDashing", false);
		TR.emitting = false;
	}

	private IEnumerator RefillDash(int amount)
	{
		_dashRefilling = true;
		yield return new WaitForSeconds(Data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + 1);
	}
	#endregion

    #region WALL SLIDE METHODS

    private void WallSlideLogic()
    {
	    if (IsFacingWall() && !IsGrounded() && !IsJumping && _moveInput.x != 0f)
	    {
		    IsWallSliding = true;
		    Animator.SetBool("isWallSliding", true);
	    }
	    else
	    {
		    IsWallSliding = false;
		    Animator.SetBool("isWallSliding", false);
	    }
    }
    #endregion

    #region CHECK METHODS
    private void CollisionsCheck()
    {
	    if (!IsDashing && !IsJumping)
	    {
		    if (IsGrounded())
		    {
			    LastOnGroundTime = Data.coyoteTime;
		    }

		    if (IsFacingWall())
		    {
			    LastOnWallTime = Data.coyoteTime;
		    }
	    }
    }
    
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

    private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }

	private bool CanWallJump()
    {
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && !IsWallJumping && IsWallSliding;
    }

	private bool CanJumpCut()
    {
		return IsJumping && RB.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return IsWallJumping && RB.velocity.y > 0;
	}

	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{
			StartCoroutine(nameof(RefillDash), 1);
		}

		return _dashesLeft > 0;
	}

	public bool IsGrounded()
	{
		return Physics2D.OverlapCircle(_groundCheckPoint.position, _groundCheckRadius, _groundLayer);
	}

	public bool IsFacingWall()
	{
		return Physics2D.OverlapCircle(_wallCheckPoint.position, _wallCheckRadius, _wallLayer);
	}
	#endregion


    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(_groundCheckPoint.position, _groundCheckRadius);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(_wallCheckPoint.position, _wallCheckRadius);
	}
    #endregion
}
