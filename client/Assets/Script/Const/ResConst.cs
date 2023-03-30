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
    /// 项目文件夹打包方式配置
    /// </summary>
    public const string BuildJson = "BuildConfig/Build";

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
    /// json后缀
    /// </summary>
    public const string JsonExtName = ".json";

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

}

[LuaCallCSharp]
public enum ResType
{ 
    AudioClip = 1,
    Prefab = 2,
    Texture = 3,
    Sprite = 4,
    Lua = 5,
    Bytes = 6,
    Txt = 7,
    Scene = 8,
    Material = 9,
    Atlas = 10,
    Font = 11,
    Asset = 12,
    AssetBundleManifest = 13,
    Json = 14,
}

