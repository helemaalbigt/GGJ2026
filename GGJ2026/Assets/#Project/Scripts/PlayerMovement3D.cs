using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement3D : MonoBehaviour
{
	#region Editor Fields
	[Header("Components")]
	[SerializeField] private Rigidbody _rb;

	[Header("Parameters")]
	[SerializeField] private float _speed = 35f;
	[SerializeField] private float _rotationSpeed = 10f;
	[SerializeField] private float _groundDrag = 5f;
	[SerializeField] private float _airSpeed = 8;
	[SerializeField] private float _airDrag = 5f;
	[SerializeField] private float _gravityForce = -9.8f;

	[Header("Grounded Checks")]
	[SerializeField] private float _groundedCheckUpOffset;
	[SerializeField] private float _groundedCheckRadius;
	[SerializeField] private LayerMask _groundedLayerMask;

    [Header("SFX")]
    [SerializeField] private AudioSource _runningSfx;
    #endregion

    #region Fields
    private Transform _cameraPivot;
	private InputAction _moveInput;
	private Vector3 _inputDirection;
	private Vector3 _lookDirection;
	private bool _isGrounded;

	private RaycastHit[] _groundedResults = new RaycastHit[1];
	private bool _canMove = true;
	#endregion

	#region Properties
	public float SpeedModifier { get; set; } = 1;
	public float AirSpeedModifier { get; set; } = 1;
	public float JumpForceModifier { get; set; } = 1;
	public float GravityModifier { get; set; } = 1;
	public float GroundDragModifier { get; set; } = 1;
	public float AirDragModifier { get; set; } = 1;

	public Vector3 Velocity => _rb.linearVelocity;
	public Vector3 InputDirection => _inputDirection.normalized;

	public bool IsGrounded
	{
		get => _isGrounded;
		private set
		{
			// prevent the same value from being passed through
			if (_isGrounded == value) return;
			// set the new grounded value
			_isGrounded = value;
		}
	}

	public bool UseGravity { get; set; } = true;
	public bool CanMove
	{
		get => _canMove;
		set
		{
			if (_canMove != value)
			{
				_canMove = value;
			}
		}
	}

	public bool CanRotate { get; set; } = true;

	public Rigidbody Rigidbody => _rb;
	#endregion

	#region Events
	#endregion

	#region Mono
	private void OnEnable()
	{
		_moveInput = CustomInputManager.GetInputAction(CustomInputManager.Player.Move);
		_cameraPivot = Camera.main.transform;
	}

	private void OnDisable()
	{
		_moveInput = null;
		_cameraPivot = null;
	}
	private void FixedUpdate()
	{
		// sets the rayscast position slightly above the feet of the player
		Vector3 raycastPos = transform.position;
		raycastPos.y += _groundedCheckUpOffset;

		CalculateInputDirection();
		CalculateDrag();
		MovePlayer();
		RotatePlayer();
		CalculateIsGrounded(raycastPos);
	}
	#endregion

	#region Methods
	public void ResetMovementModifiers(bool clearExtraModifiers)
	{
		SpeedModifier = 1f;
		AirSpeedModifier = 1f;
		GravityModifier = 1f;
		JumpForceModifier = 1f;
		GroundDragModifier = 1f;
		AirDragModifier = 1f;
	}

	private void CalculateInputDirection()
	{
		Vector2 input = _moveInput.ReadValue<Vector2>();
		Vector3 camForward = Horizontal(_cameraPivot.forward);
		Vector3 camRight = Horizontal(_cameraPivot.right);

        // play running sfx only when there is input
        if (input.magnitude > 0)
		{
			if (!_runningSfx.isPlaying)
				_runningSfx.Play();
		}
		else
		{
			_runningSfx.Stop();
        }

        _inputDirection = input.y * camForward + input.x * camRight;
	}

	private void CalculateDrag()
	{
		if (IsGrounded)
		{
			_rb.linearDamping = _groundDrag * GroundDragModifier;
		}
		else
		{
			_rb.linearDamping = _airDrag * AirDragModifier;
		}
	}
	private void CalculateIsGrounded(Vector3 raycastPos)
	{
		// does the raycast to see if the player is grounded or not, if something on the layers got hit then the players is grounded
		if (Physics.CheckSphere(raycastPos, _groundedCheckRadius, _groundedLayerMask, QueryTriggerInteraction.Ignore))
		{
			IsGrounded = true;
			return;
		}

		IsGrounded = false;
	}
	private void MovePlayer()
	{
		Vector3 moveVector = _inputDirection;

		if (!CanMove)
		{
			moveVector = Vector3.zero;
        }

        if (IsGrounded)
		{
			moveVector *= _speed * SpeedModifier;
			moveVector.y = -1f;
        }
		else
		{
            moveVector *= _airSpeed * AirSpeedModifier;
			if (UseGravity)
			{
				moveVector.y = _gravityForce * GravityModifier;
			}
		}

		AddForce(moveVector, ForceMode.Acceleration);
	}



	private void RotatePlayer()
	{
		if (_inputDirection is not { x: 0f, z: 0f } && CanRotate)
		{
			_lookDirection = _inputDirection;
		}

		// if you can't rotate then you shouldn't do anything
		if (!CanRotate) return;
		if (_lookDirection.magnitude == 0) return;
		Quaternion targetRotation = Quaternion.LookRotation(_lookDirection, Vector3.up);
		transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationSpeed);
	}
	public void SetVelocityToZero()
    {
        _runningSfx.Stop();
        _rb.linearVelocity = Vector3.zero;
	}

	public void AddForce(Vector3 force, ForceMode mode = ForceMode.Acceleration)
	{
		_rb.AddForce(force, mode);
	}

	public void Lookat(Vector3 direction, bool resetInput = false, bool snapToDirection = false)
	{
		_lookDirection = direction;
		if (resetInput)
		{
			_inputDirection = Vector3.zero;
		}
		if (snapToDirection)
		{
			transform.LookAt(transform.position + direction, Vector3.up);
		}
	}

	private Vector3 Horizontal(Vector3 reference)
	{
		reference.y = 0;
		reference.Normalize();
		return reference;
	}

	public void SetActive(bool enabled)
	{
		this.enabled = enabled;

		if (!enabled)
		{
			SetVelocityToZero();
		}
	}

	public void MoveTo(Vector3 position)
	{
		_runningSfx.Stop();
        StartCoroutine(MovePlayerStatic(position));
	}
	private IEnumerator MovePlayerStatic(Vector3 target)
	{
		var curPosition = transform.position;
		var distance = Vector3.Distance(curPosition, target);
		float travelTime = distance / _speed;
		float travelledTime = 0;

		var direction = (target - curPosition).normalized;
		Lookat(direction, true, true);

		while (travelledTime < travelTime)
		{
			travelledTime += Time.deltaTime / 3;
			_rb.MovePosition(Vector3.Lerp(curPosition, target, travelledTime / travelTime));
			yield return null;
		}
	}
	#endregion
}

