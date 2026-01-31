using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BuildSystem : MonoBehaviour
{
    public Transform cursor;
    public Transform movementPlane;
    public LayerMask grabbableMask;
    public LayerMask mouseMoveMask;
    
    private Grabbable _hoveredBlock;
    private Grabbable _grabbedBlock;
    
    private Vector2 _mousePos;
    
    private RaycastHit _interactionHit;
    private RaycastHit _movementHit;
    private Vector3 _prevMovementHit;
    private Vector3 _movement;

    private void Start() {
        _interactionHit = new RaycastHit();
    }
    
    void Update() {
        _mousePos = Mouse.current.position.ReadValue();
        
        DoHoverCheck();
        DoGrabCheck();
        DoMovement();
    }

    private void DoHoverCheck() {
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(ray, out _interactionHit, grabbableMask)) {
            cursor.position = _interactionHit.point;
            var grabbable = _interactionHit.collider.gameObject.GetComponent<Grabbable>();
            if (grabbable != null) {
                if (_hoveredBlock == null || _hoveredBlock != grabbable) {
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
            }
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame) {
            if (_grabbedBlock != null) {
                _grabbedBlock = null;
            }
        }
    }

    private void DoMovement() {
        movementPlane.position = _grabbedBlock.transform.position;
        
        Ray ray = Camera.main.ScreenPointToRay(_mousePos);
        if (Physics.Raycast(ray, out _movementHit, mouseMoveMask)) {
            var movementHit = _movementHit.point;
            _movement = _prevMovementHit - movementHit;

            if (_grabbedBlock != null) {
                _grabbedBlock.transform.position = _grabbedBlock.transform.position;
            }
            
            _prevMovementHit = movementHit;
        }
    }
}
