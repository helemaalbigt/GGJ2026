using System.Collections.Generic;
using UnityEngine;

public class GravityManager : MonoBehaviour {
    [Header("Debug")] 
    public Transform _debugParent;
    public bool _toggleDebugParent;
    private bool _debugGravOff;

    void Update() {
        if (_toggleDebugParent) {
            SetAllChildBlocksGravity(_debugParent, !_debugGravOff);
            _debugGravOff = !_debugGravOff;
            _toggleDebugParent = false;
        }
    }

    public void SetAllChildBlocksGravity(Transform parent, bool gravityOn) {
        var grabbables = parent.GetComponentsInChildren<Grabbable>(true);
        foreach (var grabbable in grabbables) {
            grabbable.rigidBody.useGravity = gravityOn;
            grabbable.rigidBody.linearDamping = gravityOn ? 1f : 100f;
            if (gravityOn) {
                grabbable.rigidBody.constraints = RigidbodyConstraints.None;
            }
            else {
                grabbable.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            }
        }
    }
}
