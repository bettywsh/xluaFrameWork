using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaManager : MonoSingleton<LuaManager>
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
    }

    public byte[] MyLoader(ref string filename)
    {
        TextAsset ta = ResManager.Instance.LoadAsset("Common", "Lua/" + filename + ".lua.bytes", typeof(TextAsset)) as TextAsset;
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


    public object[] CallFunction(string classname, string funcName, params object[] args)
    {
        LuaTable metatable = luaenv.Global.Get<LuaTable>(classname);
        LuaFunction lnew = null;
        if (metatable != null)
            lnew = (LuaFunction)metatable[funcName];
        if(lnew != null)
        {
            return lnew.Call(metatable, args);
        }
        else
        {
            return null;
        }
    }

    public object[] CallFunction(string funcName, params object[] args)
    {
        LuaFunction lf = luaenv.Global.Get<LuaFunction>(funcName);
        if(lf != null)
        {
            return lf.Call(args);
        }
        else
        {
            return null;
        }
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
        CallFunction("GameUpdate", "Update", null);
    }

    public void Destroy()
    {
        luaenv.Dispose();
    }

}
