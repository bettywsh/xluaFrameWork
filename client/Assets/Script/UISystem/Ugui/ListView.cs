using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using XLua;
using System.Linq;

[RequireComponent(typeof(UnityEngine.UI.RectMask2D))]
[RequireComponent(typeof(UnityEngine.UI.LoopScrollRect))]
[DisallowMultipleComponent]
public class ListView : MonoBehaviour, LoopScrollPrefabSource, LoopScrollDataSource
{
    public GameObject Item;

    private int mtotalCount = -1;
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

    private LuaTable Panel;
    private LuaFunction ItemRender;
    private LuaFunction ClickRender;

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
        //trans.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
        trans.gameObject.SetActive(false);
        trans.SetParent(transform, false);
        pool.Push(trans);
    }

    public void ProvideData(Transform transform, int idx)
    {

        //listViewItem.ScrollCellIndex(idx);
        //transform.SendMessage("ScrollCellIndex", idx, this);
        ListViewItem listViewItem = transform.GetComponent<ListViewItem>();
        listViewItem.ScrollCellIndex(idx);
        ItemRender.Call(Panel, idx, listViewItem.luaTable);
    }

    public LuaTable[] GetItems()
    {
        List<LuaTable> listViewItem = new List<LuaTable>();
        var lsr = GetComponent<LoopScrollRect>();
        for (int i = lsr.content.childCount - 1; i >= 0; i--)
        {
            listViewItem.Add(lsr.content.GetChild(i).GetComponent<ListViewItem>().luaTable);
        }
        return listViewItem.ToArray();
    }

    public void SetOnItemRender(LuaTable panel, LuaFunction itemRender)
    {
        this.Panel = panel;
        this.ItemRender = itemRender;
    }

}
