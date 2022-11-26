using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XLua;

public class LuaBehaviour : MonoBehaviour
{
    public LuaTable metaTable;
    public object[] args;
    public string className;
    public string classPath;

    private void Awake()
    {
        if (metaTable == null)
        {
            //Debug.LogError("Invalid script file '" + Ctrl + "', metaTable needed as a result.");
            return;
        }
        //从类中找到New函数
        LuaFunction lnew = (LuaFunction)metaTable.Get<LuaFunction>("Awake");
        if (lnew == null)
        {
            //Debug.LogError("Invalid metaTable of script '" + Ctrl + "', function 'New' needed.");
            return;
        }
        //执行New函数生成脚本对象
        lnew.Call(metaTable, transform, args);
    }

    private void Start()
    {
        //从类中找到New函数
        LuaFunction lnew = (LuaFunction)metaTable.Get<LuaFunction>("Start");
        if (lnew == null)
        {
            //Debug.LogError("Invalid metaTable of script '" + Ctrl + "', function 'New' needed.");
            return;
        }
        //执行New函数生成脚本对象
        lnew.Call(metaTable);
    }

    private void Update()
    {
        //从类中找到Update函数
        LuaFunction lnew = (LuaFunction)metaTable.Get<LuaFunction>("Update");
        if (lnew == null)
        {
            //Debug.LogError("Invalid metaTable of script '" + Ctrl + "', function 'New' needed.");
            return;
        }

        //执行Update函数生成脚本对象
        lnew.Call(metaTable, Time.deltaTime);
    }

    private void OnDestroy()
    {
        if (metaTable == null)
            return;
        //从类中找到Update函数
        LuaFunction lnew = (LuaFunction)metaTable.Get<LuaFunction>("Destroy");
        if (lnew == null)
        {
            //Debug.LogError("Invalid metaTable of script '" + Ctrl + "', function 'New' needed.");
            return;
        }

        //执行Update函数生成脚本对象
        lnew.Call(metaTable, Time.deltaTime);
    }

}
