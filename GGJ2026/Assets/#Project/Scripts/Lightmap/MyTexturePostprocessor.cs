//using UnityEditor;
//using UnityEngine;

//class MyTexturePostprocessor : AssetPostprocessor
//{
//    void OnPreprocessTexture()
//    {
//        if (assetPath.Contains("Lightmap-0_comp_") )
//        {
//            Debug.Log("Texture processed: " + assetPath);

//            TextureImporter textureImporter = (TextureImporter)assetImporter;
//            var tis = new TextureImporterSettings();
//            textureImporter.ReadTextureSettings(tis);
//            tis.readable = true;
//            textureImporter.SetTextureSettings(tis);

//        }
//    }
//}