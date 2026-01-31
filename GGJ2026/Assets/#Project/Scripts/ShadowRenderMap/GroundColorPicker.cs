using UnityEngine;

public class GroundColorPicker : MonoBehaviour
{
    [SerializeField] private RenderTexture groundTexture;
    [SerializeField] private float brightnessTreshold = 0.80f;
    [SerializeField] private bool enableLogging = false;

    // debug values in inspector
    public Color surfaceColor;
    public float brightness1; // http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color 
    public float brightness2; // http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx

    void Update()
    {
        Raycast();

        // BRIGHTNESS APPROX
        brightness1 = (surfaceColor.r + surfaceColor.r + surfaceColor.b + surfaceColor.g + surfaceColor.g + surfaceColor.g) / 6;

        // BRIGHTNESS
        brightness2 = Mathf.Sqrt((surfaceColor.r * surfaceColor.r * 0.2126f + surfaceColor.g * surfaceColor.g * 0.7152f + surfaceColor.b * surfaceColor.b * 0.0722f));
    }

    void OnGUI()
    {
        if (this.enableLogging)
        {
            GUILayout.BeginArea(new Rect(10f, 10f, Screen.width, Screen.height));

            GUILayout.Label("R = " + string.Format("{0:0.00}", surfaceColor.r));
            GUILayout.Label("G = " + string.Format("{0:0.00}", surfaceColor.g));
            GUILayout.Label("B = " + string.Format("{0:0.00}", surfaceColor.b));

            GUILayout.Label("Brightness Approx = " + string.Format("{0:0.00}", brightness1));
            GUILayout.Label("Brightness = " + string.Format("{0:0.00}", brightness2));

            GUILayout.Label("In Shadow = " + (IsInShadow() ? "true" : "false"));

            GUILayout.EndArea();
        }
    }

    void Raycast()
    {
        if (this.enableLogging)
            Debug.DrawLine(this.transform.position + Vector3.up * 1f, this.transform.position + Vector3.down * 1f, Color.red);

        // convert render texture to texture2D (probably not optimal for performance, but ok for now)
        Texture2D groundMap = toTexture2D(groundTexture);

        // get color at center of viewport
        Color surfaceColor = groundMap.GetPixelBilinear(.5f, .5f);

        // APPLY
        this.surfaceColor = surfaceColor;
    }

    Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGB24, false);
        // ReadPixels looks at the active RenderTexture.
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        return tex;
    }

    public bool IsInShadow()
    {
        return brightness2 < brightnessTreshold;
    }
}