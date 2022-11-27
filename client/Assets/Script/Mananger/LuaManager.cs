using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaManager : Singleton<LuaManager>
{
    LuaEnv luaenv = null;

    public override void Init()
    {
        luaenv = new LuaEnv();
        luaenv.AddLoader(MyLoader);
        //luaenv.AddBuildin("lpeg", XLua.LuaDLL.Lua.LoadLpeg);
        //luaenv.AddBuildin("rapidjson", XLua.LuaDLL.Lua.LoadRapidJson);
        luaenv.AddBuildin("ffi", XLua.LuaDLL.Lua.LoadFFI);
        luaenv.AddBuildin("pb", XLua.LuaDLL.Lua.LoadLuaProfobuf);
        DoFile("UpdateModule");

    }

    public byte[] MyLoader(ref string filename)
    {
        TextAsset ta = ResManager.Instance.OnLoadAsset("Common", filename, ResType.Lua) as TextAsset;
        return System.Text.Encoding.UTF8.GetBytes(ta.text);
    }

    public void DoString(string str)
    {
        luaenv.DoString(str);
    }

    public void DoFile(string fileName)
    {
        luaenv.DoString("require \"" + fileName+ "\"");
    }

    public void Require(string name, string fileName)
    {
        luaenv.DoString(name + " = require \"" + fileName + "\"");
    }

    public LuaTable GetLuaTable(string classname)
    {
        return luaenv.Global.Get<LuaTable>(classname);        
    }


    public void CallFunction(string classname, string funcName, params object[] args)
    {
        LuaTable metatable = luaenv.Global.Get<LuaTable>(classname);
        LuaFunction lnew = (LuaFunction)metatable[funcName];
        lnew.Call(metatable, args);
    }

    public void LuaGC()
    {
        luaenv.GC();
    }

    // Update is called once per frame
    void Update()
    {
        if (luaenv != null)
        {
            luaenv.Tick();
        }
    }

    public void Destroy()
    {
        luaenv.Dispose();
    }

}
