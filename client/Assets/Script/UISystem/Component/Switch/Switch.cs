using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Switch : MonoBehaviour {
    #region 字段
    public List<Transform> m_tChildrens;
    /// <summary>
    /// 该Switch所属的组 同一组只有一个可以被选中
    /// </summary>
    public SwitchGroup m_sGroup;
    [SerializeField]
    private int m_iIndex = -1;
    [SerializeField]
    private int m_iValue = -9;
    /// <summary>
    /// 标识一个状态 回调时回传 暂时用于解决 该组件声音统一处理的时候 如果自动选择的话声音也会播放
    /// </summary>
    [HideInInspector]
    public int m_iStatus = 0;
    /// <summary>
    /// 是否允许对同一个对象多次选中并响应事件
    /// </summary>
    public bool m_bAllowSelectSameOne;
    #endregion

    #region  属性
    public int Index
    {
        get
        {
            return m_iIndex;
        }
        set
        {
            m_iIndex = value;
        }
    }
    public Action<int, int> OnValueChange
    {
        get;
        set;
    }
    public int Value
    {
        get
        {
            return m_iValue;
        }
        set
        {
            if (m_iValue == value)
            {
                //选中是判断 如果是同一个对象 则在判定是否允许选中同一个
                //允许则调用选中事件
                if (m_bAllowSelectSameOne)
                {
                    InvokeValueChange();
                }
                return;
            }

            Value2 = value;
        }
    }
    public int Value2
    {
        get
        {
            return m_iValue;
        }
        set
        {
            m_iValue = value;
            //隐藏所有的子对象
            foreach (Transform tf in m_tChildrens)
            {
                tf.SetActive(false);
            }

            //限定范围 不能超出
            if (m_iValue >= m_tChildrens.Count)
                m_iValue = m_tChildrens.Count - 1;

            //显示选中的子对象
            if (m_iValue >= 0 && m_iValue < m_tChildrens.Count)
            {
                m_tChildrens[m_iValue].SetActive(true);
            }

            InvokeValueChange();
        }
    }

    #endregion

    void Awake () {
        if (m_iValue == -9) Value = 0;
        if (m_sGroup != null)
        {
            m_sGroup.AddItem(this);
        }
        //添加点击事件 点击后置为选中状态
        if (m_tChildrens != null)
        {
            Transform transform;
            for (int i = 0; i < m_tChildrens.Count; ++i)
            {
                transform = m_tChildrens[i];
                //如果子对象是按钮 则为每个按钮添加点击事件 
                //当按钮点击时会设置当前点击按钮的顺序值索引
                if (transform.GetButton() != null)
                {
                    int index = i;
                    transform.AddClickListener(() => {
                        SetValue(index);
                    });
                }
            }
        }
    }
    
    public void SetValue(int value, int status = 0)
    {
        //标识状态 回调时回传
        this.m_iStatus = status;

        if (m_sGroup != null)
        {
            m_sGroup.SelectItem = this;
        }
        else
        {
            Value = value;
        }
    }

    public void SetValue2(int value, int status = 0)
    {
        //标识状态 回调时回传
        this.m_iStatus = status;

        if (m_sGroup != null)
        {
            m_sGroup.SelectItem2 = this;
        }
        else
        {
            Value2 = value;
        }
    }

    public void InvokeValueChange()
    {
        //响应回调
        if (OnValueChange != null) OnValueChange(m_iValue, m_iStatus);
    }


    /// <summary>
    /// 隐藏所有的显示对象
    /// </summary>
    public void HideChildren()
    {
        //隐藏所有的子对象
        foreach (Transform tf in m_tChildrens)
        {
            tf.SetActive(false);
        }
    }
    /// <summary>
    /// 隐藏指定的显示对象
    /// </summary>
    public void HideChild(int index = -1)
    {
        if (index == -1)
        {
            index = m_iValue;
        }
        //隐藏选中的子对象
        if (index >= 0 && index < m_tChildrens.Count)
        {
            m_tChildrens[index].SetActive(false);
        }
    }
    /// <summary>
    /// 显示指定的显示对象
    /// </summary>
    public void ShowChild(int index = -1)
    {
        if (index == -1)
        {
            index = m_iValue;
        }
        //显示选中的子对象
        if (index >= 0 && index < m_tChildrens.Count)
        {
            m_tChildrens[index].SetActive(true);
        }
    }

    /// <summary>
    /// 直接切换选中状态 不发生事件
    /// </summary>
    /// <param name="value"></param>
    public void IsOn(int value)
    {
        m_iValue = value;
        //隐藏所有的子对象
        foreach (Transform tf in m_tChildrens)
        {
            tf.SetActive(false);
        }

        //限定范围 不能超出
        if (m_iValue >= m_tChildrens.Count)
            m_iValue = m_tChildrens.Count - 1;

        //显示选中的子对象
        if (m_iValue >= 0 && m_iValue < m_tChildrens.Count)
        {
            m_tChildrens[m_iValue].SetActive(true);
        }
    }

    /// <summary>
    /// 对象销毁时处理
    /// </summary>
    void OnDestroy()
    {
        OnValueChange = null;
        m_tChildrens = null;
        m_sGroup = null;
    }

}
