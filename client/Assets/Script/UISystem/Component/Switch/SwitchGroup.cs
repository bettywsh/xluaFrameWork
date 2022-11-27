using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SwitchGroup : MonoBehaviour {
    #region 字段
    /// <summary>
    /// Switch集合
    /// </summary>
    private List<Switch> m_sItems = new List<Switch>();
    /// <summary>
    /// 当前选中项
    /// </summary>
    private Switch m_sSelectItem;
    /// <summary>
    /// 是否在Start时打开第一个选项 默认为true
    /// </summary>
    [SerializeField]
    private bool m_bStartDefault = true;
    /// <summary>
    /// 是否允许对同一个对象多次选中并响应事件
    /// </summary>
    public bool m_bAllowSelectSameOne;
    #endregion
    #region  属性
    public Action<int> OnValueChange
    {
        get;
        set;
    }
    #endregion
    void Start()
    {
        if (m_bStartDefault && m_sItems != null)
        {
            //默认选中第一个
            foreach (Switch item in m_sItems)
            {
                if (item.Index == 0)
                {
                    item.SetValue(1, 1);
                    break;
                }
            }
        }
    }
    /// <summary>
    /// 将一个Switch添加到集合中
    /// </summary>
    /// <param name="item"></param>
    public void AddItem(Switch item)
    {
        m_sItems.Add(item);
    }

    public Switch SelectItem
    {
        get
        {
            return m_sSelectItem;
        }
        set
        {
            if (m_sSelectItem == value)
            {
                //如果允许选中同一个 则选中时
                //只会调用选中事件
                if (m_bAllowSelectSameOne)
                {
                    m_sSelectItem.InvokeValueChange();
                    InvokeValueChange();
                }
                else
                {
                    //如果SwitchGroup不允许选中同一个
                    //则进入Switch中 判断
                    //设置Value时 如果相同则会阻挡
                    m_sSelectItem.Value = 1;
                }
                return;
            }

            SelectItem2 = value;
        } 
    }

    public Switch SelectItem2
    {
        get
        {
            return m_sSelectItem;
        }
        set
        {
            m_sSelectItem = value;

            foreach (Switch item in m_sItems)
                item.Value = 0;

            m_sSelectItem.Value = 1;

            InvokeValueChange();
        }
    }

    public void InvokeValueChange()
    {
        //响应回调
        if (OnValueChange != null)
        {
            OnValueChange(m_sSelectItem.Index);
        }
    }

    /// <summary>
    /// 对象销毁时处理
    /// </summary>
    void OnDestroy()
    {
        OnValueChange = null;
        m_sItems = null;
        m_sSelectItem = null;
    }
}
