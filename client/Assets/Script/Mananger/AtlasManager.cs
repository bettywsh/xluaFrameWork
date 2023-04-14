using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class AtlasManager : MonoSingleton<AtlasManager>
{
    Dictionary<string, SpriteAtlas> spriteAtlasList = new Dictionary<string, SpriteAtlas>();
    public override void Init()
    {
        SpriteAtlasManager.atlasRequested += RequestAtlas;
    }

    private void OnDestroy()
    {
        SpriteAtlasManager.atlasRequested -= RequestAtlas;
    }


    void RequestAtlas(string atlasName, System.Action<SpriteAtlas> callback)
    {
        SpriteAtlas sa = ResManager.Instance.LoadAsset(atlasName, "Atlas/" + atlasName + ".spriteatlas", typeof(SpriteAtlas)) as SpriteAtlas;
        callback(sa);
    }
}
