using UnityEditor;

public static class TexturePreImporter
{
    public static void ProcTexture(string assetPath, ref TextureImporter importer)
    {
        if (assetPath.StartsWith("Assets/Res/UI"))
        {
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.textureType = TextureImporterType.Sprite;
            importer.wrapMode = UnityEngine.TextureWrapMode.Clamp;
        }
        else
        {
            return;
        }
        importer.mipmapEnabled = false;
        importer.compressionQuality = 50;
        var andSettings = new TextureImporterPlatformSettings();
        var iosSettings = new TextureImporterPlatformSettings();
        andSettings.name = "Android";
        iosSettings.name = "iPhone";
        andSettings.overridden = true;
        iosSettings.overridden = true;
        andSettings.compressionQuality = iosSettings.compressionQuality = 50;

        andSettings.format = TextureImporterFormat.ETC2_RGBA8;
        andSettings.androidETC2FallbackOverride = AndroidETC2FallbackOverride.Quality32Bit;
        iosSettings.format = TextureImporterFormat.ASTC_6x6;
        andSettings.maxTextureSize = iosSettings.maxTextureSize = 1024;

        importer.SetPlatformTextureSettings(andSettings);
        importer.SetPlatformTextureSettings(iosSettings);
    }


}
