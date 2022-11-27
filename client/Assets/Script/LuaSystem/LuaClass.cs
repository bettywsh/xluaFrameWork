using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;
using UnityEngine.U2D;

public class LuaClass : MonoBehaviour
{
    public string classPath;
    public string className;
    public LuaTable metaTable;
    public object[] args;
    public List<SpriteAtlas> SpriteAtlasList;

    public Sprite GetSprite(string atlasName, string spriteName)
    {
        Sprite s = null;
        for (int i = 0; i < SpriteAtlasList.Count; i++)
        {
            if (SpriteAtlasList[i].name == atlasName)
            {
                s = SpriteAtlasList[i].GetSprite(spriteName);
            }
        }
        if (s == null)
        {
            Debug.LogError(atlasName + "找不到图片" + spriteName);
        }
        return s;
    }
}
