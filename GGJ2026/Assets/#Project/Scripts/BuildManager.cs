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
    public LayerMask groundRefMask;
    public Transform grid;
    public Transform inputHelper;
    public GameObject grabHelper;
    public GameObject moveHelper;
    
    [Header("DEBUG")]
    [SerializeField]
    private Grabbable _hoveredBlock;
    [SerializeField]
    private Grabbable _grabbedBlock;
    [SerializeField]
    private Transform _groundCursor;
    [SerializeField]
    private Transform _dottedLine;
    [SerializeField]
    private Renderer _dottedLineRenderer;

    private Pose _grabbedCenterTargetPose;
    
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
        
        CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.RotateXYClockwise, RotateXYClockwisePressed);
        CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.RotateXYCounterClockwise, RotateXYCounterClockwisePressed);
        CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.RotateZYClockwise, RotateZYClockwisePressed);
        CustomInputManager.SubscribeToPerformed(CustomInputManager.Player.RotateZYCounterClockwise, RotateZYCounterClockwisePressed);
    }
    
    private void OnDisable() {
        CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.ShiftMovementPlane, ShiftPlanePressed);
        CustomInputManager.UnsubscribeFromCancelled(CustomInputManager.Player.ShiftMovementPlane, ShiftPlaneReleased);
        
        CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.RotateXYClockwise, RotateXYClockwisePressed);
        CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.RotateXYCounterClockwise, RotateXYCounterClockwisePressed);
        CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.RotateZYClockwise, RotateZYClockwisePressed);
        CustomInputManager.UnsubscribeFromPerformed(CustomInputManager.Player.RotateZYCounterClockwise, RotateZYCounterClockwisePressed);

        if (_hoveredBlock != null) {
            _hoveredBlock.SetHovered(false);
            _hoveredBlock = null;
        }
        _grabbedBlock = null;
        
        UpdateHelper();
        UpdateGroundCursor();
    }

    private void ShiftPlanePressed(InputAction.CallbackContext obj) {
        movementPlane.eulerAngles = new Vector3(0, 0, 0f);
        _hitFrames = 0;
        _moveVertically = false;
        UpdateGridGuide();
    }
    
    private void ShiftPlaneReleased(InputAction.CallbackContext obj) {
        movementPlane.eulerAngles = new Vector3(0, 0, -90f);
        _hitFrames = 0;
        _moveVertically = true;
        UpdateGridGuide();
    }

    private void RotateXYClockwisePressed(InputAction.CallbackContext obj) {
        _grabbedCenterTargetPose.rotation = Quaternion.Euler(90f, 0, 0) * _grabbedCenterTargetPose.rotation;
    }
    
    private void RotateXYCounterClockwisePressed(InputAction.CallbackContext obj) {
        _grabbedCenterTargetPose.rotation = Quaternion.Euler(-90f, 0, 0) * _grabbedCenterTargetPose.rotation;
    }
    
    private void RotateZYClockwisePressed(InputAction.CallbackContext obj) {
        _grabbedCenterTargetPose.rotation = Quaternion.Euler(0, 0, 90f) * _grabbedCenterTargetPose.rotation;
    }
    
    private void RotateZYCounterClockwisePressed(InputAction.CallbackContext obj) {
        _grabbedCenterTargetPose.rotation = Quaternion.Euler(0, 0, -90f) * _grabbedCenterTargetPose.rotation;
    }

    void Update() {
        _mousePos = Mouse.current.position.ReadValue();
        movementPlane.position = Camera.main.transform.position + Camera.main.transform.forward * 1f;
        
        DoHoverCheck();
        DoGrabCheck();
        DoMovement();
        UpdateHelper();
    }

    private void LateUpdate() {
        UpdateGroundCursor();
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
                _grabbedCenterTargetPose.position = _grabbedBlock.transform.position;
                _grabbedCenterTargetPose.rotation = _grabbedBlock.transform.rotation;
                _hitFrames = 0;

                UpdateGridGuide();
            }
        }

        if (!Mouse.current.leftButton.isPressed) {
            if (_grabbedBlock != null) {
                _hoveredBlock.SetHovered(false);
                _hoveredBlock = null;
                _grabbedBlock = null;

                UpdateGridGuide();
            }
        }
    }

    private void DoMovement() {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(ray, out _movementHit, 3f, mouseMoveMask)) {
            var movementHit = _movementHit.point;
            _movement = movementHit - _prevMovementHit;

            if (_grabbedBlock != null && _hitFrames > 2) {
                _grabbedCenterTargetPose.position += _movement;

                _grabbedBlock.rigidBody.linearVelocity = (_grabbedCenterTargetPose.position - _grabbedBlock.transform.position) * 15f;
                _grabbedBlock.rigidBody.Move(_grabbedBlock.transform.position, _grabbedCenterTargetPose.rotation);
            }
            
            _prevMovementHit = movementHit;
            _hitFrames++;
        }
    }

    private void UpdateGridGuide() {
        grid.gameObject.SetActive(_grabbedBlock != null);
        if (_grabbedBlock != null) {
            grid.position = _moveVertically ? _grabbedBlock.GetBackPos(0.005f) : new Vector3(_grabbedBlock.transform.position.x, 0.0025f, _grabbedBlock.transform.position.z);
            grid.eulerAngles = _moveVertically ? new Vector3(0,-90f, -90f) : new Vector3(90f,0,0);
        }
    }
    
    private void UpdateGroundCursor() {
        if (_grabbedBlock != null) {
            if (Physics.Raycast(_grabbedBlock.GetBottomPos(0.015f), Vector3.down, out RaycastHit hit, 1f, groundRefMask)) {
                _groundCursor.gameObject.SetActive(true);
                _dottedLine.gameObject.SetActive(true);
                _groundCursor.position = hit.point;
                _dottedLine.position = hit.point;
                _dottedLine.localScale = new Vector3(1, hit.distance, 1);
            } else {
                _groundCursor.gameObject.SetActive(false);
                _dottedLine.gameObject.SetActive(false);
            }
        }
        else {
            _groundCursor.gameObject.SetActive(false);
            _dottedLine.gameObject.SetActive(false);
        }
    }

    private void UpdateHelper() {
        if (_grabbedBlock != null) {
            inputHelper.transform.position = _grabbedBlock.GetTopPos(0.01f);
            
            inputHelper.gameObject.SetActive(true);
            grabHelper.gameObject.SetActive(false);
            moveHelper.gameObject.SetActive(true);
        } else if (_hoveredBlock != null) {
            inputHelper.transform.position = _hoveredBlock.GetTopPos(0.01f);
            
            inputHelper.gameObject.SetActive(true);
            grabHelper.gameObject.SetActive(true);
            moveHelper.gameObject.SetActive(false);
        } else {
            inputHelper.gameObject.SetActive(false);
        }
    }
}
