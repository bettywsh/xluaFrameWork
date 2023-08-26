using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.IO;

public class TaskAtals : ITask
{
    private SpriteAtlasPackingSettings packingSetting = new SpriteAtlasPackingSettings()
    {
        blockOffset = 1,
        padding = 2,
        enableRotation = false,
        enableTightPacking = false
    };
    private SpriteAtlasTextureSettings textureSetting = new SpriteAtlasTextureSettings()
    {
        sRGB = true,
        filterMode = FilterMode.Bilinear
    };
    private TextureImporterPlatformSettings iosSettings = new TextureImporterPlatformSettings()
    {
        format = TextureImporterFormat.ASTC_6x6,
        androidETC2FallbackOverride = AndroidETC2FallbackOverride.UseBuildSettings,
        name = "iPhone",
        overridden = true,
        maxTextureSize = 1024,
        compressionQuality = 50,
    };

    private TextureImporterPlatformSettings andSettings = new TextureImporterPlatformSettings()
    {
        format = TextureImporterFormat.ETC2_RGBA8,
        androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit,
        name = "Android",
        overridden = true,
        maxTextureSize = 1024,
        compressionQuality = 50,
    };

    private string imageDirPath = Application.dataPath + "/Res/UI";
    private string targetDirPath = Application.dataPath + "/App/Atlas";

    public void Run(PackSetting packSetting)
    {
        CreateAtlasOfAssetDir(imageDirPath);
    }

    public void CreateAtlasOfAssetDir(string dirAssetPath)
    {
        if (!Directory.Exists(dirAssetPath))
        {
            Debug.LogError("没有图集文件夹");
            return;
        }

        DirectoryInfo dir = new DirectoryInfo(dirAssetPath);
        string[] dirs = Directory.GetDirectories(dirAssetPath);
        foreach (string d in dirs)
        {
            SpriteAtlas atlas = new SpriteAtlas();
            atlas.SetIncludeInBuild(false);
            atlas.SetPackingSettings(packingSetting);
            atlas.SetTextureSettings(textureSetting);
            atlas.SetPlatformSettings(andSettings);
            atlas.SetPlatformSettings(iosSettings);
            DirectoryInfo dirInfo = new DirectoryInfo(d);
            var atlasPath = $"Assets/App/Atlas/{dirInfo.Name}.spriteatlas";
            Object o = AssetDatabase.LoadAssetAtPath<Object>($"{PackFile.Trans2AssetPath(d)}/Image");
            Object[] obj = { o };
            if (o != null) atlas.Add(obj);
            AssetDatabase.CreateAsset(atlas, atlasPath);
            AssetDatabase.SaveAssets();
        }

    }

}
