using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;
using UnityEngine.UIElements;

namespace UnityEngine.UI
{
    public class ListViewItem : MonoBehaviour
    {
        public LuaTable luaTable;
        private int index = 0;
        //public Button OnClickButton;
        //public GameObject SelectedObject;
        //public GameObject UnSelectedObject;

        public void ScrollCellIndex(int idx)
        {
            index = idx;
            VarPrefab vp = transform.GetComponent<VarPrefab>();
            string className = transform.name.Replace("(Clone)", "").Replace("##", "");
            transform.name = className + idx;
            if (luaTable == null)
            {
                luaTable = LuaManager.Instance.CallFunction("UIMgr", "CreateCell", "UI/Cell/" + className, transform, idx)[0] as LuaTable;
            }
            //else
            //{
            //    LuaFunction lf;
            //    luaTable.Get("OnOpen", out lf);
            //    lf.Call(luaTable, idx);
            //}
            //if (OnClickButton != null)
            //    OnClickButton.onClick.AddListener(OnClickCallBack);

        }

        //public void OnClickCallBack()
        //{
        //    if (SelectedObject != null)
        //        SelectedObject.SetActive(true);
        //    LuaFunction lf;
        //    luaTable.Get("OnClickCallBack", out lf);
        //    lf.Call();
        //}

        public void OnDestroy()
        {
            //LuaFunction lf;
            //luaTable.Get("Close", out lf);
            //lf.Call(luaTable);
            luaTable.Dispose();
            luaTable = null;
        }
    }
}
