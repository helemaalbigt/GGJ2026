using Unity.VisualScripting;
using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject mesh;
    public Renderer renderer;
    public AudioSource onCollisionSfx;


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
    
    public Vector3 GetBackPos(float margin = 0f) {
        return renderer.bounds.center - (renderer.bounds.extents.x + margin) * Vector3.up;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (onCollisionSfx == null)
        {
            // play default sound at volume depending on collision force
            SfxManager.Instance.PlayOnCollisionSfx(Mathf.InverseLerp(0, 10f, collision.relativeVelocity.magnitude));
        }
        else
            // play custom assigned sound
            onCollisionSfx.Play();

    }
}
