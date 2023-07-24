using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.AppInputActions;

public class PlayerAnimationMovement : MonoBehaviour, IPlayerActions
{
    [SerializeField] private Animator animator;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float forwardLerp = 5.0f;
    [SerializeField] private float shiftLerp = 5.0f;
    [SerializeField] private float directionLerp = 5.0f;
    [SerializeField] private float jumpTimeout = 1.0f;
    [SerializeField] private float jumpHeight;
    [SerializeField] private AnimationCurve jumpCurve;

    private AppInputActions _actions;
    private int _animMoveType = Animator.StringToHash("MoveType");
    private int _animDirection = Animator.StringToHash("MoveDirection");
    private int _animJump = Animator.StringToHash("Jump");

    private void OnEnable()
    {
        _actions = new AppInputActions();
        _actions.Player.SetCallbacks(this);
        _actions.Player.Enable();
    }

    private void OnDisable()
    {
        _actions.Player.Disable();
    }

    private bool _jumpIsStarted;
    private void Jump()
    {
        if (_jumpIsStarted) return;
        _jumpIsStarted = true;
        StartCoroutine(JumpCoroutine());
    }

    private IEnumerator JumpCoroutine()
    {
        animator.SetBool(_animJump, true);
        yield return new WaitForSeconds(jumpTimeout);
        animator.SetBool(_animJump, false);
        _jumpIsStarted = false;
        yield return null;
    }

    private float _moveDirection = 0.5f;
    private float _multiplayShift = 0.5f;
    private float _forwardMove = 0.0f;
    private void Update()
    {
        _moveDirection = Mathf.Lerp(_moveDirection, (_direction.x + 1.0f) / 2.0f, directionLerp * Time.deltaTime);
        _forwardMove = Mathf.Clamp(Mathf.Lerp(_forwardMove, _direction.magnitude > 0.1f ? 1.0f : 0.0f, forwardLerp * Time.deltaTime), 0.0f, 1.0f);
        _multiplayShift = Mathf.Lerp(_multiplayShift, _shift && _forwardMove > 0.5f ? 1.0f : 0.11f, shiftLerp * Time.deltaTime);
        
        animator.SetFloat(_animDirection, _moveDirection);
        animator.SetFloat(_animMoveType, _forwardMove * _multiplayShift);
    }

    private void FixedUpdate()
    {
        if (_jumpIsStarted) return;
        characterController.Move(Vector3.down * 0.1f);
    }

    private bool _shift = false;
    private Vector2 _direction = Vector2.zero;
    public void MoveEvent(Vector2 direction) => _direction = direction;
    public void ShiftEvent(bool shift) => _shift = shift;
    public void OnMove(InputAction.CallbackContext context) => MoveEvent(context.ReadValue<Vector2>());
    public void OnShift(InputAction.CallbackContext context)
    {
        if (context.performed) ShiftEvent(true);
        if (context.canceled) ShiftEvent(false);
    }
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnFire(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) => Jump();
}
