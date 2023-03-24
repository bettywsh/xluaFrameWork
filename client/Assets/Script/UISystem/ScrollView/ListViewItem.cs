using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using XLua;

namespace UnityEngine.UI
{
    public class ListViewItem : MonoBehaviour
    {
        public LuaTable luaTable;
        public Button OnClickButton;
        public GameObject SelectedObject;
        public GameObject UnSelectedObject;
        // private int index;

        void ScrollCellIndex(int idx)
        {
            if(OnClickButton != null)
                OnClickButton.onClick.AddListener(OnClickCallBack);

        }

        public void OnClickCallBack()
        {
            if (SelectedObject != null)
                SelectedObject.SetActive(true);
            LuaFunction lf;
            luaTable.Get("OnClickCallBack", out lf);
            lf.Call();
        }
    }

    
}
