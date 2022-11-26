using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class ResConst
{
    /// <summary>
    /// 框架根目录
    /// </summary>
    public const string RootFolderName = "App";

    /// <summary>
    /// UI目录
    /// </summary>
    public const string PrefabFolderName = "Prefab";

    /// <summary>
    /// UI目录
    /// </summary>
    public const string SceneFolderName = "Scene";
    
    /// <summary>
    /// 特效目录
    /// </summary>
    public const string SoundFolderName = "Sound";

    /// <summary>
    /// Lua目录
    /// </summary>
    public const string LuaFolderName = "Lua";

    /// <summary>
    /// ToLua目录
    /// </summary>
    public const string ToLuaFolderName = "ToLua";

    /// <summary>
    /// Atlas目录
    /// </summary>
    public const string AtlasFolderName = "Atlas";

    /// <summary>
    /// Texture目录
    /// </summary>
    public const string TextureFolderName = "Texture";

    /// <summary>
    /// Font目录
    /// </summary>
    public const string FontFolderName = "Font";

    /// <summary>
    /// Asset目录
    /// </summary>
    public const string AssetFolderName = "Asset";

    /// <summary>
    /// Font目录
    /// </summary>
    public const string ProtoFolderName = "Protobuf";


    /// <summary>
    /// Material目录
    /// </summary>
    public const string MaterialFolderName = "Material";

    /// <summary>
    /// Lua后缀
    /// </summary>
    public const string LuaExtName = ".lua.bytes";

    /// <summary>
    /// Prefab后缀
    /// </summary>
    public const string PrefabExtName = ".prefab";

    /// <summary>
    /// Atlas后缀
    /// </summary>
    public const string AtlasExtName = ".spriteatlas";

    /// <summary>
    /// Sprite后缀
    /// </summary>
    public const string TextureExtName = ".png";

    /// <summary>
    /// Material后缀
    /// </summary>
    public const string MaterialExtName = ".mat";

    /// <summary>
    /// Font后缀
    /// </summary>
    public const string FontExtName = ".ttf";

    /// <summary>
    /// Asset后缀
    /// </summary>
    public const string AssetExtName = ".asset";

    /// <summary>
    /// Scene后缀
    /// </summary>
    public const string SceneExtName = ".unity";

    /// <summary>
    /// AssetBunld后缀
    /// </summary>
    public const string AssetBunldExtName = ".unity3d";

    /// <summary>
    /// bytes后缀
    /// </summary>
    public const string BytesExtName = ".bytes";

    /// <summary>
    /// txt后缀
    /// </summary>
    public const string TxtExtName = ".txt";


    /// <summary>
    /// 版本文件
    /// </summary>
    public const string VerFile = "ver.txt";

    /// <summary>
    /// 版本文件列表
    /// </summary>
    public const string CheckFile = "files.txt";

    /// <summary>
    /// 打包后Manifest文件名
    /// </summary>
    public const string AssetBundleManifest = "NewUpdata";
    public static string AppRootPath
    {
        get { return Application.dataPath + "/" + RootFolderName; }
    }

    public static string AppRootRelativePath
    {
        get { return "Assets/" + RootFolderName; }
    }
}

[LuaCallCSharp]
public enum ResType
{ 
    AudioClip,
    Prefab,
    Texture,
    Sprite,
    Lua,
    Bytes,
    Txt,
    Scene,
    Material,
    Atlas,
    Font,    
    Asset,
    AssetBundleManifest,
}

