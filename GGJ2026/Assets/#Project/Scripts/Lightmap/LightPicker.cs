using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class LightmapPixelPicker : MonoBehaviour
{

    public Color surfaceColor;
    public float brightness1; // http://stackoverflow.com/questions/596216/formula-to-determine-brightness-of-rgb-color 
    public float brightness2; // http://www.nbdtech.com/Blog/archive/2008/04/27/Calculating-the-Perceived-Brightness-of-a-Color.aspx

    public Color shadowColor;

    public LayerMask layerMask;

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
        GUILayout.BeginArea(new Rect(10f, 10f, Screen.width, Screen.height));

        GUILayout.Label("R = " + string.Format("{0:0.00}", surfaceColor.r));
        GUILayout.Label("G = " + string.Format("{0:0.00}", surfaceColor.g));
        GUILayout.Label("B = " + string.Format("{0:0.00}", surfaceColor.b));

        GUILayout.Label("Brightness Approx = " + string.Format("{0:0.00}", brightness1));
        GUILayout.Label("Brightness = " + string.Format("{0:0.00}", brightness2));

        GUILayout.Label("SR = " + string.Format("{0:0.00}", shadowColor.r));
        GUILayout.Label("SG = " + string.Format("{0:0.00}", shadowColor.g));
        GUILayout.Label("SB = " + string.Format("{0:0.00}", shadowColor.b));

        GUILayout.Label("In Shadow = " + (RGBequal(shadowColor, Color.black) ? "true" : "false"));

        GUILayout.EndArea();
    }

    void Raycast()
    {
        // RAY TO PLAYER'S FEET
        Ray ray = new Ray(transform.position, -Vector3.up);
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.magenta);

        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, 5f, layerMask))
        {
            // GET RENDERER OF OBJECT HIT
            Renderer hitRenderer = hitInfo.collider.GetComponent<Renderer>();

            // GET LIGHTMAP APPLIED TO OBJECT
            LightmapData lightmapData = LightmapSettings.lightmaps[hitRenderer.lightmapIndex];
            // assume lightmap index 0 is ground
            //LightmapData lightmapData = LightmapSettings.lightmaps[0];

            // STORE LIGHTMAP TEXTURE
            Texture2D lightmapTex = lightmapData.lightmapColor;

            // GET LIGHTMAP COORDINATE WHERE RAYCAST HITS
            Vector2 pixelUV = hitInfo.lightmapCoord;
            //Vector2 pixelUV = new Vector2(this.transform.position.x, this.transform.position.z); // use world xz as uv for ground
            
            Debug.Log("Lightmap UV: " + pixelUV);

            // GET COLOR AT THE LIGHTMAP COORDINATE
            Color surfaceColor = lightmapTex.GetPixelBilinear(pixelUV.x, pixelUV.y);

            // APPLY
            this.surfaceColor = surfaceColor;

            // other maps
            Texture2D shadowMaskTex= lightmapData.shadowMask;

            // GET COLOR AT THE SHADOW MASK COORDINATE
            surfaceColor = shadowMaskTex.GetPixelBilinear(pixelUV.x, pixelUV.y);

            // APPLY
            this.shadowColor = surfaceColor;
        }
    }

    // compare without alpha channel
    bool RGBequal(Color c1, Color c2)
    {
        return c1.r== c2.r && c1.g == c2.g && c1.b == c2.b;
    }

}