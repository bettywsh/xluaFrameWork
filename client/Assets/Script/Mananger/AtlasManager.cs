﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : MonoSingleton<AtlasManager>
{
    Dictionary<string, SpriteAtlas> spriteAtlasList = new Dictionary<string, SpriteAtlas>();
    private void OnEnable()
    {
        SpriteAtlasManager.atlasRequested += RequestAtlas;
    }

    private void OnDisable()
    {
        SpriteAtlasManager.atlasRequested -= RequestAtlas;
    }

    void RequestAtlas(string atlasName, System.Action<SpriteAtlas> callback)
    {
        ResManager.Instance.LoadAssetAsync(atlasName, "Atlas/" + atlasName + ".spriteatlas", typeof(SpriteAtlas), (go) =>
        {
            callback(go as SpriteAtlas);
        });
    }
}
