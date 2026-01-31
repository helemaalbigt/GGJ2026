using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BuildManager : MonoBehaviour
{
    public Transform cursor;
    public Transform movementPlane;
    public LayerMask grabbableMask;
    public LayerMask mouseMoveMask;
    
    [Header("DEBUG")]
    [SerializeField]
    private Grabbable _hoveredBlock;
    [SerializeField]
    private Grabbable _grabbedBlock;
    [SerializeField]
    private Transform _debugGrabbedCenterTarget;

    private Vector3 _grabbedCenterTargetPos;
    
    private Vector2 _mousePos;
    
    private RaycastHit _interactionHit;
    private RaycastHit _movementHit;
    private Vector3 _prevMovementHit;
    private int _hitFrames;
    private Vector3 _movement;
    private bool _moveVertically = true;

    private void Start() {
        _interactionHit = new RaycastHit();
    }

    private void OnEnable() {
        CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.ShiftMovementPlane, ShiftPlanePressed);
        CustomInputManager.SubscribeToCancelled(CustomInputManager.Player.ShiftMovementPlane, ShiftPlaneReleased);
    }
    
    private void OnDisable() {
        CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.ShiftMovementPlane, ShiftPlanePressed);
        CustomInputManager.UnsubscribeFromCancelled(CustomInputManager.Player.ShiftMovementPlane, ShiftPlaneReleased);
    }

    private void ShiftPlanePressed(InputAction.CallbackContext obj) {
        movementPlane.eulerAngles = new Vector3(0, 0, 0f);
        _hitFrames = 0;
        _moveVertically = false;
    }
    
    private void ShiftPlaneReleased(InputAction.CallbackContext obj) {
        movementPlane.eulerAngles = new Vector3(0, 0, -90f);
        _hitFrames = 0;
        _moveVertically = true;
    }

    void Update() {
        _mousePos = Mouse.current.position.ReadValue();
        movementPlane.position = Camera.main.transform.position + Camera.main.transform.forward * 1f;
        
        DoHoverCheck();
        DoGrabCheck();
        DoMovement();

        _debugGrabbedCenterTarget.position = _grabbedCenterTargetPos;
    }

    private void DoHoverCheck() {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(ray, out _interactionHit, 3f, grabbableMask)) {
            cursor.position = _interactionHit.point;
            var grabbable = _interactionHit.collider.gameObject.GetComponent<Grabbable>();
            if (grabbable != null) {
                if (_hoveredBlock == null || (_hoveredBlock != grabbable && _grabbedBlock == null)) {
                    if (_hoveredBlock != null) {
                        _hoveredBlock.SetHovered(false);
                    }
                    _hoveredBlock = grabbable;
                    _hoveredBlock.SetHovered(true);
                }
            }
            else {
                CheckToHoverOff();
            }
        }
        else {
            CheckToHoverOff();
        }
    }
    
    private void CheckToHoverOff() {
        if (_grabbedBlock == null && _hoveredBlock != null) {
            _hoveredBlock.SetHovered(false);
            _hoveredBlock = null;
        }
    }

    private void DoGrabCheck() {
        if (Mouse.current.leftButton.wasPressedThisFrame) {
            if (_hoveredBlock != null && _grabbedBlock == null) {
                _grabbedBlock = _hoveredBlock;
                _grabbedCenterTargetPos = _grabbedBlock.transform.position;
                _hitFrames = 0;
            }
        }

        if (!Mouse.current.leftButton.isPressed) {
            if (_grabbedBlock != null) {
                _hoveredBlock.SetHovered(false);
                _hoveredBlock = null;
                _grabbedBlock = null;
            }
        }
    }

    private void DoMovement() {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(ray, out _movementHit, 3f, mouseMoveMask)) {
            var movementHit = _movementHit.point;
            _movement = movementHit - _prevMovementHit;
            // if (_moveVertically) {
            //     _movement.z = 0;
            // } else {
            //     _movement.y = 0;
            // }

            if (_grabbedBlock != null && _hitFrames > 2) {
                _grabbedCenterTargetPos += _movement;
                _grabbedBlock.rigidBody.linearVelocity = (_grabbedCenterTargetPos - _grabbedBlock.transform.position) * 15f;
                //_grabbedBlock.transform.position += _movement;
            }
            
            _prevMovementHit = movementHit;
            _hitFrames++;
        }
    }
}
