using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using XLua;


[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class ListView : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    public GameObject Item;
    
    private int  mtotalCount= -1;
    public int TotalCount
    {
        get
        {
            return mtotalCount;
        }
        set
        {
            mtotalCount = value;
            var ls = GetComponent<LoopScrollRect>();
            ls.prefabSource = this;
            ls.dataSource = this;
            ls.totalCount = mtotalCount;
            ls.RefillCells();
        }
    }

    // Implement your own Cache Pool here. The following is just for example.
    Stack<Transform> pool = new Stack<Transform>();
    public GameObject GetObject(int index)
    {
        if (pool.Count == 0)
        {
            return Instantiate(Item);
        }
        Transform candidate = pool.Pop();
        candidate.gameObject.SetActive(true);
        return candidate.gameObject;
    }

    public void ReturnObject(Transform trans)
    {
        // Use `DestroyImmediate` here if you don't need Pool
        trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        pool.Push(trans);
    }

    public void ProvideData(Transform transform, int idx)
    {        
        ListViewItem listViewItem = transform.GetComponent<ListViewItem>();
        VarPrefab vp = transform.GetComponent<VarPrefab>();
        LuaTable luaTable = listViewItem.luaTable;
        if (luaTable == null)
        {
            luaTable = LuaManager.Instance.CallFunction("UIMgr", "CreateCell", transform, vp.BindLuaPath)[0] as LuaTable;

        }
        else
        {
            LuaFunction lf;
            luaTable.Get("OnOpen", out lf);
            lf.Call(idx);
        }
        transform.SendMessage("ScrollCellIndex", idx);
    }
    
}
