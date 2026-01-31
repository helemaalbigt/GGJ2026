using System;
using UnityEngine;


[ExecuteInEditMode]
public class TextureScaler : MonoBehaviour {
    public Renderer renderer;
    public Transform reference;

    private void Update() {
        if (renderer == null || reference == null)
            return;
        
        var s = reference.localScale.y;
        renderer.material.SetTextureScale("_MainTex", new Vector2(1f, s * 100f));
    }
}
