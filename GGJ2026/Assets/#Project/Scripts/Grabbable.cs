using UnityEngine;

public class Grabbable : MonoBehaviour
{
    public Rigidbody rigidBody;
    public GameObject mesh;

    public void SetHovered(bool hovered) {
        if (hovered) {
            mesh.layer = LayerMask.NameToLayer("Outlined");
        } else {
            mesh.layer = LayerMask.NameToLayer("Default");
        }
    }
}
