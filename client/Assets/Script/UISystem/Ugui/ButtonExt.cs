using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public class ButtonExt : Button
{
    [Serializable]
    public struct ScaleTweenInfo
    {
        public bool shouldTween;
        public Rect rect;
        public Vector3 localScale;
        public float duration;

    }


    [SerializeField]
    private ScaleTweenInfo m_ScaleTweenInfo;

    [SerializeField]
    private float m_DoubleTime = 0.3f;
    [SerializeField]
    private float m_LongClickTime = 0.4f;
    [SerializeField]
    private float m_LongPressTime = 0.4f;
    [SerializeField]
    private float m_LongIntervalTime = 0.1f;


    [SerializeField]
    private UnityEvent m_OnDoubleClick = new UnityEvent();

    [SerializeField]
    private UnityEvent m_OnLongClick = new UnityEvent();

    [SerializeField]
    private UnityEvent m_OnLongPress = new UnityEvent();
    
    
    
    public UnityEvent onDoubleClick
    {
        get { return m_OnDoubleClick; }
        set { m_OnDoubleClick = value; }
    }

    public UnityEvent onLongClick
    {
        get { return m_OnLongClick; }
        set { m_OnLongClick = value; }
    }

    public UnityEvent onLongPress
    {
        get { return m_OnLongPress; }
        set { m_OnLongPress = value; }
    }

    private TweenerCore<Vector3, Vector3, VectorOptions> scaleTween;
    protected bool shouldLongClick { get { return (m_OnLongClick.GetPersistentEventCount() > 0) && m_LongClickTime > 0; } }

    protected float m_firstClickTime = 0;
    protected float m_secondClickTime = 0;
    protected float m_firstPressTimeForClick = 0;
    protected bool m_isLongClick = false;
    protected float m_firstPressTimeForPress = 0;
    protected int m_pressCount = 0;
    protected bool m_isFouces = false;
    protected bool m_isInBtn = true;
    
    public virtual void OnSubmit(BaseEventData eventData)
    {
        
    }
    
    public virtual void OnPointerClick(BaseEventData eventData)
    {
        
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (!IsActive() || !IsInteractable())
            return;
        if (shouldLongClick)
        {
            m_firstPressTimeForClick = Time.unscaledTime;
        }
        //if (shouldLongPress)
        //{
        //    m_firstPressTimeForPress = Time.unscaledTime;
        //    m_isFouces = true;
        //}
        if (m_ScaleTweenInfo.shouldTween)
        {
            RectTransform rectTransform = (RectTransform) transform;
            rectTransform.DOScale(m_ScaleTweenInfo.localScale, 0.2f);
        }
        
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);

        if (m_ScaleTweenInfo.shouldTween)
        {
            RectTransform rectTransform = (RectTransform)transform;
            rectTransform.DOScale(new Vector3(1, 1, 1), 0.2f);
        }

        if (m_firstPressTimeForClick != 0)
        {
            m_firstPressTimeForClick = 0;
            if (m_isLongClick)
            {
                m_isLongClick = !m_isLongClick;
            }
        }
        if (m_firstPressTimeForPress != 0)
        {
            m_firstPressTimeForPress = 0;
            if (m_pressCount != 0)
            {
                m_pressCount = 0;
            }
            m_isFouces = false;
        }
    }
   
    
    private void OnDoubleClick()
    {
        m_OnDoubleClick.Invoke();
    }

    private void OnLongClick()
    {
        m_OnLongClick.Invoke();
    }

    private void OnLongPress()
    {
        m_OnLongPress.Invoke();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        if (m_ScaleTweenInfo.shouldTween)
        {
            m_ScaleTweenInfo.localScale = new Vector3(0.9f, 0.9f, 1);
            m_ScaleTweenInfo.duration = 0.2f;
        }

    }

    private void Update()
    {
        //双击
        if (m_firstClickTime > 0)
        {
            float interval = Time.unscaledTime - m_firstClickTime;
            if (interval > m_DoubleTime)
            {
                //双击失败  点击或长点击
                if (m_isLongClick)
                {
                    OnLongClick();
                    m_isLongClick = false;
                }
                m_firstClickTime = 0;
            }
        }
        //长点击
        //if (m_firstPressTimeForClick > 0)
        //{
        //    float interval = Time.unscaledTime - m_firstPressTimeForClick;
        //    if (interval >= m_LongClickTime)
        //    {
        //        m_isLongClick = true;
        //        m_firstPressTimeForClick = 0;
        //    }
        //}
        //长按
        if (m_firstPressTimeForPress > 0)
        {
            float interval = Time.unscaledTime - m_firstPressTimeForPress;
            if (m_pressCount == 0)
            {
                if (interval >= m_LongPressTime)
                {
                    OnLongPress();
                    m_pressCount++;
                }
            }
            else
            { 
                if (interval >= m_LongPressTime + m_LongIntervalTime * m_pressCount)
                {
                    Debug.Log(m_pressCount);
                    //长按 连续触发
                    if (m_isFouces)
                    {
                        OnLongPress();
                    }
                    m_pressCount++;
                }
            }
        }
    }
}
