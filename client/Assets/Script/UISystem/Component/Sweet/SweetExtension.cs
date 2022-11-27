using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public static class SweetExtension
{
    #region Animation动画扩展
    /// <summary>
    /// 添加Animation动画播放完成回调
    /// </summary>
    /// <param name="onCompleted"></param>
    public static void AddAnimationListener(this Animation anim, Action onCompleted)
    {
        Sweet sweet = anim.transform.gameObject.GetComponent<Sweet>();
        if (sweet == null)
        {
            sweet = anim.transform.gameObject.AddComponent<Sweet>();
        }
        sweet.AddAnimationListener(anim, onCompleted);
    }
    #endregion

    #region ParticleSystem动画扩展
    /// <summary>
    /// 添加Animation动画播放完成回调
    /// </summary>
    /// <param name="onCompleted"></param>
    public static void AddParticleSystemListener(this ParticleSystem particleSystem, Action onCompleted)
    {
        Sweet sweet = particleSystem.transform.gameObject.GetComponent<Sweet>();
        if (sweet == null)
        {
            sweet = particleSystem.transform.gameObject.AddComponent<Sweet>();
        }
        sweet.AddParticleSystemListener(particleSystem, onCompleted);
    }
    #endregion

    #region Toggle扩展
    public static bool IsOn(this Transform transform, string findPath)
    {
        return transform.SafeTransformFind(findPath).IsOn();
    }
    public static bool IsOn(this Transform transform)
    {
        Toggle toggle = transform.GetComponent<Toggle>();
        if (toggle == null)
        {
            Debug.LogError("Get Toggle Is Null Path=" + transform.name);
            return false;
        }
        return toggle.isOn;
    }
    public static void IsOn(this Transform transform, string findPath, bool value)
    {
        transform.SafeTransformFind(findPath).IsOn(value);
    }
    public static void IsOn(this Transform transform, bool value)
    {
        Toggle toggle = transform.GetComponent<Toggle>();
        if (toggle == null)
        {
            Debug.LogError("Set Toggle Is Null Path=" + transform.name);
            return;
        }
        toggle.isOn = value;
    }

    public static Toggle GetToggle(this Transform transform, string findPath)
    {
        return transform.SafeTransformFind(findPath).GetToggle();
    }
    public static Toggle GetToggle(this Transform transform)
    {
        Toggle toggle = transform.GetComponent<Toggle>();
        if (toggle == null)
        {
            Debug.LogError("Get GetToggle Is Null Path=" + transform.name);
            return null;
        }
        return toggle;
    }

    /// <summary>
    /// 给Toggle OnValueChange添加事件
    /// </summary>
    /// <param name="findPath"></param>
    /// <param name="call"></param>
    public static Toggle AddToggleListener(this Transform transform, string findPath, UnityEngine.Events.UnityAction<bool> call)
    {
        return transform.Find(findPath).AddToggleListener(call);
    }
    public static Toggle AddToggleListener(this Transform transform, UnityEngine.Events.UnityAction<bool> call)
    {
        Toggle toggle = transform.GetToggle();
        Toggle.ToggleEvent evt = toggle.onValueChanged;
        evt.RemoveAllListeners();
        //Debug.LogError("@AddToggleListener:" + transform.name);
        evt.AddListener(call);
        return toggle;
    }
    public static void RemoveToggleListener(this Transform transform, string findPath)
    {
        transform.Find(findPath).RemoveToggleListener();
    }
    public static void RemoveToggleListener(this Transform transform)
    {
        Toggle.ToggleEvent evt = transform.GetComponent<Toggle>().onValueChanged;
        evt.RemoveAllListeners();
    }
    #endregion

    #region DropDown扩展
    public static Dropdown GetDropdown(this Transform transform, string findPath)
    {
        return transform.SafeTransformFind(findPath).GetDropdown();
    }
    public static Dropdown GetDropdown(this Transform transform)
    {
        Dropdown dropDown = transform.GetComponent<Dropdown>();
        if (dropDown == null)
        {
            Debug.LogError("Get Dropdown Is Null Path=" + transform.name);
            return null;
        }
        return dropDown;
    }

    public static Dropdown AddDropdownListener(this Transform transform, string findPath, UnityEngine.Events.UnityAction<int> call)
    {
        return transform.Find(findPath).AddDropdownListener(call);
    }
    public static Dropdown AddDropdownListener(this Transform transform, UnityEngine.Events.UnityAction<int> call)
    {
        Dropdown dropdown = transform.GetDropdown();
        Dropdown.DropdownEvent evt = dropdown.onValueChanged;
        evt.RemoveAllListeners();
        //Debug.LogError("@AddDropdownListener:" + transform.name);
        evt.AddListener(call);
        return dropdown;
    }
    public static void RemoveDropdownListener(this Transform transform, string findPath)
    {
        transform.Find(findPath).RemoveDropdownListener();
    }
    public static void RemoveDropdownListener(this Transform transform)
    {
        Dropdown.DropdownEvent evt = transform.GetComponent<Dropdown>().onValueChanged;
        evt.RemoveAllListeners();
    }
    #endregion

    #region Switch组件 和 SwitchGroup组件
    public static int GetSwitchValue(this Transform transform, string findPath)
    {
        return transform.SafeTransformFind(findPath).GetSwitchValue();
    }
    public static int GetSwitchValue(this Transform transform)
    {
        Switch s = transform.GetComponent<Switch>();
        if (s == null)
        {
            Debug.LogError("Get Switch Is Null Path=" + transform.name);
            return 0;
        }
        return s.Value;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="findPath"></param>
    /// <param name="value"></param>
    /// <param name="status">表示状态 遇到一个问题 如果声音统一处理则 自动选中一个项目时 也会播放 用此参数屏蔽</param>
    public static void SetSwitchValue(this Transform transform, string findPath, int value, int status = 0)
    {
        transform.SafeTransformFind(findPath).SetSwitchValue(value, status);
    }
    public static void SetSwitchValue(this Transform transform, int value, int status = 0)
    {
        Switch s = transform.GetComponent<Switch>();
        if (s == null)
        {
            Debug.LogError("Set Switch Is Null Path=" + transform.name);
            return;
        }
        s.SetValue(value, status);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="findPath"></param>
    /// <param name="value"></param>
    /// <param name="status">表示状态 遇到一个问题 如果声音统一处理则 自动选中一个项目时 也会播放 用此参数屏蔽</param>
    public static void SetSwitchValue2(this Transform transform, string findPath, int value, int status = 0)
    {
        transform.SafeTransformFind(findPath).SetSwitchValue2(value, status);
    }
    public static void SetSwitchValue2(this Transform transform, int value, int status = 0)
    {
        Switch s = transform.GetComponent<Switch>();
        if (s == null)
        {
            Debug.LogError("Set Switch Is Null Path=" + transform.name);
            return;
        }
        s.SetValue2(value, status);
    }

    public static void SetSwitchIndex(this Transform transform, string findPath, int value)
    {
        transform.SafeTransformFind(findPath).SetSwitchIndex(value);
    }
    public static void SetSwitchIndex(this Transform transform, int value)
    {
        Switch s = transform.GetComponent<Switch>();
        if (s == null)
        {
            Debug.LogError("Set Switch Is Null Path=" + transform.name);
            return;
        }
        s.Index = value;
    }
    /// <summary>
    /// 给Switch OnValueChange添加事件
    /// </summary>
    /// <param name="findPath"></param>
    /// <param name="call"></param>
    public static void AddSwitchListener(this Transform transform, string findPath, Action<int, int> call)
    {
        transform.Find(findPath).AddSwitchListener(call);
    }
    public static void AddSwitchListener(this Transform transform, Action<int, int> call)
    {
        //DBG.Log("@AddSwitchListener:" + transform.name);
        transform.GetComponent<Switch>().OnValueChange = call;
    }
    public static void RemoveSwitchListener(this Transform transform, string findPath)
    {
        transform.Find(findPath).RemoveSwitchListener();
    }
    public static void RemoveSwitchListener(this Transform transform)
    {
        transform.GetComponent<Switch>().OnValueChange = null;
    }
    /// <summary>
    /// 给SwitchGroup OnValueChange添加事件
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="findPath"></param>
    /// <param name="call"></param>
    public static void AddSwitchGroupListener(this Transform transform, string findPath, Action<int> call)
    {
        transform.Find(findPath).AddSwitchGroupListener(call);
    }
    public static void AddSwitchGroupListener(this Transform transform, Action<int> call)
    {
        //DBG.Log("@AddSwitchGroupListener:" + transform.name);
        transform.GetComponent<SwitchGroup>().OnValueChange = call;
    }
    public static void RemoveSwitchGroupListener(this Transform transform, string findPath)
    {
        transform.Find(findPath).RemoveSwitchGroupListener();
    }
    public static void RemoveSwitchGroupListener(this Transform transform)
    {
        transform.GetComponent<SwitchGroup>().OnValueChange = null;
    }
    #endregion

    #region Image扩展
    public static void SetImageRayCastTarget(this Transform transform, bool isOn)
    {
        transform.GetComponent<Image>().raycastTarget = isOn;
    }
    #endregion

    #region 坐标转换扩展
    public static Vector2 ScreenPointToUI(this Transform transform, string findPath, Vector3 position, Camera camera)
    {
        return transform.SafeTransformFind(findPath).ScreenPointToUI(position, camera);
    }
    public static Vector2 ScreenPointToUI(this Transform transform, Vector3 position, Camera camera)
    {
        Vector2 localPoint = Vector2.zero;
        try
        {
            RectTransform rect = transform as RectTransform;
            RectTransform parent = rect.parent as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parent, (Vector2)position, camera, out localPoint);
            rect.anchoredPosition = localPoint;
        }
        catch (System.Exception err)
        {
            Debug.LogError("Error!!! ScreenPointToUI:" + err.Message);
        }

        return localPoint;  
    }

    public static bool ContainsScreenPoint(this Transform transform, Vector3 screenPoint, Camera camera)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(transform as RectTransform, (Vector2)screenPoint, camera);
    }
    #endregion

    #region 事件
    public static void AddClickListener(this Transform transform, string findPath, UnityEngine.Events.UnityAction call)
    {
        if (transform.SafeTransform())
        {
            transform.SafeTransformFind(findPath).AddClickListener(call);
        }
    }
    public static void AddClickListener(this Transform transform, UnityEngine.Events.UnityAction call)
    {
        if (transform.SafeTransform())
        {
            Button.ButtonClickedEvent onClick = transform.SafeTransform().GetComponent<Button>().onClick;
            onClick.RemoveAllListeners();
            onClick.AddListener(call);
            //DBG.Log("@AddClickListener:" + transform.name);
        }
    }
    public static void RemoveClickListener(this Transform transform, string findPath)
    {
        transform.Find(findPath).RemoveClickListener();
    }
    public static void RemoveClickListener(this Transform transform)
    {
        Button.ButtonClickedEvent onClick = transform.GetComponent<Button>().onClick;
        onClick.RemoveAllListeners();
        //DBG.Log("@RemoveClickListener:" + transform.name);
    }

    public static void AddClickListeners(this Transform transform, string findPath, Action<string> call)
    {
        if (transform.SafeTransform())
        {
            transform.SafeTransformFind(findPath).AddClickListeners(call);
        }
    }
    public static void AddClickListeners(this Transform transform, Action<string> call)
    {
        if (transform.SafeTransform())
        {
            Transform child;
            Button button;
            for (int i = 0; i < transform.childCount; ++i)
            {
                child = transform.GetChild(i);
                button = child.GetComponent<Button>();
                if (button != null)
                {
                    string name = child.name;
                    button.onClick.RemoveAllListeners();
                    button.onClick.AddListener(() => {
                        if (call != null)
                            call(name);
                    });
                }
            }
            //DBG.Log("@AddClickListeners:" + transform.name);
        }
    }
    #endregion

    #region 其他
    public static Button GetButton(this Transform transform, string findPath)
    {
        return transform.SafeTransformFind(findPath).GetButton();
    }
    public static Button GetButton(this Transform transform)
    {
        return transform.GetComponent<Button>();
    }
    public static void SetActive(this Transform transform, string findPath, bool value)
    {
        transform.Find(findPath).SetActive(value);
    }
    public static void SetActive(this Transform transform, bool value)
    {
        transform.gameObject.SetActive(value);
    }
    public static bool ActiveSelf(this Transform transform, string findPath)
    {
        return transform.Find(findPath).ActiveSelf();
    }
    public static bool ActiveSelf(this Transform transform)
    {
        return transform.gameObject.activeSelf;
    }

    public static Transform SafeTransform(this Transform transform)
    {
        if (transform == null)
        {
            Debug.LogError("Transform_Null");
            return null;
        }
        return transform;
    }
    public static Transform SafeTransformFind(this Transform transform, string findPath)
    {
        Transform find = transform.Find(findPath);
        if (find == null)
        {
            Debug.LogError("【" + transform.name + "】 Find Path:【" + findPath + "】 Is Null!!!");
            return transform;
        }
        return find;
    }
    #endregion

}
