using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject mesh;
    public Renderer renderer;

    public void SetHovered(bool hovered) {
        if (hovered) {
            mesh.layer = LayerMask.NameToLayer("Outlined");
        } else {
            mesh.layer = LayerMask.NameToLayer("Default");
        }
    }

    public Vector3 GetTopPos(float margin = 0f) {
        return renderer.bounds.center + (renderer.bounds.extents.y + margin) * Vector3.up;
    }
    
    public Vector3 GetBottomPos(float margin = 0f) {
        return renderer.bounds.center - (renderer.bounds.extents.y + margin) * Vector3.up;
    }
}
