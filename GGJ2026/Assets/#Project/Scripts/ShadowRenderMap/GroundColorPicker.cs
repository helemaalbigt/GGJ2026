using UnityEngine;

public class GroundColorPicker : MonoBehaviour
{
    [SerializeField] private RenderTexture groundTexture;
    [SerializeField] private float brightnessTreshold = 0.45f;
    [SerializeField] private int sampleSize = 2;
    [SerializeField] private bool enableLogging = false;

    // debug values in inspector
    public float brightness; // http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color 

    void Update()
    {
        Color[] surfaceColors = pickGroundColors(this.sampleSize);
        this.brightness = calculateAverageBrightness(surfaceColors);
    }

    void OnGUI()
    {
        if (this.enableLogging)
        {
            GUILayout.BeginArea(new Rect(10f, 10f, Screen.width, Screen.height));

            GUILayout.Label("Brightness = " + string.Format("{0:0.00}", this.brightness));

            GUILayout.Label("In Shadow = " + (IsInShadow() ? "true" : "false"));

            GUILayout.EndArea();
        }
    }

    private Color[] pickGroundColors(int sampleSize)
    {
        // debug: show position
        if (this.enableLogging)
            Debug.DrawLine(this.transform.position + Vector3.up * 1f, this.transform.position + Vector3.down * 1f, Color.red);

        // convert render texture to texture2D (probably not optimal for performance, but ok for now)
        Texture2D groundMap = toTexture2D(groundTexture);


        int width = Mathf.FloorToInt(groundMap.width / 2) - sampleSize;
        int height = Mathf.FloorToInt(groundMap.height / 2) - sampleSize;

        // get color at center of viewport (=player position as camera is centered on player)
        Color[] surfaceColors = groundMap.GetPixels(width, height, sampleSize * 2, sampleSize * 2);

        return surfaceColors;
    }

    private float calculateAverageBrightness(Color[] colors)
    {
        float totalBrightness = 0f;
        foreach (Color col in colors)
        {
            float brightness = Mathf.Sqrt((col.r * col.r * 0.2126f + col.g * col.g * 0.7152f + col.b * col.b * 0.0722f));
            totalBrightness += brightness;
        }
        return totalBrightness / colors.Length;
    }

    private Texture2D toTexture2D(RenderTexture rTex)
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
        return this.brightness < this.brightnessTreshold;
    }
}